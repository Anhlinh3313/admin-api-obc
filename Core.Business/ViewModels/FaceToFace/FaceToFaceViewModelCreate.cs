using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;

namespace Core.Business.ViewModels.FaceToFace
{
    public class FaceToFaceViewModelCreate : IEntityBase
    {
        public FaceToFaceViewModelCreate()
        {
            
        }
        public int Id { get; set; }
        public bool IsEnabled { get; set; }
        public int ReceiverId { get; set; }
        public string ReceiverName { get; set; }
        public string Type { get; set; }
        public string Location { get; set; }
        public DateTime ExchangeTime { get; set; }
        public string Description { get; set; }
    }
}
