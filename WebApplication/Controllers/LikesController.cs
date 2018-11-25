using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using WebApplication.Models;
using WebApplication.Repositories;
using Microsoft.AspNetCore.Authorization;
using WebApplication.Helpers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;

namespace WebApplication.Controllers
{
    [Route("api/likes")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class LikesController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ILikesRepository _likesRepository;
        private readonly ITopicRepository _topicRepository;
        private readonly IJWTValidator _tokenValidator;


        public LikesController(UserManager<User> userManager, ILikesRepository likesRepository, ITopicRepository topicRepository,
            IJWTValidator tokenValidator)
        {
            _userManager = userManager;
            _likesRepository = likesRepository;
            _topicRepository = topicRepository;
            _tokenValidator = tokenValidator;
        }

        private async Task<User> GetCurrentUser()
        {
            return await _userManager.GetUserAsync(HttpContext.User);
        }

        [HttpGet("{id}")]
        public async Task<bool> IsLiked(int id)
        {
            if (ModelState.IsValid)
            {
                var isTokenValid = await _tokenValidator.IsTokenValid(Request.Headers, HttpContext);
                if (isTokenValid)
                {
                    var email = User.FindFirst(ClaimTypes.Email).Value;
                    var user = await _userManager.FindByEmailAsync(email);
                    var like = await _likesRepository.GetLikeByTopicAndUser(id, user.Id);
                    if (like != null)
                    {
                        return like.IsLiked;
                    }
                }
            }

            return false;
        }

        [HttpPut("{id}")]
        public async Task<bool> LikeChanged(int id, [FromBody]bool isLiked)
        {
            if (ModelState.IsValid)
            {
                var isTokenValid = await _tokenValidator.IsTokenValid(Request.Headers, HttpContext);
                if (isTokenValid)
                {
                    var email = User.FindFirst(ClaimTypes.Email).Value;
                    var user = await _userManager.FindByEmailAsync(email);
                    var like = await _likesRepository.GetLikeByTopicAndUser(id, user.Id);
                    var topic = await _topicRepository.GetTopicById(id);
                    if (like != null)
                    {
                        topic.Rating = isLiked ?
                            like.IsLiked ? topic.Rating : topic.Rating + 1 :
                            like.IsLiked ? topic.Rating - 1 : topic.Rating;
                        await _topicRepository.UpdateTopic(topic.Id, topic);

                        like.IsLiked = isLiked;

                        await _likesRepository.UpdateLike(like.Id, like);
                        return like.IsLiked;
                    }
                    else
                    {
                        var newLike = new Like()
                        {
                            IsLiked = true,
                            TopicId = id,
                            UserId = user.Id
                        };
                        await _likesRepository.AddLike(newLike);

                        topic.Rating++;
                        await _topicRepository.UpdateTopic(topic.Id, topic);

                        return true;
                    }
                }
            }

            return false;
        }
    }
}
