using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entity.Abstract;

namespace Core.Entity.Entities
{
    public class CustomerLikeCourse : EntityBasic
    {
        public CustomerLikeCourse()
        {
        }
        public int CourseId { get; set; }
        public int CustomerId { get; set; }
        public bool IsLiked { get; set; }
    }
}
