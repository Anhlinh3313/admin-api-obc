using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;

namespace Core.Business.ViewModels.ParticipatingProvince
{
    public class ParticipatingProvinceViewModel : IEntityBase
    {
        public ParticipatingProvinceViewModel()
        {
            
        }
        public int Id { get; set; }
        public bool IsEnabled { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }
        public string Note { get; set; }
    }
}
