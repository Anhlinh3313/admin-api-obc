using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;
using Microsoft.AspNetCore.Http;

namespace Core.Business.ViewModels.Course
{
    public class CourseViewModelRegister
    {
        public CourseViewModelRegister()
        {
            
        }
        public int CustomerCourseId { get; set; }
        public int TransactionCourseId { get; set; }
        public string ImagePath { get; set; }
    }
}
