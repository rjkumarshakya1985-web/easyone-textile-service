using MediatR;
using Microsoft.Extensions.Configuration;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Models.Response;
using Textile.Core.Interfaces.Data;
using Textile.Core.Interfaces.Services;
using Textile.Core.Managers.Commands;

namespace Textile.Core.Managers.Handlers
{
    public class RefreshTokenCommandHandler
    : IRequestHandler<RefreshTokenCommand, LoginResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _configuration;

        public RefreshTokenCommandHandler(
            IUnitOfWork unitOfWork,
            IJwtService jwtService,
            IConfiguration configuration)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<LoginResponse> Handle(
            RefreshTokenCommand request,
            CancellationToken cancellationToken)
        {
            var refreshTokenRepo = _unitOfWork.Repository<RefreshToken, Guid>();
            var userRepo = _unitOfWork.Repository<User, Guid>();

            //  Get stored refresh token
            var storedToken = await refreshTokenRepo.GetSingleAsync(
                x => x.Token == request.RefreshToken,
                x => x.User.Role);

            if (storedToken == null || storedToken.ExpiresAt < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Invalid refresh token");

            //  Load user
            var user = await userRepo.GetByIdAsync(storedToken.UserId);
            if (user == null)
                throw new UnauthorizedAccessException("User not found");

            //  Generate new access token
            var newJwt = _jwtService.GenerateJwtToken(user, request.ClientType);

            //  Generate new refresh token
            var newRefresh = _jwtService.GenerateRefreshToken();

            //  Read refresh validity from JSON (client-wise)
            var refreshDays = Convert.ToInt32(
                _configuration[$"JwtSettings:{request.ClientType}:RefreshTokenValidityInDays"]
            );

            //  Revoke old token
            storedToken.RevokedAt = DateTime.UtcNow;
            storedToken.ReplacedByToken = newRefresh;
            await refreshTokenRepo.UpdateAsync(storedToken);

            //  Add new refresh token
            var newRefreshEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = newRefresh,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(refreshDays)
            };

            await refreshTokenRepo.AddAsync(newRefreshEntity);

            await _unitOfWork.SaveChangesAsync();

            return new LoginResponse
            {
                Token = newJwt,
                RefreshToken = newRefresh
            };
        }
    }

}
