using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Business.ViewModels;
using Core.Business.ViewModels.Accounts;
using Core.Data.Abstract;
using Core.Entity.Entities;
using Core.Entity.Procedures;
using Core.Infrastructure.Utils;
using FluentScheduler;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Business.Services
{
    public class MyJob : IJob
    {
        private IServiceScopeFactory _serviceScopeFactory;
        public MyJob(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void Execute()
        {
            //_meetingChapterService.GetAllMeetingChapterExpired();
            using (var serviceScope = _serviceScopeFactory.CreateScope())
            {
                IMeetingChapterService meetingChapterService = serviceScope.ServiceProvider.GetService<IMeetingChapterService>();
                meetingChapterService.GetAllMeetingChapterExpired();
            }
        }

    }

    public class GetAllCustomerExpiredJob : IJob
    {
        private IServiceScopeFactory _serviceScopeFactory;
        public GetAllCustomerExpiredJob(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void Execute()
        {
            //_meetingChapterService.GetAllMeetingChapterExpired();
            using (var serviceScope = _serviceScopeFactory.CreateScope())
            {
                IMembershipActionService membershipActionService = serviceScope.ServiceProvider.GetService<IMembershipActionService>();
                membershipActionService.GetAllCustomerExpired();
            }
        }

    }

    public class CheckMembershipActionExpiredJob : IJob
    {
        private IServiceScopeFactory _serviceScopeFactory;
        public CheckMembershipActionExpiredJob(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void Execute()
        {
            //_meetingChapterService.GetAllMeetingChapterExpired();
            using (var serviceScope = _serviceScopeFactory.CreateScope())
            {
                IMembershipActionService membershipActionService = serviceScope.ServiceProvider.GetService<IMembershipActionService>();
                membershipActionService.CheckMembershipActionExpired();
            }
        }

    }
}
