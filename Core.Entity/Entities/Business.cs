using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entity.Abstract;

namespace Core.Entity.Entities
{
    public class Business : EntityBasic
    {
        public Business()
        {
        }
        public string Position { get; set; }
        public int ProfessionId { get; set; }
        public string BusinessName { get; set; }
        public int? FieldOperationsId { get; set; }
        public string TaxCode { get; set; }
        public int? WardId { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public int? ParticipatingProvinceId { get; set; }
        public int? ParticipatingRegionId { get; set; }
        public int? ParticipatingChapterId { get; set; }
        public int? CustomerId { get; set; }
        public bool IsActive { get; set; }
        public DateTime? DateJoin { get; set; }
        public string AvatarPath { get; set; }
        public int? NewProfessionId { get; set; }
        public int? NewFieldOperationsId { get; set; }
    }
}
