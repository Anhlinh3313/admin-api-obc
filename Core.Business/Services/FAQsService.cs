using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels;
using Core.Data.Abstract;
using Core.Entity.Entities;
using Core.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Core.Business.ViewModels.FAQs;

namespace Core.Business.Services
{
    public class FAQsService : BaseService, IFAQsService
    {
        public IGeneralService _iGeneralRawService { get; set; }
        public FAQsService(Microsoft.Extensions.Logging.ILogger<dynamic> logger, 
            IOptions<AppSettings> optionsAccessor, 
            IUnitOfWork unitOfWork,
            IGeneralService iGeneralRawService) : base(logger, optionsAccessor, unitOfWork)
        {
            _iGeneralRawService = iGeneralRawService;
        }

        public async Task<JsonResult> GetListFAQsAsync(string keySearch, int pageNum, int pageSize)
        {
            try
            {
                var listFAQs = await _unitOfWork.RepositoryR<FAQs>()
                    .FindBy(x => x.IsEnabled == true &&
                                 (string.IsNullOrEmpty(keySearch) ||
                                  x.Question.ToLower().Contains(keySearch.Trim().ToLower()))).OrderByDescending(x => x.Id).ToListAsync();
                var listFAQ = listFAQs.OrderBy(x => x.Priority).ToList();
                var total = listFAQ.Count();
                var totalPage = (int)Math.Ceiling((double)total / pageSize);
                var result = listFAQ.Skip((pageNum - 1) * pageSize).Take(pageSize).ToList();
                return JsonUtil.Success(result, "Success", total);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> GetDetailFAQsAsync(int id)
        {
            try
            {
                var result = await _unitOfWork.RepositoryR<FAQs>().GetSingleAsync(x => x.Id == id);
                return JsonUtil.Success(result);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> CreateFAQsAsync(FAQsViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Question) || string.IsNullOrWhiteSpace(model.Question))
                    return JsonUtil.Error(ValidatorMessage.FAQ.QuestionNotEmpty);

                model.Question = model.Question.Trim();

                if (_unitOfWork.RepositoryR<FAQs>().Any(x => x.Question.ToLower().Equals(model.Question.ToLower())))
                    return JsonUtil.Error(ValidatorMessage.FAQ.UniqueQuestion);

                model.IsActive = true;
                return JsonUtil.Success(await _iGeneralRawService.Create<FAQs, FAQsViewModel>(model));
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> UpdateFAQsAsync(FAQsViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Question) || string.IsNullOrWhiteSpace(model.Question))
                    return JsonUtil.Error(ValidatorMessage.FAQ.QuestionNotEmpty);

                model.Question = model.Question.Trim();

                var faqOld = await _unitOfWork.RepositoryR<FAQs>().GetSingleAsync(x => x.Id == model.Id);
                if (!faqOld.Question.ToLower().Equals(model.Question.ToLower()))
                    if (_unitOfWork.RepositoryR<FAQs>().Any(x => x.Question.ToLower().Equals(model.Question.ToLower()) && x.Id != model.Id))
                        return JsonUtil.Error(ValidatorMessage.FAQ.UniqueQuestion);
                if (faqOld.Priority !=model.Priority)
                    if (_unitOfWork.RepositoryR<FAQs>().Any(x => x.Priority == model.Priority && x.Id != model.Id))
                        return JsonUtil.Error(ValidatorMessage.FAQ.UniquePriority);

                if (model.IsActive == null) model.IsActive = faqOld.IsActive;
                model.IsEnabled = true;
                return JsonUtil.Success(await _iGeneralRawService.Update<FAQs, FAQsViewModel>(model));
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> DeEnabledFAQsAsync(int faqId)
        {
            try
            {
                var faq = await _unitOfWork.RepositoryR<FAQs>().GetSingleAsync(x => x.Id == faqId);
                if (faq == null) return JsonUtil.Error(ValidatorMessage.FAQ.NotExist);
                faq.IsEnabled = !faq.IsEnabled;
                _unitOfWork.RepositoryCRUD<FAQs>().Update(faq);
                await _unitOfWork.CommitAsync();
                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> DeActiveFAQsAsync(int faqId)
        {
            try
            {
                var faq = await _unitOfWork.RepositoryR<FAQs>().GetSingleAsync(x => x.Id == faqId);
                if (faq == null) return JsonUtil.Error(ValidatorMessage.FAQ.NotExist);
                faq.IsActive = !faq.IsActive;
                _unitOfWork.RepositoryCRUD<FAQs>().Update(faq);
                await _unitOfWork.CommitAsync();
                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> GetListFAQsMobile(string keySearch, int pageNum, int pageSize)
        {
            try
            {
                var listFAQs = await _unitOfWork.RepositoryR<FAQs>()
                    .FindBy(x => x.IsActive == true && x.Priority != 0 &&
                                 (string.IsNullOrEmpty(keySearch) ||
                                  x.Question.ToLower().Contains(keySearch.Trim().ToLower()))).OrderByDescending(x => x.Id).ToListAsync();
                var listFAQ = listFAQs.OrderBy(x => x.Priority).ToList();
                var total = listFAQ.Count();
                var totalPage = (int)Math.Ceiling((double)total / pageSize);
                var result = listFAQ.Skip((pageNum - 1) * pageSize).Take(pageSize).ToList();
                return JsonUtil.Success(result, "Success", total);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> GetAllFAQsMobile(string keySearch)
        {
            try
            {
                var listFAQs = await _unitOfWork.RepositoryR<FAQs>()
                    .FindBy(x => x.IsActive == true && x.Priority != 0 &&
                                 (string.IsNullOrEmpty(keySearch) ||
                                  x.Question.ToLower().Contains(keySearch.Trim().ToLower()))).ToListAsync();
                var listFAQ = listFAQs.OrderBy(x => x.Priority).ToList();
                var total = listFAQ.Count();
                return JsonUtil.Success(listFAQ, "Success", total);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }
    }
}
