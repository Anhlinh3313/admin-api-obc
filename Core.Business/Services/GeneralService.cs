using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels;
using Core.Data.Abstract;
using Core.Entity.Abstract;
using Core.Infrastructure.ViewModels;
using Microsoft.Extensions.Options;

namespace Core.Business.Services
{
    public class GeneralService<TCreateViewModel, TUpdateViewModel, TInfoViewModel, TModel> : BaseService, IGeneralService<TCreateViewModel, TUpdateViewModel, TInfoViewModel, TModel>
        where TCreateViewModel : class, IEntityBase, new()
        where TUpdateViewModel : class, IEntityBase, new()
        where TModel : class, IEntityBasic, new()
    {
        public GeneralService(Microsoft.Extensions.Logging.ILogger<dynamic> logger, IOptions<AppSettings> optionsAccessor, IUnitOfWork unitOfWork) : base(logger, optionsAccessor, unitOfWork)
        {
        }

        public virtual async Task<ResponseViewModel> Create(TCreateViewModel viewModel)
        {
            TModel model = Mapper.Map<TModel>(viewModel);
            _unitOfWork.RepositoryCRUD<TModel>().Insert(model);
            await _unitOfWork.CommitAsync();
            return ResponseViewModel.CreateSuccess(Mapper.Map<TInfoViewModel>(model));
        }

        public virtual async Task<ResponseViewModel> Create(List<TCreateViewModel> listViewModel)
        {
            var listInfo = new List<TInfoViewModel>();
            var listModel = new List<TModel>();

            foreach (var item in listViewModel)
            {
                var model = Mapper.Map<TModel>(item);
                _unitOfWork.RepositoryCRUD<TModel>().Insert(model);
                listModel.Add(model);
            }

            await _unitOfWork.CommitAsync();

            foreach (var item in listModel)
            {
                listInfo.Add(Mapper.Map<TInfoViewModel>(item));
            }
            return ResponseViewModel.CreateSuccess(listModel);
        }

        public virtual async Task<ResponseViewModel> Create<TCustomViewModel>(TCustomViewModel viewModel)
        {
            TModel model = Mapper.Map<TModel>(viewModel);
            _unitOfWork.RepositoryCRUD<TModel>().Insert(model);
            await _unitOfWork.CommitAsync();
            return ResponseViewModel.CreateSuccess(Mapper.Map<TInfoViewModel>(model));
        }

        public virtual async Task<ResponseViewModel> Create<TCustomViewModel>(List<TCustomViewModel> listViewModel)
        {
            var listInfo = new List<TInfoViewModel>();
            var listModel = new List<TModel>();

            foreach (var item in listViewModel)
            {
                var model = Mapper.Map<TModel>(item);
                _unitOfWork.RepositoryCRUD<TModel>().Insert(model);
                listInfo.Add(Mapper.Map<TInfoViewModel>(model));
                listModel.Add(model);
            }

            await _unitOfWork.CommitAsync();
            return ResponseViewModel.CreateSuccess(listModel);
        }

        public virtual async Task<ResponseViewModel> Delete(BasicViewModel viewModel)
        {
            var entity = _unitOfWork.RepositoryCRUD<TModel>().GetSingle(viewModel.Id);
            if (entity != null)
            {
                try
                {
                    entity.IsEnabled = false;
                    _unitOfWork.RepositoryCRUD<TModel>().Update(entity);
                    //_unitOfWork.RepositoryCRUD<TModel>().Delete(viewModel.Id);
                    await _unitOfWork.CommitAsync();
                    return ResponseViewModel.CreateSuccess(viewModel);
                }
                catch (SqlException sqlEx)
                {
                    if (sqlEx.Number == 547)
                    {
                        entity.IsEnabled = false;
                        return await Update(entity);
                    }
                    return ResponseViewModel.CreateError(sqlEx.Message);
                }
                catch (Exception ex)
                {
                    if (ex.InnerException is SqlException)
                    {
                        var sqlEx = ex.InnerException as SqlException;

                        if (sqlEx.Number == 547)
                        {
                            entity.IsEnabled = false;
                            await Update(entity);
                            return ResponseViewModel.CreateError("Dữ liệu đã được sử dụng, không thể xoá");
                        }
                        return ResponseViewModel.CreateError(sqlEx.Message);
                    }
                    return ResponseViewModel.CreateError(ex.Message);
                }
            }
            return ResponseViewModel.CreateError(ValidatorMessage.General.NotExist);
        }

        public virtual async Task<ResponseViewModel> Destroy(BasicViewModel viewModel)
        {
            var entity = _unitOfWork.RepositoryCRUD<TModel>().GetSingle(viewModel.Id);

            if (entity != null)
            {
                try
                {
                    _unitOfWork.RepositoryCRUD<TModel>().Delete(viewModel.Id);
                    await _unitOfWork.CommitAsync();
                    return ResponseViewModel.CreateSuccess(Mapper.Map<TInfoViewModel>(entity));
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 547) return ResponseViewModel.CreateError(ValidatorMessage.General.NotDestroy); // The {...} statement conflicted with the {...} constraint {...}
                    return ResponseViewModel.CreateError(ex.Message);
                }
                catch (Exception ex)
                {
                    if (ex.InnerException is SqlException)
                    {
                        var sqlEx = ex.InnerException as SqlException;

                        if (sqlEx.Number == 547)
                        {
                            entity.IsEnabled = false;
                            await Update(entity);
                            return ResponseViewModel.CreateError("Dữ liệu đã ");
                        }
                        return ResponseViewModel.CreateError(sqlEx.Message);
                    }
                    return ResponseViewModel.CreateError(ex.Message);
                }
            }

            return ResponseViewModel.CreateError(ValidatorMessage.General.NotExist);
        }

