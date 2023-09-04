using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entity.Abstract;

namespace Core.Entity.Entities
{
    public class Provinces : IEntityBase
    {
        public Provinces()
        {
        }
        public int Id { get; set; }
        public bool IsEnabled { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string VSEOracleCode { get; set; }
        public string FullName { get; set; }
        public string Level { get; set; }
    }
}
