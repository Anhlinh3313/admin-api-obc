using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;

namespace Core.Business.ViewModels.MembershipAction
{
    public class MembershipActionViewModel : IEntityBase
    {
        public MembershipActionViewModel()
        {
            
        }
        public int Id { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime CreatedWhen { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public string Time { get; set; }
        public int ExpenseId { get; set; }
        public DateTime? ExtendDate { get; set; }
        public int CustomerId { get; set; }
    }
}
