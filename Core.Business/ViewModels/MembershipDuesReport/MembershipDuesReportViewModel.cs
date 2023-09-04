using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;
using Core.Entity.Procedures;

namespace Core.Business.ViewModels.MembershipDuesReport
{
    public class MembershipDuesReportViewModel
    {
        public MembershipDuesReportViewModel()
        {
            
        }
        public List<Proc_GetMembershipDuesReportAllMember> AllMember { get; set; }
        public List<Proc_GetMembershipDuesReportAllMemberExpired> AllMemberExpired { get; set; }
        public List<Proc_GetMembershipDuesReportAllMemberLate> AllMemberLate { get; set; }
        public List<Proc_GetMembershipDuesReportAllNewMember> AllNewMember { get; set; }
    }
}
