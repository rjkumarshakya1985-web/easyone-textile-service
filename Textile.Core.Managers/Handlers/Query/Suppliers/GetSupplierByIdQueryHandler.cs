using AutoMapper;
using MediatR;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Models.Response.Suppliers;
using Textile.Core.Interfaces.Data;
using Textile.Core.Managers.Query.Suppliers;

namespace Textile.Core.Managers.Handlers.Query.Suppliers
{
    public class GetSupplierByIdQueryHandler : IRequestHandler<GetSupplierByIdQuery, SupplierDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetSupplierByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<SupplierDTO> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
        {
            var supplierRepository = _unitOfWork.Repository<Supplier, Guid>();

            var supplier = await supplierRepository.GetByIdAsync(request.SupplierId, s => s.City.State, c => c.SubDepartment.Department,
                u => u.User, a => a.Agent);

            if (supplier == null)
                return null;

            var supplierDto = _mapper.Map<SupplierDTO>(supplier);

           
            supplierDto.StateId = supplier.City?.StateId;
            supplierDto.DepartmentId = supplier.SubDepartment?.DepartmentId ?? 0;
            supplierDto.UserName = supplier.User.UserName;

            supplierDto.AgentObj = supplier.Agent == null ? null
             : new Entities.Models.Response.Agents.AgentTableResponse()
                 {
                   Id = supplier.Agent.Id,
                   Name = supplier.Agent.Name
                 };

            var transportRepo = _unitOfWork.Repository<SupplierTransport, Guid>();
            supplierDto.TransportIds = (await transportRepo.GetAllAsync(x => x.SupplierId == supplier.Id)).Select(x => x.TransportId).ToList();

            return supplierDto;
        }
    }

}
