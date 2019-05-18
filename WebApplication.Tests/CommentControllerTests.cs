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
    public class CommentControllerTests
    {
        [Fact]
        public async Task GetCommentsShouldReturnComments()
        {
            var mockData = new List<Comment>
            {
                new Comment
                {
                    Id = 1,
                    Text = "comment1",
                    TopicId = 1,
                    UserId = "1",
                    UserName = "name1",
                    Time = DateTime.Now
                },
                new Comment
                {
                    Id = 2,
                    Text = "comment2",
                    TopicId = 1,
                    UserId = "1",
                    UserName = "name1",
                    Time = DateTime.Now
                },
            };
            var commentsRepositoryMock = new Mock<ICommentRepository>();
            commentsRepositoryMock.Setup(c => c.GetAllComments())
                .ReturnsAsync(mockData);

            var controller = new CommentController(null, commentsRepositoryMock.Object, null, null);
            var result = await controller.GetAllComments(1);
            Assert.Equal(mockData[0].Text, result.ToList()[0].Text);
            Assert.Equal(mockData[0].UserName, result.ToList()[0].UserName);
        }

        [Fact]
        public async Task AddCommentShouldReturnAddedComment()
        {
            var mockData = new Comment
            {
                Id = 1,
                Text = "comment1",
                TopicId = 1,
                UserId = "1",
                UserName = "name1",
                Time = DateTime.Now
            };
            var commentViewModel = new CommentViewModel
            {
                Text="comment1",
                TopicId = 1
            };
            var commentRepositoryMock = new Mock<ICommentRepository>();
            var tokenValidatorMock = new Mock<IJWTValidator>();
            var store = new Mock<IUserStore<User>>();
            var userManagerMock = new Mock<UserManager<User>>(store.Object,
                null, null, null, null, null, null, null, null);
            //commentRepositoryMock.Setup(t => t.AddComment(mockData));
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

            var controller = new CommentController(userManagerMock.Object,
                commentRepositoryMock.Object, null, tokenValidatorMock.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var result = await controller.AddComment(commentViewModel);
            Assert.Equal(200, (result as OkResult).StatusCode);
        }

        [Fact]
        public async Task AddCommentShouldReturnBadRequestIfInvalidToken()
        {
            var mockData = new Comment
            {
                Id = 1,
                Text = "comment1",
                TopicId = 1,
                UserId = "1",
                UserName = "name1",
                Time = DateTime.Now
            };
            var commentViewModel = new CommentViewModel
            {
                Text = "comment1",
                TopicId = 1
            };
            var tokenValidatorMock = new Mock<IJWTValidator>();
            var store = new Mock<IUserStore<User>>();
            var userManagerMock = new Mock<UserManager<User>>(store.Object,
                null, null, null, null, null, null, null, null);
            tokenValidatorMock
                .Setup(t => t.IsTokenValid(It.IsAny<IHeaderDictionary>(), It.IsAny<HttpContext>()))
                .ReturnsAsync(false);

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

            var controller = new CommentController(userManagerMock.Object,
                null, null, tokenValidatorMock.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var result = await controller.AddComment(commentViewModel);
            Assert.Equal(400, (result as BadRequestResult).StatusCode);
        }

        [Fact]
        public async Task DeleteShouldDeleteCommentIfAccesTokenIsValid()
        {
            var mockData = new Comment
            {
                Id = 1,
                Text = "comment1",
                TopicId = 1,
                UserId = "1",
                UserName = "name1",
                Time = DateTime.Now
            };
            var commentViewModel = new CommentViewModel
            {
                Text = "comment1",
                TopicId = 1
            };
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Email, "example@gmail.com")
            }));
            var commentRepositoryMock = new Mock<ICommentRepository>();
            var controller = new CommentController(null,
                commentRepositoryMock.Object, null, null);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var result = await controller.DeleteComment(1);
            Assert.Equal(200, (result as OkResult).StatusCode);
        }
    }
}
