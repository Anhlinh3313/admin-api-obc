using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entity.Abstract;

namespace Core.Entity.Entities
{
    public class FaceToFace : EntityBasic
    {
        public FaceToFace()
        {
        }
        public int CustomerId { get; set; }
        public int ReceiverId { get; set; }
        public string Type { get; set; }
        public string Location { get; set; }
        public DateTime ExchangeTime { get; set; }
        public string Description { get; set; }
        public int StatusFaceToFaceId { get; set; }
        public string ReasonCancel { get; set; }
        public string ImagePathReceive { get; set; }
        public string ImagePathGive { get; set; }
    }
}
