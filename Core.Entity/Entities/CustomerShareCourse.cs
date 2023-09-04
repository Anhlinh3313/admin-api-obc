using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entity.Abstract;

namespace Core.Entity.Entities
{
    public class CustomerShareCourse : IEntityBase
    {
        public CustomerShareCourse()
        {
        }
        public int Id { get; set; }
        public bool IsEnabled { get; set; }
        public int CourseId { get; set; }
        public int CustomerId { get; set; }
        public bool IsShared { get; set; }
    }
}
