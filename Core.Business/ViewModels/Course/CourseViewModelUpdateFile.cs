using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;
using Microsoft.AspNetCore.Http;

namespace Core.Business.ViewModels.Course
{
    public class CourseViewModelUpdateFile
    {
        public CourseViewModelUpdateFile()
        {
            
        }
        public int Id { get; set; }
        public string[] ImagePath { get; set; }
        public string[] VideoPath { get; set; }
    }
}
