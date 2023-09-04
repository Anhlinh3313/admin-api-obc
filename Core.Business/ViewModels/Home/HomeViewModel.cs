using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;

namespace Core.Business.ViewModels.Home
{
    public class HomeViewModel
    {
        public HomeViewModel()
        {
            
        }
        public int TypeId { get; set; }
        public string TypeName { get; set; }
    }
}
