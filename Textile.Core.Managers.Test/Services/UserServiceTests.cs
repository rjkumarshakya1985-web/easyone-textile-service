using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Enums;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Interfaces.Data;
using Textile.Core.Managers.Services;

namespace Textile.Core.Managers.Test.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IRepository<User, Guid>> _repoMock;
        private readonly TextileDbContext _context;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _repoMock = new Mock<IRepository<User, Guid>>();

            var options = new DbContextOptionsBuilder<TextileDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            _context = new TextileDbContext(options, httpContextAccessorMock.Object);

            _unitOfWorkMock
                .Setup(x => x.Repository<User, Guid>())
                .Returns(_repoMock.Object);

            _service = new UserService(_unitOfWorkMock.Object, _context);
        }


        [Fact]
        public async Task GetByIdAsync_Should_Return_UserResponse_When_User_Exists()
        {
            // Arrange
            var id = Guid.NewGuid();

            var user = new User
            {
                Id = id,
                UserName = "Raj",
                Email = "raj@test.com",
                Phone = "99999",
                IsActive = true,
                RoleId = (int)RoleEnum.SuperAdmin
            };

            _repoMock.Setup(x => x.GetByIdAsync(id))
                     .ReturnsAsync((User?)user);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            result.Should().NotBeNull();
            result!.UserName.Should().Be("Raj");
            result.Role.Should().Be(RoleEnum.SuperAdmin);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Null_When_Not_Found()
        {
            var id = Guid.NewGuid();

            _repoMock.Setup(x => x.GetByIdAsync(id))
                     .ReturnsAsync((User?)null);

            var result = await _service.GetByIdAsync(id);

            result.Should().BeNull();
        }

        [Fact]
        public async Task ToggleActiveAsync_Should_Toggle_Status()
        {
            var id = Guid.NewGuid();

            var user = new User
            {
                Id = id,
                IsActive = true
            };

            _repoMock.Setup(x => x.GetByIdAsync(id))
                     .ReturnsAsync(user);

            var result = await _service.ToggleActiveAsync(id);

            Assert.True(result);
            Assert.False(user.IsActive);

            _repoMock.Verify(x => x.UpdateAsync(user), Times.Once);
        }
    }
}