        public virtual ResponseViewModel Get(int id)
        {
            var entity = _unitOfWork.RepositoryR<TModel>().GetSingle(id);

            if (entity != null)
                return ResponseViewModel.CreateSuccess(Mapper.Map<TInfoViewModel>(entity));

            return ResponseViewModel.CreateSuccess();
        }

        public virtual ResponseViewModel Get(int id, params string[] includeProperties)
        {
            var entity = _unitOfWork.RepositoryR<TModel>().GetSingle(id, includeProperties);

            if (entity != null)
                return ResponseViewModel.CreateSuccess(Mapper.Map<TInfoViewModel>(entity));

            return ResponseViewModel.CreateSuccess();
        }

        public virtual ResponseViewModel Get(int id, params Expression<Func<TModel, object>>[] includeProperties)
        {
            var entity = _unitOfWork.RepositoryR<TModel>().GetSingle(id, includeProperties);

            if (entity != null)
                return ResponseViewModel.CreateSuccess(Mapper.Map<TInfoViewModel>(entity));

            return ResponseViewModel.CreateSuccess();
        }

        public virtual ResponseViewModel GetAll(int? pageSize = null, int? pageNumber = null)
        {
            return GetAll(pageSize, pageNumber, new string[0]);
        }

        public virtual ResponseViewModel GetAll(int? pageSize = null, int? pageNumber = null, params string[] includeProperties)
        {
            if (pageSize.HasValue && pageNumber.HasValue)
            {
                int iPageNumber = (int)pageNumber;
                int iPageSize = (int)pageSize;

                var totalCount = _unitOfWork.RepositoryR<TModel>().Count();
                var totalPages = Math.Ceiling((double)totalCount / iPageSize);

                var data = _unitOfWork.RepositoryR<TModel>()
                                      .GetAll(includeProperties)
                                      .Skip((iPageNumber - 1) * iPageSize)
                                      .Take(iPageSize);

                return ResponseViewModel.CreateSuccess(Mapper.Map<IEnumerable<TInfoViewModel>>(data), dataCount: totalCount);
            }

            return ResponseViewModel.CreateSuccess(Mapper.Map<IEnumerable<TInfoViewModel>>(_unitOfWork.RepositoryR<TModel>().GetAll(includeProperties)));
        }

