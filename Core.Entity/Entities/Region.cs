using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entity.Abstract;

namespace Core.Entity.Entities
{
    public class Region : EntitySimple
    {
        public Region()
        {
        }
        public bool IsActive { get; set; }
        public string Note { get; set; }
        public int ProvinceId { get; set; }
    }
}
