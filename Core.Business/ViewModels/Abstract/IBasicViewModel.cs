using System;
namespace Core.Business.ViewModels.Abstract
{
    public interface IBasicViewModel : IBaseViewModel
    {
		int Id { get; set; }
        bool IsEnabled { get; set; }
    }
}
