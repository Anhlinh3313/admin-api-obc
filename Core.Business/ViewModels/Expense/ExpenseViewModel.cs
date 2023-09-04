using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;

namespace Core.Business.ViewModels.Expense
{
    public class ExpenseViewModel : IEntityBase
    {
        public ExpenseViewModel()
        {
            
        }
        public int Id { get; set; }
        public bool IsEnabled { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int Duration { get; set; }
        public string TimeType { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
    }
}
