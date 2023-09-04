using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;

namespace Core.Business.ViewModels.Thanks
{
    public class ThanksViewModelCreate : IEntityBase
    {
        public ThanksViewModelCreate()
        {
            
        }
        public int Id { get; set; }
        public bool IsEnabled { get; set; }
        public int ReceiverId { get; set; }
        public string ReceiverName { get; set; }
        public string Type { get; set; }
        public long Value { get; set; }
        public string Note { get; set; }
    }
}
