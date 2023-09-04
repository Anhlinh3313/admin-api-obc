using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entity.Abstract;

namespace Core.Entity.Entities
{
    public class TimeCourse : IEntityBase
    {
        public TimeCourse()
        {
        }
        public int Id { get; set; }
        public bool IsEnabled { get; set; }
        public int CourseId { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
    }
}
