using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Business.ViewModels;
using Core.Entity.Abstract;
using Core.Infrastructure.ViewModels;

namespace Core.Business.Services.Abstract
{
    public interface IGeneralService<TCreateViewModel, TUpdateViewModel, TInfoViewModel, TModel>
    {
        Task<ResponseViewModel> Create(TCreateViewModel viewModel);
        Task<ResponseViewModel> Create(List<TCreateViewModel> listViewModel);
        Task<ResponseViewModel> Create<TCustomViewModel>(TCustomViewModel viewModel);
        Task<ResponseViewModel> Create<TCustomViewModel>(List<TCustomViewModel> listViewModel);
        Task<ResponseViewModel> Delete(BasicViewModel viewModel);
        Task<ResponseViewModel> Destroy(BasicViewModel viewModel);
        ResponseViewModel FindBy(Expression<Func<TModel, bool>> predicate);
        ResponseViewModel FindBy(Expression<Func<TModel, bool>> predicate, int? pageSize = null, int? pageNumber = null, string cols = null);
        ResponseViewModel FindBy(Expression<Func<TModel, bool>> predicate, int? pageSize = null, int? pageNumber = null, params string[] includeProperties);
        ResponseViewModel Get(int id);
        ResponseViewModel Get(int id, params string[] includeProperties);
        ResponseViewModel Get(int id, params Expression<Func<TModel, object>>[] includeProperties);
        ResponseViewModel GetAll(int? pageSize = null, int? pageNumber = null);
        ResponseViewModel GetAll(int? pageSize = null, int? pageNumber = null, params string[] includeProperties);
        ResponseViewModel GetAll(int? pageSize = null, int? pageNumber = null, params Expression<Func<TModel, object>>[] includeProperties);
        Task<ResponseViewModel> Update(TUpdateViewModel viewModel);
        Task<ResponseViewModel> Update(List<TUpdateViewModel> listViewModel);
        Task<ResponseViewModel> Update<TCutomViewModel>(TCutomViewModel viewModel) where TCutomViewModel : class, IEntityBase;
        Task<ResponseViewModel> Update<TCutomViewModel>(List<TCutomViewModel> listViewModel) where TCutomViewModel : class, IEntityBase;
    }

    public interface IGeneralService<TViewModel, TInfoViewModel, TModel> : IGeneralService<TViewModel, TViewModel, TInfoViewModel, TModel>
    {
    }

    public interface IGeneralService<TViewModel, TModel> : IGeneralService<TViewModel, TViewModel, TModel>
    {
        ResponseViewModel FindBy<TInfoViewModel>(Expression<Func<TModel, bool>> predicate);
        ResponseViewModel FindBy<TInfoViewModel>(Expression<Func<TModel, bool>> predicate, int? pageSize = null, int? pageNumber = null, params string[] includeProperties);
        ResponseViewModel Get<TInfoViewModel>(int id, params Expression<Func<TModel, object>>[] includeProperties);
        ResponseViewModel GetAll<TInfoViewModel>(int? pageSize = null, int? pageNumber = null);
        ResponseViewModel GetAll<TInfoViewModel>(int? pageSize = null, int? pageNumber = null, params string[] includeProperties);
        ResponseViewModel GetAll<TInfoViewModel>(int? pageSize = null, int? pageNumber = null, params Expression<Func<TModel, object>>[] includeProperties);
    }

    public interface IGeneralService<TModel> : IGeneralService<TModel, TModel>
    {
    }

