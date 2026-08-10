using MediatR;
using Microsoft.Extensions.Configuration;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Enums;
using Textile.Core.Entities.Models.Response;
using Textile.Core.Interfaces.Data;
using Textile.Core.Interfaces.Services;
using Textile.Core.Managers.Commands;


namespace Textile.Core.Managers.Handlers
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _configuration;

        public LoginCommandHandler(IUnitOfWork unitOfWork, IJwtService jwtService, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {

            int[] allowedRoles;

            switch (request.ClientType)
            {
                case ClientType.Web:
                    allowedRoles = new[]
                    {
                        (int)RoleEnum.SuperAdmin,
                        (int)RoleEnum.Supplier,
                        (int)RoleEnum.StockIncharge
                    };
                    break;
                case ClientType.Mobile:
                    allowedRoles = new[]
                    {
                       (int)RoleEnum.SuperAdmin,
                       (int)RoleEnum.PackingSlipOperator
                    };
                    break;
                case ClientType.Windows:
                    allowedRoles = new[]
                    {
                       (int)RoleEnum.SuperAdmin,
                       (int)RoleEnum.Cashier,
                       (int)RoleEnum.PackingSlipOperator
                    };
                    break;
                default:
                    return new LoginResponse
                    {
                        IsLoginFailed = true,
                        Message = "Invalid client type."
                    };
            }


            ///

            var  _userRepository = _unitOfWork.Repository<User, Guid>();

            var _supplierRepository = _unitOfWork.Repository<Supplier, Guid>();

           
            

            var user = await _userRepository.GetSingleAsync(x =>
                               x.UserName == request.UserName &&
                               x.Password == request.Password &&
                               allowedRoles.Contains(x.RoleId),
                               x => x.Role);


            if (user == null)
            {
                return new LoginResponse
                {
                    IsLoginFailed = false,
                    Message = "Invalid username or password."
                };
            }

            if (!user.IsActive)
            {

                return new LoginResponse
                {
                    IsLoginFailed = false,
                    Message = "Your account is deactivated. Please contact the administrator."
                };
            }


            // Generate JWT
            var jwtToken = _jwtService.GenerateJwtToken(user,request.ClientType);
            var refreshToken = _jwtService.GenerateRefreshToken();

            var refreshDays = Convert.ToInt32(
            _configuration[$"JwtSettings:{request.ClientType}:RefreshTokenValidityInDays"]
            );

            var tokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),

                Token = refreshToken,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(refreshDays)
            };

            var _refreshtokenRepository = _unitOfWork.Repository<RefreshToken, Guid>();

            await _refreshtokenRepository.AddAsync(tokenEntity);

            string? displayName = user.UserName;
            var mustChangePassword = false;

            if (user.RoleId == (int)RoleEnum.Supplier)
            {
                var supplier = await _supplierRepository.GetSingleAsync(
                    x => x.UserId == user.Id
                );

                displayName = supplier?.Name ?? user.UserName;
                mustChangePassword = supplier != null && user.Password == supplier.Code;
            }


            return new LoginResponse
            {
                Token = jwtToken,
                RefreshToken = refreshToken,
                RoleName = user.Role.Name,
                UserName = user.UserName,
                Name   = displayName,
                IsDeveloper = user.IsDeveloper,
                MustChangePassword = mustChangePassword,
                IsLoginFailed = true 
            };
        }
    }

}
