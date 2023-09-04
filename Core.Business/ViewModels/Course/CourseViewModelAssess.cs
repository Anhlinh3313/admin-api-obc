using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;
using Core.Entity.Procedures;

namespace Core.Business.ViewModels.Course
{
    public class CourseViewModelAssess
    {
        public CourseViewModelAssess()
        {
            
        }
        public List<Proc_GetListAssess> Assess { get; set; }
        public float Sum { get; set; }
    }
}
