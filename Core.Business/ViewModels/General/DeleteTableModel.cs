using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Core.Business.ViewModels.Validators;
using Core.Data.Core.Utils;
using Core.Entity.Abstract;
using Core.Entity.Entities;

namespace Core.Business.ViewModels.General
{
    public class DeleteTableModel
    {
        public string TableName { get; set; }
        public int Id { get; set; }
        public int? UserId { get; set; }
    }
}
