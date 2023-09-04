using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Business.ViewModels.Course;
using Core.Entity.Abstract;
using Core.Entity.Entities;

namespace Core.Business.ViewModels.Event
{
    public class EventAndCourseViewModelLiked
    {
        public EventAndCourseViewModelLiked()
        {
            
        }
        public int RowNum { get; set; }
        public string FormType { get; set; }
        public int? EventId { get; set; }
        public string EventCode { get; set; }
        public string EventName { get; set; }
        public TimeEventMobile[] TimeEvents { get; set; }
        public int? EventType { get; set; }
        public string ShortDescription { get; set; }
        public int? CourseId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public TimeCourseMobile[] TimeCourses { get; set; }
        public int? CourseType { get; set; }
        public double? Assess { get; set; }
        public bool? Assessed { get; set; }
        public int? VideoId { get; set; }
        public string VideoCode { get; set; }
        public string VideoName { get; set; }
        public int? VideoType { get; set; }
        public TimeCourseMobile[] TimeVideos { get; set; }
        public string ImagePath { get; set; }
        public int? Scores { get; set; }
        public DateTime DateLiked { get; set; }
        public int Total { get; set; }
    }
}
