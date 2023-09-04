using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;
using Microsoft.AspNetCore.Http;

namespace Core.Business.ViewModels.Course
{
    public class CourseViewModelCreate
    {
        public CourseViewModelCreate()
        {
            
        }
        public string Name { get; set; }
        public string Objects { get; set; }
        public TimeCourseModel[] TimeCourses { get; set; }
        public bool Fee { get; set; }
        public string Form { get; set; }
        public int Scores { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
    }
}
