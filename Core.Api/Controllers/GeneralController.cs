using System;
using Core.Api.Controllers.Abstract;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Data.Abstract;
using Core.Entity.Abstract;
using Core.Infrastructure.Helper;
using Core.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Core.Business.ViewModels;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Core.Api.Controllers
{
    public class GeneralController<TCreateViewModel, TUpdateViewModel, TInfoViewModel, TModel> : BaseController, IGeneralController<TCreateViewModel, TUpdateViewModel, TInfoViewModel, TModel>
        where TCreateViewModel : class, IEntityBase, new()
        where TUpdateViewModel : class, IEntityBase, new()
        where TModel : class, IEntityBase, new()
    {
        protected readonly IGeneralService<TCreateViewModel, TUpdateViewModel, TInfoViewModel, TModel> _iGeneralService;

        public GeneralController(
            Microsoft.Extensions.Logging.ILogger<dynamic> logger,
            IOptions<AppSettings> optionsAccessor,
            IOptions<JwtIssuerOptions> jwtOptions,
            IUnitOfWork unitOfWork,
            IGeneralService<TCreateViewModel, TUpdateViewModel, TInfoViewModel, TModel> iGeneralService) : base(logger, optionsAccessor, jwtOptions, unitOfWork)
        {
            _iGeneralService = iGeneralService;
        }

        [HttpPost("Create")]
        public virtual async Task<JsonResult> Create([FromBody] TCreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return JsonUtil.Error(ModelState);
            }

            return JsonUtil.Create(await _iGeneralService.Create(viewModel));
        }

        [HttpPost("Delete")]
        public virtual async System.Threading.Tasks.Task<JsonResult> Delete([FromBody] BasicViewModel viewModel)
        {
            return JsonUtil.Create(await _iGeneralService.Delete(viewModel));
        }

        [HttpPost("Destroy")]
        public virtual async System.Threading.Tasks.Task<JsonResult> Destroy([FromBody] BasicViewModel viewModel)
        {
            return JsonUtil.Create(await _iGeneralService.Destroy(viewModel));
        }

        [HttpGet("Get")]
        public virtual JsonResult Get(int id, string cols = null)
        {
            string[] includeProperties = new string[0];

            if (!string.IsNullOrWhiteSpace(cols))
            {
                includeProperties = cols.Split(',');
            }

            return JsonUtil.Create(_iGeneralService.Get(id, includeProperties));
        }

        [HttpGet("GetAll")]
        public virtual JsonResult GetAll(int? pageSize = null, int? pageNumber = null, string cols = null)
        {
            string[] includeProperties = new string[0];

            if (!string.IsNullOrWhiteSpace(cols))
            {
                includeProperties = cols.Split(',');
            }

            return JsonUtil.Create(_iGeneralService.GetAll(pageSize, pageNumber, includeProperties));
        }

        [HttpPost("Update")]
        public virtual async Task<JsonResult> Update([FromBody] TUpdateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return JsonUtil.Error(ModelState);
            }

            return JsonUtil.Create(await _iGeneralService.Update(viewModel));
        }

        protected JsonResult FindBy(Expression<Func<TModel, bool>> predicate)
        {
            return JsonUtil.Create(_iGeneralService.FindBy(predicate));
        }

        protected JsonResult FindBy(Expression<Func<TModel, bool>> predicate, int? pageSize = default(int?), int? pageNumber = default(int?), string cols = null)
        {
            string[] includeProperties = new string[0];

            if (!string.IsNullOrWhiteSpace(cols))
            {
                includeProperties = cols.Split(',');
            }

            return JsonUtil.Create(_iGeneralService.FindBy(predicate, pageSize, pageNumber, includeProperties));
        }
    }

    public class GeneralController<TCreateUpdateViewModel, TInfoViewModel, TModel> : GeneralController<TCreateUpdateViewModel, TCreateUpdateViewModel, TInfoViewModel, TModel>, IGeneralController<TCreateUpdateViewModel, TInfoViewModel, TModel>
        where TCreateUpdateViewModel : class, IEntityBase, new()
        where TModel : class, IEntityBase, new()
    {
        public GeneralController(
            Microsoft.Extensions.Logging.ILogger<dynamic> logger,
            IOptions<AppSettings> optionsAccessor,
            IOptions<JwtIssuerOptions> jwtOptions,
            IUnitOfWork unitOfWork,
            IGeneralService<TCreateUpdateViewModel, TInfoViewModel, TModel> iGeneralService)
            : base(logger, optionsAccessor, jwtOptions, unitOfWork, iGeneralService)
        {
        }
    }

    public class GeneralController<TViewModel, TModel> : GeneralController<TViewModel, TViewModel, TModel>, IGeneralController<TViewModel, TModel>
        where TViewModel : class, IEntityBase, new()
        where TModel : class, IEntityBase, new()
    {
        public GeneralController(
            Microsoft.Extensions.Logging.ILogger<dynamic> logger,
            IOptions<AppSettings> optionsAccessor,
            IOptions<JwtIssuerOptions> jwtOptions,
            IUnitOfWork unitOfWork,
            IGeneralService<TViewModel, TModel> iGeneralService)
            : base(logger, optionsAccessor, jwtOptions, unitOfWork, iGeneralService)
        {
        }
    }
}
