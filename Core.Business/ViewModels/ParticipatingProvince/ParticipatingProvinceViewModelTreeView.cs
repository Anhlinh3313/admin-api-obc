using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Business.ViewModels.Region;
using Core.Entity.Abstract;

namespace Core.Business.ViewModels.ParticipatingProvince
{
    public class ParticipatingProvinceViewModelTreeView : IEntityBase
    {
        public ParticipatingProvinceViewModelTreeView()
        {
            
        }
        public int Id { get; set; }
        public bool IsEnabled { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }
        public string Note { get; set; }
        public List<RegionViewModelTreeView> Children { get; set; }
    }
}
