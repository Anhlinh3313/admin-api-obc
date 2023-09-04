using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;

namespace Core.Business.ViewModels.Business
{
    public class BusinessViewModelCreate : IEntityBase
    {
        public BusinessViewModelCreate()
        {
            
        }
        public int Id { get; set; }
        public bool IsEnabled { get; set; }

        public string BusinessName { get; set; }
        public string Position { get; set; }
        public int ProfessionId { get; set; }
        public int CustomerId { get; set; }
        public bool IsActive { get; set; }
    }
}
