using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Business.ViewModels
{
    public class FilterViewModel
    {
        public int? Id { get; set; }
        public string SearchText { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
}
