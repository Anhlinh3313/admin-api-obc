using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entity.Abstract;

namespace Core.Entity.Entities
{
    public class Districts : IEntityBase
    {
        public Districts()
        {
        }
        public int Id { get; set; }
        public bool IsEnabled { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string VSEOracleCode { get; set; }
        public string FullName { get; set; }
        public string Level { get; set; }
        public int ProvinceId { get; set; }
        public bool IsRemote { get; set; }
        public int KmNumber { get; set; }
    }
}
