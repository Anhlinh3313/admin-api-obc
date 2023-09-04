using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;

namespace Core.Business.ViewModels.LogAction
{
    public class LogActionViewModel : IEntityBase
    {
        public LogActionViewModel()
        {
            
        }
        public int Id { get; set; }
        public bool IsEnabled { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedWhen { get; set; }
        public string ActionName { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
    }
}
