using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entity.Abstract;

namespace Core.Entity.Entities
{
    public class Chapter : EntitySimple
    {
        public Chapter()
        {
        }
        public bool IsActive { get; set; }
        public string Note { get; set; }
        public int RegionId { get; set; }
        public int ProvinceId { get; set; }
        public string LinkGroupChat { get; set; }
        public string QrCodePath { get; set; }
    }
}