    public interface IGeneralService
    {
        Task<ResponseViewModel> Create<TModel>(TModel viewModel) where TModel : class, IEntityBase, new();
        Task<ResponseViewModel> Create<TModel, TCreateViewModel>(TCreateViewModel viewModel) where TModel : class, IEntityBase, new() where TCreateViewModel : class, IEntityBase, new();
        Task<ResponseViewModel> Create<TModel, TInfoViewModel, TCreateViewModel>(TCreateViewModel viewModel) where TModel : class, IEntityBase, new() where TCreateViewModel : class, IEntityBase, new();
        Task<ResponseViewModel> Create<TModel>(List<TModel> listViewModel) where TModel : class, IEntityBase, new();
        Task<ResponseViewModel> Create<TModel, TCreateViewModel>(List<TCreateViewModel> listViewModel) where TModel : class, IEntityBase, new() where TCreateViewModel : class, IEntityBase, new();
        Task<ResponseViewModel> Create<TModel, TInfoViewModel, TCreateViewModel>(List<TCreateViewModel> listViewModel) where TModel : class, IEntityBase, new() where TCreateViewModel : class, IEntityBase, new();
        Task<ResponseViewModel> Update<TModel>(TModel viewModel) where TModel : class, IEntityBase, new();
        Task<ResponseViewModel> Update<TModel, TUpdateViewModel>(TUpdateViewModel viewModel) where TModel : class, IEntityBase, new() where TUpdateViewModel : class, IEntityBase, new();
        Task<ResponseViewModel> Update<TModel, TInfoViewModel, TUpdateViewModel>(TUpdateViewModel viewModel) where TModel : class, IEntityBase, new() where TUpdateViewModel : class, IEntityBase, new();
        Task<ResponseViewModel> Update<TModel>(List<TModel> listViewModel) where TModel : class, IEntityBase, new();
        Task<ResponseViewModel> Update<TModel, TUpdateViewModel>(List<TUpdateViewModel> listViewModel) where TModel : class, IEntityBase, new() where TUpdateViewModel : class, IEntityBase, new();
        Task<ResponseViewModel> Update<TModel, TInfoViewModel, TUpdateViewModel>(List<TUpdateViewModel> listViewModel) where TModel : class, IEntityBase, new() where TUpdateViewModel : class, IEntityBase, new();
        ResponseViewModel FindBy<TModel>(Expression<Func<TModel, bool>> predicate, int? pageSize = null, int? pageNumber = null, string cols = null) where TModel : class, IEntityBase, new();
        ResponseViewModel FindBy<TModel>(Expression<Func<TModel, bool>> predicate, int? pageSize = null, int? pageNumber = null, params string[] includeProperties) where TModel : class, IEntityBase, new();
        ResponseViewModel FindBy<TModel, TInfoViewModel>(Expression<Func<TModel, bool>> predicate, int? pageSize = null, int? pageNumber = null, string cols = null) where TModel : class, IEntityBase, new();
        ResponseViewModel FindBy<TModel, TInfoViewModel>(Expression<Func<TModel, bool>> predicate, int? pageSize = null, int? pageNumber = null, params string[] includeProperties) where TModel : class, IEntityBase, new();
        ResponseViewModel GetSingle<TModel>(Expression<Func<TModel, bool>> predicate, string cols) where TModel : class, IEntityBase, new();
        ResponseViewModel GetSingle<TModel>(Expression<Func<TModel, bool>> predicate, params string[] includeProperties) where TModel : class, IEntityBase, new();
        ResponseViewModel GetSingle<TModel>(Expression<Func<TModel, bool>> predicate, params Expression<Func<TModel, object>>[] includeProperties) where TModel : class, IEntityBase, new();
        ResponseViewModel GetSingle<TModel, TInfoViewModel>(Expression<Func<TModel, bool>> predicate, string cols) where TModel : class, IEntityBase, new();
        ResponseViewModel GetSingle<TModel, TInfoViewModel>(Expression<Func<TModel, bool>> predicate, params string[] includeProperties) where TModel : class, IEntityBase, new();
        ResponseViewModel GetSingle<TModel, TInfoViewModel>(Expression<Func<TModel, bool>> predicate, params Expression<Func<TModel, object>>[] includeProperties) where TModel : class, IEntityBase, new();
    }
}
