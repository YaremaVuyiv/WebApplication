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
    public class TopicControllerTests
    {
        [Fact]
        public async Task GetTopicsShouldReturnListOfTopics()
        {
            var mockData = new List<Topic> {
                new Topic
                {
                    Id = 1,
                    CreatorId = "1",
                    Body = "body1",
                    Name = "name1"
                },
                new Topic
                {
                    Id = 2,
                    CreatorId = "2",
                    Body = "body2",
                    Name = "name2"
                },
            };
            var topicRepositoryMock = new Mock<ITopicRepository>();
            topicRepositoryMock.Setup(t => t.GetAllTopics()).ReturnsAsync(mockData);
            var controller = new TopicController(topicRepositoryMock.Object, null, null);
            var result = await controller.Get();
            Assert.Equal(mockData.Count, result.ToList().Count);
            Assert.Equal(mockData[0].Id, result.ToList()[0].Id);
        }

        [Fact]
        public async Task GetTopicByIdShouldReturnTopic()
        {
            var mockData = new Topic
            {
                Id = 1,
                CreatorId = "1",
                Body = "body1",
                Name = "name1"
            };
            var topicRepositoryMock = new Mock<ITopicRepository>();
            topicRepositoryMock.Setup(t => t.GetTopicById(It.IsAny<int>())).ReturnsAsync(mockData);
            var controller = new TopicController(topicRepositoryMock.Object, null, null);
            var result = await controller.Get(1);
            Assert.Equal(mockData.Id, result.Id);
            Assert.Equal(mockData.Name, result.Name);
        }

        [Fact]
        public async Task PostShouldReturnAddedTopic()
        {
            var mockData = new Topic
            {
                Id = 1,
                CreatorId = "1",
                Body = "body1",
                Name = "name1"
            };
            var topicViewModel = new TopicViewModel
            {
                Body = "body1",
                Name = "name1"
            };
            var topicRepositoryMock = new Mock<ITopicRepository>();
            var tokenValidatorMock = new Mock<IJWTValidator>();
            var store = new Mock<IUserStore<User>>();
            var userManagerMock = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
            topicRepositoryMock.Setup(t => t.AddTopic(mockData));
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

            var controller = new TopicController(topicRepositoryMock.Object, userManagerMock.Object, tokenValidatorMock.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var result = await controller.Post(topicViewModel);
            Assert.Equal(200, (result as ObjectResult).StatusCode);
            var viewModel = (result as OkObjectResult).Value as Topic;
            Assert.Equal(mockData.Body, viewModel.Body);
            Assert.Equal(mockData.Name, viewModel.Name);
        }

        [Fact]
        public async Task PostShouldReturnBadRequestIfAccesTokenIsInvalid()
        {
            var topicViewModel = new TopicViewModel
            {
                Body = "body1",
                Name = null
            };
            var tokenValidatorMock = new Mock<IJWTValidator>();
            tokenValidatorMock
                .Setup(t => t.IsTokenValid(It.IsAny<IHeaderDictionary>(), It.IsAny<HttpContext>()))
                .ReturnsAsync(false);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Email, "example@gmail.com")
            }));

            var controller = new TopicController(null, null, tokenValidatorMock.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var result = await controller.Post(topicViewModel);
            Assert.Equal(400, (result as ObjectResult).StatusCode);
        }

        [Fact]
        public async Task DeletShouldReturnBadRequestIfAccesTokenIsInvalid()
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

            var controller = new TopicController(null, null, tokenValidatorMock.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var result = await controller.Delete(1);
            Assert.Equal(400, (result as BadRequestResult).StatusCode);
        }

        [Fact]
        public async Task DeleteShouldDeleteTopicIfAccesTokenIsValid()
        {
            var tokenValidatorMock = new Mock<IJWTValidator>();
            tokenValidatorMock
                .Setup(t => t.IsTokenValid(It.IsAny<IHeaderDictionary>(), It.IsAny<HttpContext>()))
                .ReturnsAsync(true);

            var topicRepositoryMock = new Mock<ITopicRepository>();
            topicRepositoryMock.Setup(t => t.GetTopicById(It.IsAny<int>())).ReturnsAsync(
                new Topic
                {
                    Id = 1
                });

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Email, "example@gmail.com")
            }));

            var controller = new TopicController(topicRepositoryMock.Object, null, tokenValidatorMock.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var result = await controller.Delete(1);
            Assert.Equal(200, (result as OkResult).StatusCode);
        }
    }
}
