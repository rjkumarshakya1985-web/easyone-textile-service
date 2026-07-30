using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.Data;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Dto;
using Textile.Core.Entities.Models;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Requests.Departments;
using Textile.Core.Entities.Models.Response.Departments;
using Textile.Core.Entities.Models.Response.Masters;
using Textile.Core.Interfaces.Services;
using Textile.Core.Managers.Commands.Departments;
using Textile.Core.Managers.Handlers.Query.Masters;
using Textile.Core.Managers.Query;
using Textile.Core.Managers.Query.Departments;
namespace EasyOneService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class MasterController : BaseController
    {


        private readonly ILogger<MasterController> _logger;
        private readonly IMediator _mediator;
        private readonly IHsnCodeService _hsnCodeService;
        private readonly IMasterDataService _masterDataService;
        private readonly IMapper _mapper;
        public MasterController(IUserContextService userContextService, ILogger<MasterController> logger, IMediator mediator,
            IHsnCodeService hsnCodeService, IMasterDataService masterDataService, IMapper mapper) : base(userContextService
               )
        {
            _logger = logger;
            _mapper = mapper;
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _hsnCodeService = hsnCodeService ?? throw new ArgumentNullException(nameof(hsnCodeService));
            _masterDataService = masterDataService ?? throw new ArgumentNullException(nameof(masterDataService));
        }

        #region GST

        [HttpGet("gsts")]
        public async Task<IEnumerable<Gst>> GetGsts()
        {
            return await _masterDataService.GetGsts();
        }
        #endregion

        #region State,City

        [HttpGet("states")]
        public async Task<IEnumerable<State>> GetStates()
        {
            var query = new GetAllStatesQuery();
            return await _mediator.Send(query);
        }


        [HttpGet("cities/{id}")]
        public async Task<IEnumerable<City>> GetCities(int id)
        {
            var query = new GetCitiesByStateIdQuery(id);
            return await _mediator.Send(query);
        }

        #endregion

        #region Look Up

        [HttpGet("transport-lookup")]
        public async Task<List<LookupDto<int>>> TransportLookUp([FromQuery] int? transportType = null)
        {
            return await _masterDataService.GetTransportLookUp(transportType);
        }

        [HttpGet("hsncode-lookup")]
        public async Task<List<LookupDto<Guid>>> HsnCodeLookUp()
        {
            return await _masterDataService.GetHsnCodeLookUp();
        }

        #endregion


        #region Department
        [HttpGet("departments")]
        public async Task<IEnumerable<DepartmentResponse>> GetDepartments()
        {
            var query = new GetDepartmentsQuery();
            return await _mediator.Send(query);
        }

        [HttpPost("savedepartment")]
        public async Task<bool> SaveDepartment(DepartmentRequest request)
        {
            var command = new SaveDepartmentCommand(request);
            return await _mediator.Send(command);
        }

        #endregion


        #region Sub Department
        [HttpGet("sub-departments/{departmentId}")]
        public async Task<IEnumerable<SubDepartmentResponse>> GetSubDepartments(int departmentId)
        {
            var query = new GetSubDepartmentsQuery(departmentId);
            return await _mediator.Send(query);
        }


        [HttpPost("savesubdepartment")]
        public async Task<bool> SaveSubDepartment(DepartmentRequest request)
        {
            var command = new SaveSubDepartmentCommand(request);
            return await _mediator.Send(command);
        }

        #endregion

        #region Stock Group 

        [HttpGet("create-stock-groups")]
        public async Task<IEnumerable<StockGroupResponse>> CreateStockGroups()
        {
            var query = new GetAllStockGroupsQuery();
            return await _mediator.Send(query);
        }

        [HttpGet("stock-groups")]
        public async Task<IEnumerable<StockGroupResponse>> GetStockGroups()
        {
            var query = new GetAllStockGroupsQuery();
            return await _mediator.Send(query);
        }
        #endregion


        #region
        [HttpPost("hsncode-table")]
        public async Task<TableResult<ProductHsnCode>> GetHsnCodeTable(TableDataRequest tableDataRequest)
        {

            return await _hsnCodeService.GetTableData(tableDataRequest);
        }


        [HttpPost("create-hsncode")]
        public async Task<bool> CreateHsnCode(HsnCodeRequest request)
        {

            try
            {
                return await _hsnCodeService.CreateAsync(request, CurrentUserId, CurrentUserName);
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is SqlException sqlEx &&
                    (sqlEx.Number == 2601 || sqlEx.Number == 2627))
                {
                    throw new Exception($"Hsn Code {request.Name} already exists.");
                }

                throw;
            }


        }

        [HttpDelete("delete-hsncode/{id}")]
        public async Task<bool> DeleteHsnCode(Guid id)
        {
            return await _hsnCodeService.DeleteAsync(id);

        }


        #endregion


        #region

        [HttpGet("mobile/states")]
        public async Task<IActionResult> GetMobileStates()
        {

            try
            {
                var query = new GetAllStatesQuery();
                var states = await _mediator.Send(query);

                var result = _mapper.Map<List<StateRespose>>(states);
                return ApiResponse(result, "Fetch State records");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating packing slip");
                return ApiError(ex.Message, 500);
            }
        }


        [HttpGet("mobile/cities/{id}")]
        public async Task<IActionResult> GetMobileCities(int id)
        {

            try
            {
                var query = new GetCitiesByStateIdQuery(id);
                var cities = await _mediator.Send(query);
                var result = _mapper.Map<List<StateRespose>>(cities);
                return ApiResponse(result, "City State records");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating packing slip");
                return ApiError(ex.Message, 500);
            }
        }



        #endregion
    }
}
