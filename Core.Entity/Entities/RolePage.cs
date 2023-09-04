using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entity.Abstract;

namespace Core.Entity.Entities
{
    public class RolePage : IEntityBase
    {
        public RolePage()
        {
        }
        public int Id { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsView { get; set; }
        public bool IsCreate { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public int PageId { get; set; }
        public int RoleId { get; set; }
    }
}
