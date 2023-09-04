using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;

namespace Core.Business.ViewModels.FAQs
{
    public class FAQsViewModel : IEntityBase
    {
        public FAQsViewModel()
        {
            
        }
        public int Id { get; set; }
        public bool IsEnabled { get; set; }
        public string Question { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
        public int Priority { get; set; }
    }
}
