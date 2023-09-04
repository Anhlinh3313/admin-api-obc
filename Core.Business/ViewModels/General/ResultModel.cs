using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Core.Business.ViewModels.Validators;
using Core.Data.Core.Utils;
using Core.Entity.Abstract;
using Core.Entity.Entities;
using Core.Infrastructure.Enum;

namespace Core.Business.ViewModels.General
{
    public class ResultModel
    {
        public StatusResponse isSuccess { get; set; }
        public string message { get; set; }
        public int? dataCount { get; set; }
        public dynamic data { get; set; }
    }
}
