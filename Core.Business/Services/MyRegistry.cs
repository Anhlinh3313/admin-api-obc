using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Data.Abstract;
using Core.Data.Core;
using FluentScheduler;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Business.Services
{
    public class MyRegistry : Registry
    {
        public MyRegistry(IServiceScopeFactory serviceScopeFactory)
        {
            Schedule(() => new MyJob(serviceScopeFactory).Execute()).ToRunNow().AndEvery(4).Hours();
            Schedule(() => new GetAllCustomerExpiredJob(serviceScopeFactory).Execute()).ToRunEvery(1).Days().At(10, 00);
            Schedule(() => new CheckMembershipActionExpiredJob(serviceScopeFactory).Execute()).ToRunNow().AndEvery(1)
                .Hours();
            //Schedule(() => new MembershipActionJob(membershipActionService)).ToRunNow().AndEvery(1).Minutes();

        }
    }
}
