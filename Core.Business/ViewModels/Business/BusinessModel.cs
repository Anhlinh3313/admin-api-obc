using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;

namespace Core.Business.ViewModels.Business
{ 
    public class BusinessModel
    {
        public BusinessModel()
        {
            
        }

        public string KeySearch { get; set; }
        public string Province { get; set; }
        public string Profession { get; set; }
        public string FieldOperation { get; set; }
        public int? CustomerRole { get; set; }
        public int? PageNum { get; set; }
        public int? PageSize { get; set; }
    }
}
