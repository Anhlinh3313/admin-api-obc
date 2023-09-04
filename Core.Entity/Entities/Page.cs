using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entity.Abstract;

namespace Core.Entity.Entities
{
    public class Page : IEntityBase
    {
        public Page()
        {
        }
        public int Id { get; set; }
        public bool IsEnabled { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Icon { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public string PathName { get; set; }
        public int PageOrder { get; set; }
        public int? ParentId { get; set; }
        public string Environment { get; set; }
    }
}
