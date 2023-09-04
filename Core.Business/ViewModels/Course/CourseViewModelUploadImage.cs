using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;
using Microsoft.AspNetCore.Http;

namespace Core.Business.ViewModels.Course
{
    public class CourseViewModelUploadImage
    {
        public CourseViewModelUploadImage()
        {
            
        }
        public int CourseId { get; set; }
        public List<IFormFile> Files { get; set; }
    }
}
