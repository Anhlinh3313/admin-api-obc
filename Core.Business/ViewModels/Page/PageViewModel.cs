using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;

namespace Core.Business.ViewModels.Page
{
    public class PageViewModel
    {
        public PageViewModel()
        {
            
        }
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public PageViewModel[] Children { get; set; }
    }
}
