﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using WebApplication.Models;
using WebApplication.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using WebApplication.Helpers;
using WebApplication.Common;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace WebApplication.Controllers
{
    [Route("api/account")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AccountController : Controller
    {
        private const string JWTIssuer = "Jwt:Issuer";
        private const string JWTKey = "Jwt:Key";
        private const string Null = "null";

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly IJWTValidator _tokenValidator;
        private IConfiguration _config;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager,
            IConfiguration config, IEmailSender emailSender, IJWTValidator iTokenValidatorValidator)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _config = config;
            _tokenValidator = iTokenValidatorValidator;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody]RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User { Email = model.Email, UserName = model.Username };
                //var email = User.FindFirst(ClaimTypes.Email)?.Value;
                var currentUser = await _userManager.FindByEmailAsync(model.Email);

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded && currentUser == null)
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = code }, Request.Scheme);
                    await _emailSender.SendEmailAsync(user.Email, "Confirm your account",
                        $"Please confirm your account by clicking this link: <a href='{"http://localhost:4200/confirmEmail?userId=" + user.Id + "&token=" + code}'>link</a>");
                    return Ok();
                }

                return BadRequest(result.Errors?.First()?.Description);
            }

            return BadRequest("Invalid model");
        }

        [HttpPost("confirmEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailViewModel confirmEmail)
        {
            var user = await _userManager.FindByIdAsync(confirmEmail.UserId);
            var result = await _userManager.ConfirmEmailAsync(user, confirmEmail.Token);
            if (result.Succeeded)
            {
                return Ok();
            }

            return BadRequest(result.Errors?.First()?.Description);
        }

        [HttpPost("forgotPassword")]
        [AllowAnonymous]
        public async Task ForgotPassword([FromBody]ForgotPasswordViewModel forgotPasswordModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(forgotPasswordModel.Email);
                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    if (token != null)
                    {
                        var callbackUrl = Url.Action("RecoverPassword", "Account", new { email = user.Email, token = token }, Request.Scheme);
                        await _emailSender.SendEmailAsync(user.Email, "Password recovery",
                            $"Please recover your password by clicking this link: <a href='{"https://dublog.herokuapp.com/recoverPassword/?email=" + user.Email + "&token=" + token}'>link</a>");
                    }
                }
            }
        }

        [HttpPost("RecoverPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> RecoverPassword([FromBody]RecoverPasswordViewModel recoverPasswordModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(recoverPasswordModel.Email);
                if (user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, recoverPasswordModel.Token, recoverPasswordModel.Password);
                    if (result.Succeeded)
                    {
                        return Ok();
                    }
                }
            }

            return BadRequest();
        }

        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordViewModel changePasswordModel)
        {
            if (ModelState.IsValid)
            {
                var isTokenValid = await _tokenValidator.IsTokenValid(Request.Headers, HttpContext);
                if (isTokenValid)
                {
                    var email = User.FindFirst(ClaimTypes.Email).Value;
                    var user = await _userManager.FindByEmailAsync(email);
                    var result = await _userManager.ChangePasswordAsync(user, changePasswordModel.OldPassword, changePasswordModel.NewPassword);
                    if (result.Succeeded)
                    {
                        return Ok();
                    }
                }
            }

            return BadRequest();
        }

        [HttpPost("lockOutUser")]
        [Authorize(Roles = Constants.Admin)]
        public async Task<IActionResult> LockOutUser([FromBody]string userId)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(userId);
                var result = await _userManager.SetLockoutEndDateAsync(user, new DateTimeOffset(DateTime.UtcNow.AddDays(1)));
                if (result.Succeeded)
                {
                    return Ok();
                }
            }

            return BadRequest();
        }

        [HttpGet]
        public async Task<bool> CanAccessAdminFields()
        {
            if (ModelState.IsValid)
            {
                var isTokenValid = await _tokenValidator.IsTokenValid(Request.Headers, HttpContext);
                if (isTokenValid)
                {
                    var email = User.FindFirst(ClaimTypes.Email).Value;
                    var currentUser = await _userManager.FindByEmailAsync(email);
                    bool existRole = await _roleManager.RoleExistsAsync(Constants.Admin);
                    if (!existRole)
                    {
                        var res = await _roleManager.CreateAsync(new IdentityRole(Constants.Admin));
                        var resu = await _userManager.AddToRoleAsync(currentUser, Constants.Admin);
                    }
                    var result = await _userManager.IsInRoleAsync(currentUser, Constants.Admin);
                    return result;
                }
            }

            return false;
        }

        [HttpGet("logInIfValidToken")]
        [AllowAnonymous]
        public async Task LogInIfValidToken()
        {
            if (ModelState.IsValid)
            {
                if (Request.Headers.ContainsKey(Constants.Authorization) &&
                    Request.Headers[Constants.Authorization].ToString().Contains(' ') &&
                    !Request.Headers[Constants.Authorization].ToString().Contains(Null))
                {
                    var token = Request.Headers[Constants.Authorization].ToString().Split(' ')[1];
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var tokenInfo = tokenHandler.ReadJwtToken(token.ToString());

                    var email = User.FindFirst(ClaimTypes.Email).Value;
                    var currentUser = await _userManager.FindByEmailAsync(email);
                    var user = _userManager.Users.ToList()
                        .Find(u => u.Id == tokenInfo.Claims.ToList().Find(c => c.Type == JwtRegisteredClaimNames.Jti).Value);
                    if (user.Email == tokenInfo.Claims.ToList().Find(c => c.Type == JwtRegisteredClaimNames.Email).Value &&
                        user.UserName == tokenInfo.Claims.ToList().Find(c => c.Type == JwtRegisteredClaimNames.Sub).Value &&
                        currentUser == null)
                    {
                        var res = await _signInManager.CanSignInAsync(user);
                        await _signInManager.SignInAsync(user, false);
                    }
                }
            }
        }

        [HttpGet("userDetails")]
        public async Task<User> GetUserDetails()
        {
            var isTokenValid = await _tokenValidator.IsTokenValid(Request.Headers, HttpContext);
            if (isTokenValid)
            {
                var email = User.FindFirst(ClaimTypes.Email).Value;
                var user = await _userManager.FindByEmailAsync(email);
                return user;
            }

            return null;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody]LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(model.Username);
                    var token = BuildToken(user);
                    //var role = await _userManager.GetRolesAsync(user);
                    //user.Role = role.FirstOrDefault();
                    //user.LockoutEnabled = false;
                    return Ok(token);
                }
                else
                {
                    if (result.IsNotAllowed)
                    {
                        return BadRequest("Confirm your email to log in");
                    }
                    else
                    {
                        return BadRequest("Wrong password or login");
                    }
                }
            }

            return BadRequest("Wrong password or login");
        }

        [HttpPost("logout")]
        [AllowAnonymous]
        public async Task<IActionResult> LogOff()
        {
            var isTokenValid = await _tokenValidator.IsTokenValid(Request.Headers, HttpContext);
            if (isTokenValid)
            {
                await _signInManager.SignOutAsync();
                return Ok();
            }

            return BadRequest();
        }

        private string BuildToken(User user)
        {

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config[JWTKey]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config[JWTIssuer],
              _config[JWTIssuer],
              claims,
              expires: DateTime.Now.AddMinutes(30),
              signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
