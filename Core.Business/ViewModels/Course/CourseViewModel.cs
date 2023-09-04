using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;
using Core.Entity.Entities;

namespace Core.Business.ViewModels.Course
{
    public class CourseViewModel : IEntityBase
    {
        public CourseViewModel()
        {
            
        }
        public int Id { get; set; }
        public bool IsEnabled { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public TimeCourseMobile[] TimeCourses { get; set; }
        public string Objects { get; set; }
        public int NumberOfAttendees { get; set; }
        public string Form { get; set; }
        public bool Fee { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedWhen { get; set; }
    }
}