        public virtual ResponseViewModel GetAll(int? pageSize = null, int? pageNumber = null, params Expression<Func<TModel, object>>[] includeProperties)
        {
            if (pageSize.HasValue && pageNumber.HasValue)
            {
                int iPageNumber = (int)pageNumber;
                int iPageSize = (int)pageSize;

                var totalCount = _unitOfWork.RepositoryR<TModel>().Count();
                var totalPages = Math.Ceiling((double)totalCount / iPageSize);

                var data = _unitOfWork.RepositoryR<TModel>()
                                      .GetAll(includeProperties)
                                      .Skip((iPageNumber - 1) * iPageSize)
                                      .Take(iPageSize);

                return ResponseViewModel.CreateSuccess(Mapper.Map<IEnumerable<TInfoViewModel>>(data), dataCount: totalCount);
            }

            return ResponseViewModel.CreateSuccess(Mapper.Map<IEnumerable<TInfoViewModel>>(_unitOfWork.RepositoryR<TModel>().GetAll(includeProperties)));
        }

        public virtual ResponseViewModel FindBy(Expression<Func<TModel, bool>> predicate)
        {
            return ResponseViewModel.CreateSuccess(Mapper.Map<IEnumerable<TInfoViewModel>>(_unitOfWork.RepositoryR<TModel>().FindBy(predicate)));
        }

        public virtual ResponseViewModel FindBy(Expression<Func<TModel, bool>> predicate, int? pageSize = null, int? pageNumber = null, string cols = null)
        {
            string[] includeProperties = new string[0];

            if (!string.IsNullOrWhiteSpace(cols))
            {
                includeProperties = cols.Split(',');
            }

            return FindBy(predicate, pageSize, pageNumber, includeProperties);
        }

        public virtual ResponseViewModel FindBy(Expression<Func<TModel, bool>> predicate, int? pageSize = null, int? pageNumber = null, params string[] includeProperties)
        {
            if (pageSize.HasValue && pageNumber.HasValue)
            {
                int iPageNumber = (int)pageNumber;
                int iPageSize = (int)pageSize;
                var dataAll = _unitOfWork.RepositoryR<TModel>()
                                      .GetAll(includeProperties)
                                      .Where(predicate);
                var data = dataAll.Skip((iPageNumber - 1) * iPageSize).Take(iPageSize);
                var totalCount = dataAll.Count();
                var totalPages = Math.Ceiling((double)totalCount / iPageSize);
                Mapper.Map<IEnumerable<TInfoViewModel>>(data);

                return ResponseViewModel.CreateSuccess(Mapper.Map<IEnumerable<TInfoViewModel>>(data), dataCount: totalCount);
            }

            return ResponseViewModel.CreateSuccess(Mapper.Map<IEnumerable<TInfoViewModel>>(_unitOfWork.RepositoryR<TModel>().GetAll(includeProperties).Where(predicate)));
        }

        private async Task<ResponseViewModel> Update(TModel viewModel)
        {
            TModel model = Mapper.Map<TModel>(viewModel);
            _unitOfWork.RepositoryCRUD<TModel>().Update(model);
            await _unitOfWork.CommitAsync();
            return ResponseViewModel.CreateSuccess(Mapper.Map<TInfoViewModel>(model));
        }

        public virtual async Task<ResponseViewModel> Update(TUpdateViewModel viewModel)
        {
            TModel model = _unitOfWork.RepositoryR<TModel>().GetSingle(viewModel.Id);
            model = Mapper.Map(viewModel, model);
            _unitOfWork.RepositoryCRUD<TModel>().Update(model);
            await _unitOfWork.CommitAsync();
            return ResponseViewModel.CreateSuccess(Mapper.Map<TInfoViewModel>(model));
        }

