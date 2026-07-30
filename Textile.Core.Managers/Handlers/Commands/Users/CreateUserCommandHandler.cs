using MediatR;
using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Enums;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Interfaces.Data;
using Textile.Core.Managers.Commands.Users;

namespace Textile.Core.Managers.Handlers.Commands.Users
{

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TextileDbContext _context;

        public CreateUserCommandHandler(IUnitOfWork unitOfWork, TextileDbContext context)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork)); ;
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Guid> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            var request = command.UserRequest;
            var userRepository = _unitOfWork.Repository<User, Guid>();

            // CREATE
            if (request.Id == Guid.Empty || request.Id==null)
            {
                bool userExists = await _context.Users
                    .AnyAsync(x => x.UserName == request.UserName, cancellationToken);

                if (userExists)
                    throw new InvalidOperationException(
                        "Username already exists. Please choose another username."
                    );

                var newUser = new User
                {
                    Id = Guid.NewGuid(),
                    RoleId = request.RoleId,
                    UserName = request.UserName,
                    Password = request.Password, // TODO: hash before saving
                    Email = request.Email,
                    Phone = request.Phone,
                    IsActive = request.IsActive,
                    CreatedBy = request.CreatedBy
                        ?? throw new InvalidOperationException("CreatedBy is required"),
                    CreatedByUserName = request.CreatedByUserName,
                    CreatedOn = DateTime.UtcNow
                };

                await userRepository.AddAsync(newUser);

                if(newUser.RoleId == (int)RoleEnum.PackingSlipOperator)
                {
                    var userDetail = new UserDetail
                    {
                        Id = Guid.NewGuid(),
                        UserId = newUser.Id,
                        DepartmentId = request.DepartmentId

                    };
                    await _unitOfWork.Repository<UserDetail, Guid>().AddAsync(userDetail);
                }
                return newUser.Id;
            }

            // UPDATE
            var existingUser = await userRepository
                .GetSingleAsync(x => x.Id == request.Id,x=>x.UserDetail);

            if (existingUser == null)
                throw new KeyNotFoundException("User not found");

            bool duplicateUserName = await _context.Users.AnyAsync(
                x => x.UserName == request.UserName && x.Id != request.Id,
                cancellationToken);

            if (duplicateUserName)
                throw new InvalidOperationException(
                    "Username already exists. Please choose another username."
                );

            existingUser.RoleId = request.RoleId;
            existingUser.UserName = request.UserName;
            existingUser.Password = request.Password;
            existingUser.Email = request.Email;
            existingUser.Phone = request.Phone;
            existingUser.IsActive = request.IsActive;
            existingUser.ModifiedBy = request.CreatedBy;
            existingUser.ModifiedByUserName = request.CreatedByUserName;
           
            existingUser.ModifiedOn = DateTime.UtcNow;

            if(request.RoleId == (int)RoleEnum.PackingSlipOperator)
            {
                if(existingUser.UserDetail == null)
                {
                    var userDetail = new UserDetail
                    {
                        Id = Guid.NewGuid(),
                        UserId = existingUser.Id,
                        DepartmentId = request.DepartmentId
                    };
                    await _unitOfWork.Repository<UserDetail, Guid>().AddAsync(userDetail);
                }
                else
                {
                    existingUser.UserDetail.DepartmentId = request.DepartmentId;
                   await _unitOfWork.Repository<UserDetail, Guid>().UpdateAsync(existingUser.UserDetail);
                }
            }

            await userRepository.UpdateAsync(existingUser);
            return existingUser.Id;
        }

    }

}
