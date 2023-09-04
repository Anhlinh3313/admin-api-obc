using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;

namespace Core.Business.ViewModels.FieldOperations
{
    public class FieldOperationsViewModel
    {
        public FieldOperationsViewModel()
        {
            
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProfessionId { get; set; }
    }
}
