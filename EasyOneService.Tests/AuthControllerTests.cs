using FluentAssertions;
using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Textile.Core.Entities.Enums;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Response;
using Textile.Core.Managers.Commands;
using EasyOneService.Controllers;
namespace EasyOneService.Tests
{
    public class AuthControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ILogger<AuthController>> _loggerMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<AuthController>>();

            _controller = new AuthController(
                _loggerMock.Object,
                _mediatorMock.Object);
        }
    

        [Fact]
        public async Task Login_Should_ReturnOk_When_LoginSuccessful()
        {
            // Arrange
            var request = new LoginRequest
            {
                UserName = "test",
                Password = "123",
                ClientType = ClientType.Web
            };

            var loginResponse = new LoginResponse
            {
                Token = "jwt-token",
                RefreshToken = "refresh-token"
            };

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(loginResponse);

            // Act
            var result = await _controller.Login(request);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(loginResponse);
        }


        [Fact]
        public async Task Login_Should_ReturnUnauthorized_When_ExceptionThrown()
        {
            var request = new LoginRequest
            {
                UserName = "test",
                Password = "123",
                ClientType = ClientType.Web
            };

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new UnauthorizedAccessException("Invalid credentials"));

            var result = await _controller.Login(request);

            var unauthorizedResult = result.Should().BeOfType<UnauthorizedObjectResult>().Subject;
            unauthorizedResult.Value.Should().BeEquivalentTo(new
            {
                message = "Invalid credentials"
            });
        }


        [Fact]
        public async Task Refresh_Should_ReturnOk_When_Successful()
        {
            var request = new RefreshTokenRequest
            {
                RefreshToken = "refresh-token",
                ClientType = ClientType.Web
            };

            var response = new LoginResponse
            {
                Token = "new-token"
            };

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<RefreshTokenCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await _controller.Refresh(request);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task Refresh_Should_ReturnUnauthorized_When_ExceptionThrown()
        {
            var request = new RefreshTokenRequest
            {
                RefreshToken = "refresh-token",
                ClientType = ClientType.Web
            };

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<RefreshTokenCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new UnauthorizedAccessException("Invalid token"));

            var result = await _controller.Refresh(request);

            var unauthorizedResult = result.Should().BeOfType<UnauthorizedObjectResult>().Subject;
            unauthorizedResult.Value.Should().BeEquivalentTo(new
            {
                message = "Invalid token"
            });
        }
    }
}
