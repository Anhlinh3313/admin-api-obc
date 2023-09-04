using Microsoft.Extensions.DependencyInjection;
using Core.Data.Abstract;
using Core.Business.Services;
using Core.Data.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Core.Api.Core.Sercurity;
using Core.Business.Services.Abstract;

namespace Core.API
{
    public partial class Startup
    {
        private void MappingScopeService(IServiceCollection services)
        {
            //General
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //services.AddSingleton<IAuthorizationHandler, PermissionsHandler>();
            //UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //Services
            services.AddScoped(typeof(IGeneralService), typeof(GeneralService));
            services.AddScoped(typeof(IGeneralService<,>), typeof(GeneralService<,>));
            services.AddScoped(typeof(IGeneralService<,,>), typeof(GeneralService<,,>));
            services.AddScoped(typeof(IGeneralService<,,,>), typeof(GeneralService<,,,>));
            services.AddScoped<IEncryptionService, EncryptionService>();
            services.AddScoped<IAccountService, AccountService>();
            //services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IParticipatingProvinceService, ParticipatingProvinceService>();
            services.AddScoped<IRegionService, RegionService>();
            services.AddScoped<IChapterService, ChapterService>();
            services.AddScoped<IIntroduceService, IntroduceService>();
            services.AddScoped<IFAQsService, FAQsService>();
            services.AddScoped<ITermConditionService, TermConditionService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IBusinessService, BusinessService>();
            services.AddScoped<IProvinceService, ProvinceService>();
            services.AddScoped<IDistrictService, DistrictService>();
            services.AddScoped<IWardService, WardService>();
            services.AddScoped<IProfessionService, ProfessionService>();
            services.AddScoped<IFieldOperationsService, FieldOperationsService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IExpenseService, ExpenseService>();
            services.AddScoped<IStatusService, StatusService>();
            services.AddScoped<IMembershipActionService, MembershipActionService>();
            services.AddScoped<ILogActionService, LogActionService>();
            services.AddScoped<IOpportunityService, OpportunityService>();
            services.AddScoped<IThanksService, ThanksService>();
            services.AddScoped<IFaceToFaceService, FaceToFaceService>();
            services.AddScoped<IGuestsService, GuestsService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<INotifyService, NotifyService>();
            services.AddScoped<IPageService, PageService>();
            services.AddScoped<IRolePageService, RolePageService>();
            services.AddScoped<IChapterMemberCompanyService, ChapterMemberCompanyService>();
            services.AddScoped<IClassificationsService, ClassificationsService>();
            services.AddScoped<IMeetingChapterService, MeetingChapterService>();
            services.AddScoped<IMembershipDuesReportService, MembershipDuesReportService>();
            services.AddScoped<IKpiService, KpiService>();
            services.AddScoped<IHomeService, HomeService>();
            services.AddScoped<IConnectionManager, ConnectionManager>();
            services.AddScoped<ISignalRHubService, SignalRHubService>();
            services.AddScoped<IAbsenceMedicalService, AbsenceMedicalService>();

            //Repository
            services.AddScoped(typeof(IEntityCRUDRepository<>), typeof(EntityCRUDRepository<>));
            services.AddScoped(typeof(IEntityRRepository<>), typeof(EntityRRepository<>));
            services.AddScoped(typeof(IEntityVPRepository<>), typeof(EntityVPRepository<>));
        }
    }
}
