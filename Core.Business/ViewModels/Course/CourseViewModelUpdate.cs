using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;
using Microsoft.AspNetCore.Http;

namespace Core.Business.ViewModels.Course
{
    public class CourseViewModelUpdate
    {
        public CourseViewModelUpdate()
        {
            
        }
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public TimeCourseModel[] TimeCourses { get; set; }
        public string Objects { get; set; }
        public bool Fee { get; set; }
        public string Form { get; set; }
        public int Scores { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string[] ImagePath { get; set; }
        public string[] VideoPath { get; set; }
    }
    public class TimeCourseModel
    {
        public TimeCourseModel()
        {

        }
        public string DateStart { get; set; }
        public string DateEnd { get; set; }
        public string TimeStart { get; set; }
        public string TimeEnd { get; set; }
    }
}
