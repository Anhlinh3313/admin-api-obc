using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Business.ViewModels;
using Core.Business.ViewModels.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace Core.Api.Controllers.Abstract
{
    public interface IGeneralController<TCreateViewModel, TUpdateViewModel, TInfoViewModel, TModel>
    {
        Task<JsonResult> Create([FromBody]TCreateViewModel viewModel);
        Task<JsonResult> Delete([FromBody]BasicViewModel viewModel);
        Task<JsonResult> Destroy([FromBody]BasicViewModel viewModel);
        JsonResult Get(int id, string cols = null);
        JsonResult GetAll(int? pageSize = null, int? pageNumber = null, string cols = null);
        Task<JsonResult> Update([FromBody]TUpdateViewModel viewModel);
    }

    public interface IGeneralController<TCreateUpdateViewModel, TInfoViewModel, TModel> : IGeneralController<TCreateUpdateViewModel, TCreateUpdateViewModel, TInfoViewModel, TModel>
    {
    }

    public interface IGeneralController<TCreateUpdateViewModel, TModel> : IGeneralController<TCreateUpdateViewModel, TCreateUpdateViewModel, TModel>
    {
    }
}
