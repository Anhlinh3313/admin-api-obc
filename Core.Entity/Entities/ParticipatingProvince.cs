using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entity.Abstract;

namespace Core.Entity.Entities
{
    public class ParticipatingProvince : EntitySimple
    {
        public ParticipatingProvince()
        {
        }
        public bool IsActive { get; set; }
        public string Note { get; set; }
    }
}
