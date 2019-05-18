using System;
using Xunit;
using Moq;
using WebApplication.Repositories;
using System.Collections.Generic;
using WebApplication.Models;
using WebApplication.Controllers;
using System.Threading.Tasks;
using System.Linq;
using WebApplication.ViewModels;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace WebApplication.Tests
{
    public class LikesControllerTests
    {
        [Fact]
        public async Task IsLikedsShouldReturnTrueIfTokenValid()
        {
            var likesRepositoryMock = new Mock<ILikesRepository>();
            likesRepositoryMock.Setup(c => c.GetLikeByTopicAndUser(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(new Like
                {
                    IsLiked = true,
                    TopicId = 1,
                    UserId = "1"
                });
            var tokenValidatorMock = new Mock<IJWTValidator>();
            var store = new Mock<IUserStore<User>>();
            var userManagerMock = new Mock<UserManager<User>>(store.Object,
                null, null, null, null, null, null, null, null);
            tokenValidatorMock
                .Setup(t => t.IsTokenValid(It.IsAny<IHeaderDictionary>(), It.IsAny<HttpContext>()))
                .ReturnsAsync(true);

            userManagerMock.Setup(u => u.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(
                new User
                {
                    Id = "1"
                });

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Email, "example@gmail.com")
            }));
            var controller = new LikesController(userManagerMock.Object, likesRepositoryMock.Object, null, tokenValidatorMock.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
            var result = await controller.IsLiked(1);
            Assert.True(result);
        }

        [Fact]
        public async Task IsLikedsShouldReturnFalseIfTokenInValid()
        {
            var tokenValidatorMock = new Mock<IJWTValidator>();
            tokenValidatorMock
                .Setup(t => t.IsTokenValid(It.IsAny<IHeaderDictionary>(), It.IsAny<HttpContext>()))
                .ReturnsAsync(false);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Email, "example@gmail.com")
            }));
            var controller = new LikesController(null, null, null, tokenValidatorMock.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
            var result = await controller.IsLiked(1);
            Assert.False(result);
        }

        [Fact]
        public async Task LikeChangedShouldReturnFalseIfTokenInvalid()
        {
            var tokenValidatorMock = new Mock<IJWTValidator>();
            tokenValidatorMock
                .Setup(t => t.IsTokenValid(It.IsAny<IHeaderDictionary>(), It.IsAny<HttpContext>()))
                .ReturnsAsync(false);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Email, "example@gmail.com")
            }));
            var controller = new LikesController(null, null, null, tokenValidatorMock.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
            var result = await controller.LikeChanged(1, true);
            Assert.False(result);
        }

        [Fact]
        public async Task LikeChangedShouldReturnTrueIfLikeWasNull()
        {
            var topicRepositoryMock = new Mock<ITopicRepository>();
            topicRepositoryMock.Setup(t => t.GetTopicById(It.IsAny<int>()))
                .ReturnsAsync(new Topic
                {
                    Id = 1,
                    Rating = 0
                });
            topicRepositoryMock.Setup(t => t.UpdateTopic(It.IsAny<int>(), It.IsAny<Topic>()))
                .ReturnsAsync(new Topic());
            var likesRepositoryMock = new Mock<ILikesRepository>();
            likesRepositoryMock.Setup(c => c.GetLikeByTopicAndUser(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(Task.FromResult<Like>(null));
            var tokenValidatorMock = new Mock<IJWTValidator>();
            var store = new Mock<IUserStore<User>>();
            var userManagerMock = new Mock<UserManager<User>>(store.Object,
                null, null, null, null, null, null, null, null);
            tokenValidatorMock
                .Setup(t => t.IsTokenValid(It.IsAny<IHeaderDictionary>(), It.IsAny<HttpContext>()))
                .ReturnsAsync(true);

            userManagerMock.Setup(u => u.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(
                new User
                {
                    Id = "1"
                });

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Email, "example@gmail.com")
            }));
            var controller = new LikesController(userManagerMock.Object,
                likesRepositoryMock.Object, topicRepositoryMock.Object, tokenValidatorMock.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
            var result = await controller.LikeChanged(1, true);
            Assert.True(result);
        }

        [Fact]
        public async Task LikeChangedShouldReturnFalseIfLikeWasNotNull()
        {
            var topicRepositoryMock = new Mock<ITopicRepository>();
            topicRepositoryMock.Setup(t => t.GetTopicById(It.IsAny<int>()))
                .ReturnsAsync(new Topic
                {
                    Id = 1,
                    Rating = 0
                });
            topicRepositoryMock.Setup(t => t.UpdateTopic(It.IsAny<int>(), It.IsAny<Topic>()))
                .ReturnsAsync(new Topic());
            var likesRepositoryMock = new Mock<ILikesRepository>();
            likesRepositoryMock.Setup(c => c.GetLikeByTopicAndUser(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(new Like
                {
                    Id = 1,
                    IsLiked = true
                });
            likesRepositoryMock.Setup(l => l.UpdateLike(It.IsAny<int>(), It.IsAny<Like>()))
                .ReturnsAsync(new Like());
            var tokenValidatorMock = new Mock<IJWTValidator>();
            var store = new Mock<IUserStore<User>>();
            var userManagerMock = new Mock<UserManager<User>>(store.Object,
                null, null, null, null, null, null, null, null);
            tokenValidatorMock
                .Setup(t => t.IsTokenValid(It.IsAny<IHeaderDictionary>(), It.IsAny<HttpContext>()))
                .ReturnsAsync(true);

            userManagerMock.Setup(u => u.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(
                new User
                {
                    Id = "1"
                });

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Email, "example@gmail.com")
            }));
            var controller = new LikesController(userManagerMock.Object,
                likesRepositoryMock.Object, topicRepositoryMock.Object, tokenValidatorMock.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
            var result = await controller.LikeChanged(1, false);
            Assert.False(result);
        }
    }
}
