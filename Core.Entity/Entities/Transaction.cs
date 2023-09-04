using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entity.Abstract;

namespace Core.Entity.Entities
{
    public class Transaction : EntitySimple
    {
        public Transaction()
        {
        }

        public int ExpenseId { get; set; }
        public int CustomerId { get; set; }
        public int StatusTransactionId { get; set; }
        public string Note { get; set; }
        public string ImagePath { get; set; }
        public DateTime? DateActive { get; set; }
        public int? ChapterId { get; set; }
    }
}
