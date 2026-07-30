using MediatR;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Interfaces.Data;
using Textile.Core.Interfaces.Services;
using Textile.Core.Managers.Commands;

namespace Textile.Core.Managers.Handlers
{

    public class ChangePasswordCommandHandler
     : IRequestHandler<ChangePasswordCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;

        public ChangePasswordCommandHandler(
            IUnitOfWork unitOfWork,
            IJwtService jwtService)
        {
            _unitOfWork = unitOfWork
                ?? throw new ArgumentNullException(nameof(unitOfWork));
            _jwtService = jwtService
                ?? throw new ArgumentNullException(nameof(jwtService));
        }

        public async Task<bool> Handle(
            ChangePasswordCommand request,
            CancellationToken cancellationToken)
        {
            var userRepository = _unitOfWork.Repository<User, Guid>();

            var user = await userRepository.GetSingleAsync(
                x => x.Id == request.UserId
            );

            
            if (user == null)
                throw new Exception("User not found");

           if(user.Password != request.changePasswordRequest.OldPassword)
                throw new Exception("The old password you entered is incorrect.");

          
           user.Password = request.changePasswordRequest.NewPassword;

           await userRepository.UpdateAsync(user);

           return true;
        }
    }

}
