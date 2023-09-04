using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;
using Microsoft.AspNetCore.Http;

namespace Core.Business.ViewModels.Course
{
    public class CourseViewModelStatus
    {
        public CourseViewModelStatus()
        {
            
        }
        public int StatusId { get; set; }
        public string StatusName { get; set; }
    }
}
