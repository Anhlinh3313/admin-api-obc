using Core.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Business.ViewModels
{
    public class UserRelationViewModel : EntitySimple
    {
        public UserRelationViewModel() { }

        public int UserId { get; set; }
        public int UserRelationId { get; set; }
    }
}
