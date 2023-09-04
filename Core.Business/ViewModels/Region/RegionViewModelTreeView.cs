using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Business.ViewModels.Chapter;
using Core.Entity.Abstract;

namespace Core.Business.ViewModels.Region
{
    public class RegionViewModelTreeView : IEntityBase
    {
        public RegionViewModelTreeView()
        {
            
        }
        public int Id { get; set; }
        public bool IsEnabled { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }
        public string Note { get; set; }
        public int ProvinceId { get; set; }
        public List<ChapterViewModelCreate> Children { get; set; }
    }
}
