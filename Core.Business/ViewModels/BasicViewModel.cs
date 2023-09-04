using System;
using Core.Business.ViewModels.Abstract;

namespace Core.Business.ViewModels
{
    public class BasicViewModel : IBasicViewModel
    {
        public int Id { get; set; }
        public bool IsEnabled { get; set; }
    }
}
