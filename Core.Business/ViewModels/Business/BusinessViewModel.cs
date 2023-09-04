using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;

namespace Core.Business.ViewModels.Business
{ 
    public class BusinessViewModel : IEntityBase
    {
        public BusinessViewModel()
        {
            
        }
        public int Id { get; set; }
        public bool IsEnabled { get; set; }
        public string BusinessName { get; set; }
        public int ProfessionId { get; set; }
        public int? FieldOperationsId { get; set; }
        public string TaxCode { get; set; }
        public string Position { get; set; }
        public int ProvinceId { get; set; }
        public int DistrictId { get; set; }
        public int WardId { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string AvatarPath { get; set; }

        public string ProfessionName { get; set; }
        public string FieldOperationsName { get; set; }
    }
}
