using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;
using Microsoft.AspNetCore.Http;

namespace Core.Business.ViewModels.Transaction
{
    public class TransactionViewModelCreate
    {
        public TransactionViewModelCreate()
        {
            
        }
        public IFormFile File { get; set; }
        public int ExpenseId { get; set; }
    }
}