        public virtual async Task<ResponseViewModel> Update(List<TUpdateViewModel> listViewModel)
        {
            int[] ids = listViewModel.Select(x => x.Id).ToArray();

            var listInfo = new List<TInfoViewModel>();
            var listModel = _unitOfWork.RepositoryR<TModel>().FindBy(x => ids.Contains(x.Id)).ToList<TModel>();

            foreach (var item in listViewModel)
            {
                var model = listModel.SingleOrDefault<TModel>(x => x.Id == item.Id);

                if (model != null)
                {
                    model = Mapper.Map(item, model);
                    _unitOfWork.RepositoryCRUD<TModel>().Update(model);
                }
            }

            await _unitOfWork.CommitAsync();

            foreach (var item in listModel)
            {
                listInfo.Add(Mapper.Map<TInfoViewModel>(item));
            }

            return ResponseViewModel.CreateSuccess(listInfo);
        }

        public virtual async Task<ResponseViewModel> Update<TCutomViewModel>(TCutomViewModel viewModel) where TCutomViewModel : class, IEntityBase
        {
            TModel model = _unitOfWork.RepositoryR<TModel>().GetSingle(viewModel.Id);
            model = Mapper.Map(viewModel, model);
            _unitOfWork.RepositoryCRUD<TModel>().Update(model);
            await _unitOfWork.CommitAsync();
            return ResponseViewModel.CreateSuccess(Mapper.Map<TInfoViewModel>(model));
        }

