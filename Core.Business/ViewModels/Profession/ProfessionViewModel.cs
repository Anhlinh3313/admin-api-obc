using System.Collections.Generic;
using Core.Entity.Abstract;

namespace Core.Business.ViewModels.Profession
{
    public class ProfessionViewModel : IEntityBase
    {
        public ProfessionViewModel()
        {
            
        }
        public int Id { get; set; }
        public bool IsEnabled { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public List<Entity.Entities.FieldOperations> FieldOperations { get; set; }
    }

    public class ProfessionModel
    {
        public ProfessionModel()
        {

        }
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
