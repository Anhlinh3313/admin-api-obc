using System;
using System.Linq;
using AutoMapper;
using Core.Business.ViewModels.Accounts;
using Core.Business.ViewModels.General;
using Core.Data;
using Core.Data.Core;
using Core.Entity.Entities;
using Core.Entity.Procedures;
using Core.Infrastructure.Security;

namespace Core.Business.ViewModels.Mappings
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            //CreateMap<User, UserInfoViewModel>().AfterMap((src, dest) =>
            //{
            //    using (var context = new ApplicationContext())
            //    {
            //        UnitOfWork unitOfWork = new UnitOfWork(context);
            //        //dest.RoleIds = unitOfWork.RepositoryR<UserRole>().FindBy(x => x.UserId == src.Id).Select(x => x.RoleId).ToArray();
            //        //var supplierUser = unitOfWork.RepositoryR<SupplierUser>().GetSingle(x => x.UserId == src.Id);
            //        //if(supplierUser != null)
            //        //{
            //        //    dest.SupplierId = supplierUser.SupplierId;
            //        //}
            //        //else
            //        //{
            //        //    dest.SupplierId = null;
            //        //}
            //    }
            //}).ReverseMap();

            //CreateMap<SupplierUser, SupplierUserInfoViewModdel>().AfterMap((src, dest) =>
            //{
            //    using (var context = new ApplicationContext())
            //    {
            //        UnitOfWork unitOfWork = new UnitOfWork(context);
            //        dest.RoleIds = unitOfWork.RepositoryR<UserRole>().FindBy(x => x.UserId == src.UserId && x.IsEnabled == true).Select(x => x.RoleId).ToArray();
            //        var user= unitOfWork.RepositoryR<User>().GetSingle(x => x.Id == src.UserId && x.IsEnabled == true);
            //        if(user != null)
            //        {
            //            dest.UserName = user.UserName;
            //            dest.IsBlocked = user.IsBlocked;
            //            dest.AvatarPath = user.AvatarPath;
            //        }
            //    }
            //}).ReverseMap();

            //CreateMap<District, DistrictInfoViewModel>();
         

            //CreateMap<PriceService, PriceServiceInfoViewModel>().AfterMap((src, dest) =>
            //{
            //    using (var context = new ApplicationContext())
            //    {
            //        UnitOfWork unitOfWork = new UnitOfWork(context);
            //        dest.PriceServiceDetails = unitOfWork.RepositoryR<PriceServiceDetail>().FindBy(x => x.PriceServiceId == src.Id).ToList();
            //    }
            //});
            //CreateMap<Proc_GetListTransportations, BookingByTransportationInfoViewModel>();
        }
    }
}