        public virtual async Task<ResponseViewModel> Update<TCutomViewModel>(List<TCutomViewModel> listViewModel) where TCutomViewModel : class, IEntityBase
        {
            int[] ids = listViewModel.Select(x => x.Id).ToArray();

            var listInfo = new List<TInfoViewModel>();
            var listModel = _unitOfWork.RepositoryR<TModel>().FindBy(x => ids.Contains(x.Id)).ToList<TModel>();

            foreach (var item in listViewModel)
            {
                var model = listModel.SingleOrDefault<TModel>(x => x.Id == item.Id);

                if (model != null)
                {
                    model = Mapper.Map(item, model);
                    _unitOfWork.RepositoryCRUD<TModel>().Update(model);
                }
            }
            await _unitOfWork.CommitAsync();

            foreach (var item in listModel)
            {
                listInfo.Add(Mapper.Map<TInfoViewModel>(item));
            }

            return ResponseViewModel.CreateSuccess(listInfo);
        }
    }

    public class GeneralService<TCreateUpdateViewModel, TInfoViewModel, TModel> : GeneralService<TCreateUpdateViewModel, TCreateUpdateViewModel, TInfoViewModel, TModel>, IGeneralService<TCreateUpdateViewModel, TInfoViewModel, TModel>
        where TCreateUpdateViewModel : class, IEntityBase, new()
        where TModel : class, IEntityBasic, new()
    {
        public GeneralService(Microsoft.Extensions.Logging.ILogger<dynamic> logger, IOptions<AppSettings> optionsAccessor, IUnitOfWork unitOfWork) : base(logger, optionsAccessor, unitOfWork)
        {
        }
    }

    public class GeneralService<TViewModel, TModel> : GeneralService<TViewModel, TModel, TModel>, IGeneralService<TViewModel, TModel>
        where TViewModel : class, IEntityBase, new()
        where TModel : class, IEntityBasic, new()
    {
        public GeneralService(Microsoft.Extensions.Logging.ILogger<dynamic> logger, IOptions<AppSettings> optionsAccessor, IUnitOfWork unitOfWork) : base(logger, optionsAccessor, unitOfWork)
        {
        }

        public virtual ResponseViewModel Get<TInfoViewModel>(int id, params Expression<Func<TModel, object>>[] includeProperties)
        {
            var entity = _unitOfWork.RepositoryR<TModel>().GetSingle(id, includeProperties);

            if (entity != null)
                return ResponseViewModel.CreateSuccess(Mapper.Map<TInfoViewModel>(entity));

            return ResponseViewModel.CreateSuccess();
        }

        public virtual ResponseViewModel GetAll<TInfoViewModel>(int? pageSize = null, int? pageNumber = null)
        {
            return GetAll<TInfoViewModel>(pageSize, pageNumber, new string[0]);
        }

        public virtual ResponseViewModel GetAll<TInfoViewModel>(int? pageSize = null, int? pageNumber = null, params string[] includeProperties)
        {
            if (pageSize.HasValue && pageNumber.HasValue)
            {
                int iPageNumber = (int)pageNumber;
                int iPageSize = (int)pageSize;

                var totalCount = _unitOfWork.RepositoryR<TModel>().Count();
                var totalPages = Math.Ceiling((double)totalCount / iPageSize);

                var data = _unitOfWork.RepositoryR<TModel>()
                                      .GetAll(includeProperties)
                                      .Skip((iPageNumber - 1) * iPageSize)
                                      .Take(iPageSize);

                return ResponseViewModel.CreateSuccess(Mapper.Map<IEnumerable<TInfoViewModel>>(data), dataCount: totalCount);
            }

            return ResponseViewModel.CreateSuccess(Mapper.Map<IEnumerable<TInfoViewModel>>(_unitOfWork.RepositoryR<TModel>().GetAll(includeProperties)));
        }

        public virtual ResponseViewModel GetAll<TInfoViewModel>(int? pageSize = null, int? pageNumber = null, params Expression<Func<TModel, object>>[] includeProperties)
        {
            if (pageSize.HasValue && pageNumber.HasValue)
            {
                int iPageNumber = (int)pageNumber;
                int iPageSize = (int)pageSize;

                var totalCount = _unitOfWork.RepositoryR<TModel>().Count();
                var totalPages = Math.Ceiling((double)totalCount / iPageSize);

                var data = _unitOfWork.RepositoryR<TModel>()
                                      .GetAll(includeProperties)
                                      .Skip((iPageNumber - 1) * iPageSize)
                                      .Take(iPageSize);

                return ResponseViewModel.CreateSuccess(Mapper.Map<IEnumerable<TInfoViewModel>>(data), dataCount: totalCount);
            }

            return ResponseViewModel.CreateSuccess(Mapper.Map<IEnumerable<TInfoViewModel>>(_unitOfWork.RepositoryR<TModel>().GetAll(includeProperties)));
        }

        public virtual ResponseViewModel FindBy<TInfoViewModel>(Expression<Func<TModel, bool>> predicate)
        {
            return ResponseViewModel.CreateSuccess(Mapper.Map<IEnumerable<TInfoViewModel>>(_unitOfWork.RepositoryR<TModel>().FindBy(predicate)));
        }

        public virtual ResponseViewModel FindBy<TInfoViewModel>(Expression<Func<TModel, bool>> predicate, int? pageSize = null, int? pageNumber = null, params string[] includeProperties)
        {
            if (pageSize.HasValue && pageNumber.HasValue)
            {
                int iPageNumber = (int)pageNumber;
                int iPageSize = (int)pageSize;

                var totalCount = _unitOfWork.RepositoryR<TModel>().Count();
                var totalPages = Math.Ceiling((double)totalCount / iPageSize);

                var data = _unitOfWork.RepositoryR<TModel>()
                                      .GetAll(includeProperties)
                                      .Where(predicate)
                                      .Skip((iPageNumber - 1) * iPageSize)
                                      .Take(iPageSize);

                return ResponseViewModel.CreateSuccess(Mapper.Map<IEnumerable<TInfoViewModel>>(data), dataCount: totalCount);
            }

            return ResponseViewModel.CreateSuccess(Mapper.Map<IEnumerable<TInfoViewModel>>(_unitOfWork.RepositoryR<TModel>().GetAll(includeProperties).Where(predicate)));
        }
    }

    public class GeneralService<TModel> : GeneralService<TModel, TModel>, IGeneralService<TModel>
        where TModel : class, IEntityBasic, new()
    {
        public GeneralService(Microsoft.Extensions.Logging.ILogger<dynamic> logger, IOptions<AppSettings> optionsAccessor, IUnitOfWork unitOfWork) : base(logger, optionsAccessor, unitOfWork)
        {

        }
    }

    public class GeneralService : IGeneralService
    {
        private readonly IUnitOfWork _unitOfWork;

        public GeneralService(
            Microsoft.Extensions.Logging.ILogger<dynamic> logger,
            IOptions<AppSettings> optionsAccessor,
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public virtual Task<ResponseViewModel> Create<TModel>(List<TModel> listViewModel)
            where TModel : class, IEntityBase, new()
        {
            return Create<TModel, TModel, TModel>(listViewModel);
        }

        public virtual Task<ResponseViewModel> Create<TModel, TCreateViewModel>(List<TCreateViewModel> listViewModel)
            where TModel : class, IEntityBase, new()
            where TCreateViewModel : class, IEntityBase, new()
        {
            return Create<TModel, TModel, TCreateViewModel>(listViewModel);
        }


        public virtual async Task<ResponseViewModel> Create<TModel, TInfoViewModel, TCreateViewModel>(List<TCreateViewModel> listViewModel)
            where TModel : class, IEntityBase, new()
            where TCreateViewModel : class, IEntityBase, new()
        {
            var listInfo = new List<TInfoViewModel>();
            var listModel = new List<TModel>();

            foreach (var item in listViewModel)
            {
                var model = Mapper.Map<TModel>(item);
                _unitOfWork.RepositoryCRUD<TModel>().Insert(model);
                listModel.Add(model);
            }

            await _unitOfWork.CommitAsync();

            foreach (var item in listModel)
            {
                listInfo.Add(Mapper.Map<TInfoViewModel>(item));
            }
            return ResponseViewModel.CreateSuccess(listModel);
        }

        public virtual Task<ResponseViewModel> Create<TModel>(TModel viewModel)
            where TModel : class, IEntityBase, new()
        {
            return Create<TModel, TModel>(viewModel);
        }

        public virtual Task<ResponseViewModel> Create<TModel, TCreateViewModel>(TCreateViewModel viewModel)
            where TModel : class, IEntityBase, new()
            where TCreateViewModel : class, IEntityBase, new()
        {
            return Create<TModel, TModel, TCreateViewModel>(viewModel);
        }

        public virtual async Task<ResponseViewModel> Create<TModel, TInfoViewModel, TCreateViewModel>(TCreateViewModel viewModel)
            where TModel : class, IEntityBase, new()
            where TCreateViewModel : class, IEntityBase, new()
        {
            try
            {
                TModel model = Mapper.Map<TModel>(viewModel);
                model.Id = 0;
                _unitOfWork.RepositoryCRUD<TModel>().Insert(model);
                await _unitOfWork.CommitAsync();
                return ResponseViewModel.CreateSuccess(Mapper.Map<TInfoViewModel>(model));
            }
            catch (Exception ex)
            {
                return ResponseViewModel.CreateError(ex.Message);
            }
        }

        public virtual Task<ResponseViewModel> Update<TModel>(List<TModel> listViewModel)
            where TModel : class, IEntityBase, new()
        {
            return Update<TModel, TModel, TModel>(listViewModel);
        }

        public virtual Task<ResponseViewModel> Update<TModel, TUpdateViewModel>(List<TUpdateViewModel> listViewModel)
            where TModel : class, IEntityBase, new()
            where TUpdateViewModel : class, IEntityBase, new()
        {
            return Update<TModel, TModel, TUpdateViewModel>(listViewModel);
        }


        public virtual async Task<ResponseViewModel> Update<TModel, TInfoViewModel, TUpdateViewModel>(List<TUpdateViewModel> listViewModel)
            where TModel : class, IEntityBase, new()
            where TUpdateViewModel : class, IEntityBase, new()
        {
            var listInfo = new List<TInfoViewModel>();
            var listModel = new List<TModel>();

            foreach (var item in listViewModel)
            {
                var model = Mapper.Map<TModel>(item);
                _unitOfWork.RepositoryCRUD<TModel>().Update(model);
                listModel.Add(model);
            }

            await _unitOfWork.CommitAsync();

            foreach (var item in listModel)
            {
                listInfo.Add(Mapper.Map<TInfoViewModel>(item));
            }
            return ResponseViewModel.CreateSuccess(listModel);
        }

        public virtual Task<ResponseViewModel> Update<TModel>(TModel viewModel)
            where TModel : class, IEntityBase, new()
        {
            return Update<TModel, TModel>(viewModel);
        }

        public virtual Task<ResponseViewModel> Update<TModel, TUpdateViewModel>(TUpdateViewModel viewModel)
            where TModel : class, IEntityBase, new()
            where TUpdateViewModel : class, IEntityBase, new()
        {
            return Update<TModel, TModel, TUpdateViewModel>(viewModel);
        }

        public virtual async Task<ResponseViewModel> Update<TModel, TInfoViewModel, TUpdateViewModel>(TUpdateViewModel viewModel)
            where TModel : class, IEntityBase, new()
            where TUpdateViewModel : class, IEntityBase, new()
        {
            try
            {
                TModel model = _unitOfWork.RepositoryR<TModel>().GetSingle(viewModel.Id);
                model = Mapper.Map(viewModel, model);
                _unitOfWork.RepositoryCRUD<TModel>().Update(model);
                await _unitOfWork.CommitAsync();
                return ResponseViewModel.CreateSuccess(Mapper.Map<TInfoViewModel>(model));
            }
            catch (Exception ex)
            {
                return ResponseViewModel.CreateError(ex.Message);
            }
        }

        public virtual ResponseViewModel FindBy<TModel>(Expression<Func<TModel, bool>> predicate, int? pageSize = null, int? pageNumber = null, string cols = null)
            where TModel : class, IEntityBase, new()
        {
            return FindBy<TModel, TModel>(predicate, pageSize, pageNumber, cols);
        }

        public virtual ResponseViewModel FindBy<TModel>(Expression<Func<TModel, bool>> predicate, int? pageSize = null, int? pageNumber = null, params string[] includeProperties)
            where TModel : class, IEntityBase, new()
        {
            return FindBy<TModel, TModel>(predicate, pageSize, pageNumber, includeProperties);
        }

        public virtual ResponseViewModel FindBy<TModel, TInfoViewModel>(Expression<Func<TModel, bool>> predicate, int? pageSize = null, int? pageNumber = null, string cols = null)
            where TModel : class, IEntityBase, new()
        {
            var includeProperties = new string[0];

            if (!string.IsNullOrWhiteSpace(cols))
            {
                includeProperties = cols.Split(',');
            }

            return FindBy<TModel, TInfoViewModel>(predicate, pageSize, pageNumber, includeProperties);
        }

        public virtual ResponseViewModel FindBy<TModel, TInfoViewModel>(Expression<Func<TModel, bool>> predicate, int? pageSize = null, int? pageNumber = null, params string[] includeProperties)
            where TModel : class, IEntityBase, new()
        {
            if (pageSize.HasValue && pageNumber.HasValue)
            {
                int iPageNumber = (int)pageNumber;
                int iPageSize = (int)pageSize;

                var totalCount = _unitOfWork.RepositoryR<TModel>().Count();
                var totalPages = Math.Ceiling((double)totalCount / iPageSize);

                var data = _unitOfWork.RepositoryR<TModel>()
                                      .GetAll(includeProperties)
                                      .Where(predicate)
                                      .Skip((iPageNumber - 1) * iPageSize)
                                      .Take(iPageSize);

                return ResponseViewModel.CreateSuccess(Mapper.Map<IEnumerable<TInfoViewModel>>(data), dataCount: totalCount);
            }

            return ResponseViewModel.CreateSuccess(Mapper.Map<IEnumerable<TInfoViewModel>>(_unitOfWork.RepositoryR<TModel>().GetAll(includeProperties).Where(predicate)));
        }

        public virtual ResponseViewModel GetAll<TModel>(Expression<Func<TModel, bool>> predicate, int? pageSize = null, int? pageNumber = null, params string[] includeProperties)
            where TModel : class, IEntityBase, new()
        {
            return GetAll<TModel, TModel>(predicate, pageSize, pageNumber, includeProperties);
        }

        public virtual ResponseViewModel GetAll<TModel, TInfoViewModel>(Expression<Func<TModel, bool>> predicate, int? pageSize = null, int? pageNumber = null, params string[] includeProperties)
            where TModel : class, IEntityBase, new()
        {
            if (pageSize.HasValue && pageNumber.HasValue)
            {
                int iPageNumber = (int)pageNumber;
                int iPageSize = (int)pageSize;

                var totalCount = _unitOfWork.RepositoryR<TModel>().Count();
                var totalPages = Math.Ceiling((double)totalCount / iPageSize);

                var data = _unitOfWork.RepositoryR<TModel>()
                                      .GetAll(includeProperties)
                                      .Where(predicate)
                                      .Skip((iPageNumber - 1) * iPageSize)
                                      .Take(iPageSize);

                return ResponseViewModel.CreateSuccess(Mapper.Map<IEnumerable<TInfoViewModel>>(data), dataCount: totalCount);
            }

            return ResponseViewModel.CreateSuccess(Mapper.Map<IEnumerable<TInfoViewModel>>(_unitOfWork.RepositoryR<TModel>().GetAll(includeProperties).Where(predicate)));
        }

        ResponseViewModel IGeneralService.GetSingle<TModel>(Expression<Func<TModel, bool>> predicate, string cols)
        {
            return GetSingle<TModel, TModel>(predicate, cols);
        }

        ResponseViewModel IGeneralService.GetSingle<TModel>(Expression<Func<TModel, bool>> predicate, params string[] includeProperties)
        {
            return GetSingle<TModel, TModel>(predicate, includeProperties);
        }

        public virtual ResponseViewModel GetSingle<TModel>(Expression<Func<TModel, bool>> predicate, params Expression<Func<TModel, object>>[] includeProperties)
            where TModel : class, IEntityBase, new()
        {
            return GetSingle<TModel, TModel>(predicate, includeProperties);
        }


        ResponseViewModel IGeneralService.GetSingle<TModel, TInfoViewModel>(Expression<Func<TModel, bool>> predicate, string cols)
        {
            var includeProperties = new string[0];

            if (!string.IsNullOrWhiteSpace(cols))
            {
                includeProperties = cols.Split(',');
            }

            return GetSingle<TModel, TInfoViewModel>(predicate, includeProperties);
        }

        public virtual ResponseViewModel GetSingle<TModel, TInfoViewModel>(Expression<Func<TModel, bool>> predicate, params string[] includeProperties)
            where TModel : class, IEntityBase, new()
        {
            return ResponseViewModel.CreateSuccess(Mapper.Map<TInfoViewModel>(_unitOfWork.RepositoryR<TModel>().GetSingle(predicate, includeProperties)));
        }

        public virtual ResponseViewModel GetSingle<TModel, TInfoViewModel>(Expression<Func<TModel, bool>> predicate, params Expression<Func<TModel, object>>[] includeProperties)
            where TModel : class, IEntityBase, new()
        {
            var entity = _unitOfWork.RepositoryR<TModel>().GetSingle(predicate, includeProperties);

            if (entity != null)
                return ResponseViewModel.CreateSuccess(Mapper.Map<TInfoViewModel>(entity));

            return ResponseViewModel.CreateSuccess();
        }
    }
}
