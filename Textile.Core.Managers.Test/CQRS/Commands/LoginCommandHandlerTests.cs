using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Linq.Expressions;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Enums;
using Textile.Core.Interfaces.Data;
using Textile.Core.Interfaces.Services;
using Textile.Core.Managers.Commands;
using Textile.Core.Managers.Handlers;

namespace Textile.Core.Managers.Test.CQRS.Commands
{
    public class LoginCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IJwtService> _jwtServiceMock;
        private readonly Mock<IConfiguration> _configurationMock;

        private readonly Mock<IRepository<User, Guid>> _userRepoMock;
        private readonly Mock<IRepository<Supplier, Guid>> _supplierRepoMock;
        private readonly Mock<IRepository<RefreshToken, Guid>> _refreshRepoMock;

        private readonly LoginCommandHandler _handler;

        public LoginCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _jwtServiceMock = new Mock<IJwtService>();
            _configurationMock = new Mock<IConfiguration>();

            _userRepoMock = new Mock<IRepository<User, Guid>>();
            _supplierRepoMock = new Mock<IRepository<Supplier, Guid>>();
            _refreshRepoMock = new Mock<IRepository<RefreshToken, Guid>>();

            _unitOfWorkMock
                .Setup(x => x.Repository<User, Guid>())
                .Returns(_userRepoMock.Object);

            _unitOfWorkMock
                .Setup(x => x.Repository<Supplier, Guid>())
                .Returns(_supplierRepoMock.Object);

            _unitOfWorkMock
                .Setup(x => x.Repository<RefreshToken, Guid>())
                .Returns(_refreshRepoMock.Object);

            _configurationMock
                .Setup(x => x["JwtSettings:Web:RefreshTokenValidityInDays"])
                .Returns("7");

            _handler = new LoginCommandHandler(
                _unitOfWorkMock.Object,
                _jwtServiceMock.Object,
                _configurationMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_When_ClientTypeInvalid()
        {
            var command = new LoginCommand("user", "pass", (ClientType)999);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().NotBeNull();
            result.IsLoginFailed.Should().BeTrue();
            result.Message.Should().Be("Invalid client type.");
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_When_UserNotFound()
        {
            _userRepoMock
                .Setup(x => x.GetSingleAsync(It.IsAny<Expression<Func<User, bool>>>(),
                                             It.IsAny<Expression<Func<User, object>>>()))
                .ReturnsAsync((User)null);

            var command = new LoginCommand("wrong", "wrong", ClientType.Web);

            var result = await _handler.Handle(command, CancellationToken.None);


            result.Should().NotBeNull(); 
            result.IsLoginFailed.Should().BeFalse();
            result.Message.Should().Be("Invalid username or password.");
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_When_UserInactive()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "test",
                Password = "123",
                IsActive = false,
                RoleId = (int)RoleEnum.SuperAdmin,
                Role = new Role { Name = "SuperAdmin" }
            };

            _userRepoMock
                .Setup(x => x.GetSingleAsync(It.IsAny<Expression<Func<User, bool>>>(),
                                             It.IsAny<Expression<Func<User, object>>>()))
                .ReturnsAsync(user);

            var command = new LoginCommand("test", "123", ClientType.Web);

            var result = await _handler.Handle(command, CancellationToken.None);

           
            result.Should().NotBeNull();
            result.IsLoginFailed.Should().BeFalse();
            result.Message.Should().Be("Your account is deactivated. Please contact the administrator.");
        }

        [Fact]
        public async Task Handle_Should_ReturnToken_When_LoginSuccessful()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "test",
                Password = "123",
                IsActive = true,
                RoleId = (int)RoleEnum.SuperAdmin,
                Role = new Role { Name = "SuperAdmin" }
            };

            _userRepoMock
                .Setup(x => x.GetSingleAsync(It.IsAny<Expression<Func<User, bool>>>(),
                                             It.IsAny<Expression<Func<User, object>>>()))
                .ReturnsAsync(user);

            _jwtServiceMock.Setup(x => x.GenerateJwtToken(user, ClientType.Web))
                           .Returns("jwt-token");

            _jwtServiceMock.Setup(x => x.GenerateRefreshToken())
                           .Returns("refresh-token");

            var command = new LoginCommand("test", "123", ClientType.Web);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsLoginFailed.Should().BeTrue();
            result.Token.Should().Be("jwt-token");
            result.RefreshToken.Should().Be("refresh-token");
            result.RoleName.Should().Be("SuperAdmin");
            result.UserName.Should().Be("test");
        }
    }


}
