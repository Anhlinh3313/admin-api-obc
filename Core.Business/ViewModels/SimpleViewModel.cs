using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Core.Business.ViewModels.Abstract;
using Core.Business.ViewModels.Validators;
using Core.Data.Core.Utils;
using Core.Entity.Abstract;

namespace Core.Business.ViewModels
{
    public class SimpleViewModel<TViewModel,TModel> : IEntitySimple, IValidatableObject
	    where TViewModel : class, IEntitySimple, new()
		where TModel : class, IEntitySimple, new()
    {
        public int Id { get; set; }
        public string Code { get; set; }
		public string Name { get; set; }
		public string ConcurrencyStamp { get; set; }
        public bool IsEnabled { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			var validator = new GeneralAbstractValidator<SimpleViewModel<TViewModel, TModel>, TModel>(EntityUtil.GetUnitOfWork(validationContext));
			var result = validator.Validate(this);
			return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
		}
    }

	public class SimpleViewModel : IEntitySimple
	{
		public int Id { get; set; }
		public string Code { get; set; }
		public string Name { get; set; }
		public string ConcurrencyStamp { get; set; }
		public bool IsEnabled { get; set; }
	}
}
