using System;
using System.Threading.Tasks;
using AutoMapper;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Data.Abstract;
using Core.Entity.Entities;
using Core.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Core.Business.ViewModels.Introduce;

namespace Core.Business.Services
{
    public class IntroduceService : BaseService, IIntroduceService
    {
        public IGeneralService _iGeneralRawService { get; set; }
        public IntroduceService(Microsoft.Extensions.Logging.ILogger<dynamic> logger, 
            IOptions<AppSettings> optionsAccessor, 
            IUnitOfWork unitOfWork,
            IGeneralService iGeneralRawService) : base(logger, optionsAccessor, unitOfWork)
        {
            _iGeneralRawService = iGeneralRawService;
        }

        public async Task<JsonResult> GetListIntroduceAsync()
        {
            try
            {
                var result = await _unitOfWork.RepositoryR<Introduce>().GetAll().ToListAsync();
                return JsonUtil.Success(result);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> CreateIntroduceAsync(IntroduceViewModel model)
        {
            try
            {
                var checkIntroduce = await _unitOfWork.RepositoryR<Introduce>().AnyAsync();
                if (!string.IsNullOrEmpty(model.Content) || !string.IsNullOrWhiteSpace(model.Content))
                    model.Content = model.Content.Trim();
                if (!string.IsNullOrEmpty(model.Address) || !string.IsNullOrWhiteSpace(model.Address))
                    model.Address = model.Address.Trim();
                if (!string.IsNullOrEmpty(model.Email) || !string.IsNullOrWhiteSpace(model.Email))
                    model.Email = model.Email.Trim();
                if (!string.IsNullOrEmpty(model.Facebook) || !string.IsNullOrWhiteSpace(model.Facebook))
                    model.Facebook = model.Facebook.Trim();
                if (!string.IsNullOrEmpty(model.Instagram) || !string.IsNullOrWhiteSpace(model.Instagram))
                    model.Instagram = model.Instagram.Trim();
                if (!string.IsNullOrEmpty(model.PhoneNumber) || !string.IsNullOrWhiteSpace(model.PhoneNumber))
                    model.PhoneNumber = model.PhoneNumber.Trim();
                if (!string.IsNullOrEmpty(model.Twitter) || !string.IsNullOrWhiteSpace(model.Twitter))
                    model.Twitter = model.Twitter.Trim();
                if (!string.IsNullOrEmpty(model.Website) || !string.IsNullOrWhiteSpace(model.Website))
                    model.Website = model.Website.Trim();
                if (!string.IsNullOrEmpty(model.Youtube) || !string.IsNullOrWhiteSpace(model.Youtube))
                    model.Youtube = model.Youtube.Trim();
                if (!string.IsNullOrEmpty(model.AccountName) || !string.IsNullOrWhiteSpace(model.AccountName))
                    model.AccountName = model.AccountName.Trim();
                if (!string.IsNullOrEmpty(model.AccountNumber) || !string.IsNullOrWhiteSpace(model.AccountNumber))
                    model.AccountNumber = model.AccountNumber.Trim();
                if (!string.IsNullOrEmpty(model.Bank) || !string.IsNullOrWhiteSpace(model.Bank))
                    model.Bank = model.Bank.Trim();
                if (!string.IsNullOrEmpty(model.BankBranch) || !string.IsNullOrWhiteSpace(model.BankBranch))
                    model.BankBranch = model.BankBranch.Trim();
                if (!string.IsNullOrEmpty(model.LinkGroupChat) || !string.IsNullOrWhiteSpace(model.LinkGroupChat))
                    model.LinkGroupChat = model.LinkGroupChat.Trim();
                if (!checkIntroduce)
                {
                    Introduce introduce = Mapper.Map<Introduce>(model);
                    introduce.Id = 0;
                    _unitOfWork.RepositoryCRUD<Introduce>().Insert(introduce);
                    await _unitOfWork.CommitAsync();
                    return JsonUtil.Success(Mapper.Map<IntroduceViewModel>(introduce));
                }
                else
                {
                    var introduce = await _unitOfWork.RepositoryR<Introduce>().GetSingleAsync(x => x.Id > 0);
                    model.Id = introduce.Id;
                    model.IsEnabled = true;

                    introduce = Mapper.Map(model, introduce);
                    _unitOfWork.RepositoryCRUD<Introduce>().Update(introduce);
                    await _unitOfWork.CommitAsync();
                    return JsonUtil.Success(introduce);
                }
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message); 
            }
        }
    }
}
