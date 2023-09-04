using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;

namespace Core.Business.ViewModels.ChapterMemberCompany
{
    public class ChapterMemberCompanyViewModel
    {
        public ChapterMemberCompanyViewModel()
        {
            
        }
        public long Id { get; set; }
        public int CustomerId { get; set; }
        public string FullName { get; set; }
        public string BusinessName { get; set; }
        public string ProfessionName { get; set; }
        public List<string> Note { get; set; }
    }
}
