using System;
using AutoMapper;
using Core.Business.ViewModels.Accounts;
using Core.Business.ViewModels.Business;
using Core.Business.ViewModels.Chapter;
using Core.Business.ViewModels.Customer;
using Core.Business.ViewModels.Expense;
using Core.Business.ViewModels.FAQs;
using Core.Business.ViewModels.MembershipAction;
using Core.Business.ViewModels.ParticipatingProvince;
using Core.Business.ViewModels.Region;
using Core.Business.ViewModels.User;
using Core.Entity.Abstract;
using Core.Infrastructure.Http;
using Core.Infrastructure.Security;
using Core.Infrastructure.Utils;

namespace Core.Business.ViewModels.Mappings
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            //Account
            CreateMap<CreateAccountViewModel, Entity.Entities.User>()
                .AfterMap((src, dest) =>
                {
                    SetGeneralColsCreate(dest);
                    dest.SecurityStamp = Guid.NewGuid().ToString();
                    dest.PasswordHash = new Encryption().EncryptPassword(src.PassWord, dest.SecurityStamp);
                }).ReverseMap();

            CreateMap<UpdateAccountViewModel, Entity.Entities.User>()
                .AfterMap((src, dest) =>
                {
                    SetGeneralColsUpdate(dest);
                    if (!Util.IsNull(src.PassWord))
                    {
                        //dest.SecurityStamp = Guid.NewGuid().ToString();
                        //dest.PasswordHash = new Encryption().EncryptPassword(src.PassWord, dest.SecurityStamp);
                    }
                }).ReverseMap();

            CreateMap<ChangePassWordViewModel, Entity.Entities.User>()
                .AfterMap((src, dest) =>
                {
                    SetGeneralColsUpdate(dest);
                    if (!Util.IsNull(src.NewPassWord))
                    {
                        dest.SecurityStamp = Guid.NewGuid().ToString();
                        dest.PasswordHash = new Encryption().EncryptPassword(src.NewPassWord, dest.SecurityStamp);
                    }
                }).ReverseMap();
            //CreateMap<VehiclesViewModel, Vehicle>().AfterMap((src, dest) => { if (src.Id > 0) SetGeneralColsUpdate(dest); else SetGeneralColsCreate(dest); }).ReverseMap();
           
            CreateMap<ChangePassWordViewModel, Entity.Entities.User>()
                .AfterMap((src, dest) =>
                {
                    SetGeneralColsUpdate(dest);
                    if (!Util.IsNull(src.NewPassWord))
                    {
                        dest.SecurityStamp = Guid.NewGuid().ToString();
                        dest.PasswordHash = new Encryption().EncryptPassword(src.NewPassWord, dest.SecurityStamp);
                    }
                }).ReverseMap();
            // Customer
            CreateMap<CustomerViewModelCreate, Entity.Entities.Customer>()
                .AfterMap((src, dest) =>
                {
                    SetGeneralColsCreate(dest);
                    dest.SecurityStamp = Guid.NewGuid().ToString();
                    dest.PasswordHash = new Encryption().EncryptPassword(src.Password, dest.SecurityStamp);
                }).ReverseMap();
            CreateMap<CustomerViewModel, Entity.Entities.Customer>()
                .AfterMap((src, dest) =>
                {
                    SetGeneralColsUpdate(dest);
                }).ReverseMap();
            // Business
            CreateMap<BusinessViewModelCreate, Entity.Entities.Business>()
                .AfterMap((src, dest) =>
                {
                    SetGeneralColsCreate(dest);
                }).ReverseMap();
            CreateMap<BusinessViewModel, Entity.Entities.Business>()
                .AfterMap((src, dest) =>
                {
                    SetGeneralColsUpdate(dest);
                }).ReverseMap();
            // User
            CreateMap<UserViewModel, Entity.Entities.User>()
                .AfterMap((src, dest) =>
                {
                    SetGeneralColsCreate(dest);
                    dest.SecurityStamp = Guid.NewGuid().ToString();
                    dest.PasswordHash = new Encryption().EncryptPassword(src.Password, dest.SecurityStamp);
                }).ReverseMap();
            CreateMap<UserViewModel, Entity.Entities.User>()
                .AfterMap((src, dest) =>
                {
                    SetGeneralColsUpdate(dest);
                    if (!string.IsNullOrWhiteSpace(src.Password) || !string.IsNullOrEmpty(src.Password))
                    {
                        dest.SecurityStamp = Guid.NewGuid().ToString();
                        dest.PasswordHash = new Encryption().EncryptPassword(src.Password, dest.SecurityStamp);
                    }
                }).ReverseMap();
            // Province
            CreateMap<ParticipatingProvinceViewModel, Entity.Entities.ParticipatingProvince>()
                .AfterMap((src, dest) =>
                {
                    SetGeneralColsCreate(dest);
                }).ReverseMap();
            CreateMap<ParticipatingProvinceViewModel, Entity.Entities.ParticipatingProvince>()
                .AfterMap((src, dest) =>
                {
                    SetGeneralColsUpdate(dest);
                }).ReverseMap();
            // Region
            CreateMap<RegionViewModelCreate, Entity.Entities.Region>()
                .AfterMap((src, dest) =>
                {
                    SetGeneralColsCreate(dest);
                }).ReverseMap();
            CreateMap<RegionViewModelCreate, Entity.Entities.Region>()
                .AfterMap((src, dest) =>
                {
                    SetGeneralColsUpdate(dest);
                }).ReverseMap();
            // Chapter
            CreateMap<ChapterViewModelCreate, Entity.Entities.Chapter>()
                .AfterMap((src, dest) =>
                {
                    SetGeneralColsCreate(dest);
                }).ReverseMap();
            CreateMap<ChapterViewModelCreate, Entity.Entities.Chapter>()
                .AfterMap((src, dest) =>
                {
                    SetGeneralColsUpdate(dest);
                }).ReverseMap();
            // FAQs
            CreateMap<FAQsViewModel, Entity.Entities.FAQs>()
                .AfterMap((src, dest) =>
                {
                    SetGeneralColsCreate(dest);
                }).ReverseMap();
            CreateMap<FAQsViewModel, Entity.Entities.FAQs>()
                .AfterMap((src, dest) =>
                {
                    SetGeneralColsUpdate(dest);
                }).ReverseMap();
            // Expense
            CreateMap<ExpenseViewModel, Entity.Entities.Expense>()
                .AfterMap((src, dest) =>
                {
                    SetGeneralColsCreate(dest);
                }).ReverseMap();
            CreateMap<ExpenseViewModel, Entity.Entities.Expense>()
                .AfterMap((src, dest) =>
                {
                    SetGeneralColsUpdate(dest);
                }).ReverseMap();
        }

        public void SetGeneralColsCreate(IEntityBasic data)
        {
            var currentDate = DateTime.Now;
            var currentUserId = HttpContext.CurrentUserId;

            data.Id = 0;
            data.ConcurrencyStamp = Guid.NewGuid().ToString();
            data.CreatedWhen = currentDate;
            data.CreatedBy = currentUserId;
            data.ModifiedWhen = currentDate;
            data.ModifiedBy = currentUserId;
            data.IsEnabled = true;
        }

        public void SetGeneralColsUpdate(IEntityBasic data)
        {
            data.ConcurrencyStamp = Guid.NewGuid().ToString();
            data.ModifiedWhen = DateTime.Now;
            data.ModifiedBy = HttpContext.CurrentUserId;
        }
    }
}
