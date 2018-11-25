using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using WebApplication.Models;
using WebApplication.Repositories;
using Microsoft.AspNetCore.Authorization;
using WebApplication.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using WebApplication.Helpers;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApplication.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;

namespace WebApplication.Controllers
{
    [Route("api/comment")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CommentController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ICommentRepository _commentRepository;
        private readonly ITopicRepository _topicRepository;
        private readonly IJWTValidator _tokenValidator;


        public CommentController(UserManager<User> userManager, ICommentRepository commentRepository, ITopicRepository topicRepository,
            IJWTValidator tokenValidator)
        {
            _userManager = userManager;
            _commentRepository = commentRepository;
            _topicRepository = topicRepository;
            _tokenValidator = tokenValidator;
        }

        [HttpGet("{topicId}")]
        [AllowAnonymous]
        public async Task<IEnumerable<Comment>> GetAllComments(int topicId)
        {
            if (ModelState.IsValid)
            {
                var comments = await _commentRepository.GetAllComments();
                comments = comments.Where(c => c.TopicId == topicId);
                comments.ToList().Sort((c1, c2) => { return c1.Time.CompareTo(c2.Time); });
                comments.ToList().ForEach(c =>
                {
                    c.Time = c.Time.ToLocalTime();
                    c.UserId = null;
                });
                return comments;
            }

            return null;
        }

        [HttpPost]
        public async Task AddComment([FromBody]CommentViewModel comment)
        {
            if (ModelState.IsValid)
            {
                var isTokenValid = await _tokenValidator.IsTokenValid(Request.Headers, HttpContext);
                if (isTokenValid)
                {
                    var commentToAdd = new Comment() {TopicId = comment.TopicId, Text = comment.Text };
                    commentToAdd.Time = DateTime.UtcNow;
                    var email = User.FindFirst(ClaimTypes.Email).Value;
                    var user = await _userManager.FindByEmailAsync(email);
                    commentToAdd.UserId = user.Id;
                    commentToAdd.UserName = user.UserName;
                    await _commentRepository.AddComment(commentToAdd);
                }
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Constants.Admin)]
        public async Task<IActionResult> DeleteComment(int id)
        {
            if (ModelState.IsValid)
            {
                await _commentRepository.DeleteComment(id);

                return Ok();
            }

            return BadRequest();
        }
    }
}
