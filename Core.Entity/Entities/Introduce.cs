using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entity.Abstract;

namespace Core.Entity.Entities
{
    public class Introduce : IEntityBase
    {
        public Introduce()
        {
        }
        public int Id { get; set; }
        public bool IsEnabled { get; set; }
        public string Content { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Website { get; set; }
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string Instagram { get; set; }
        public string Youtube { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string Bank { get; set; }
        public string BankBranch { get; set; }
        public string LinkGroupChat { get; set; }
    }
}
