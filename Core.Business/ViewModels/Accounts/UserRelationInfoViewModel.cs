using Core.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Business.ViewModels
{
    public class UserRelationInfoViewModel : EntitySimple
    {
        public UserRelationInfoViewModel() { }

        public int UserId { get; set; }
        public int UserRelationId { get; set; }
        
        public Entity.Entities.User User { get; set; }
        public Entity.Entities.User UserRelationUser { get; set; }

    }
}
