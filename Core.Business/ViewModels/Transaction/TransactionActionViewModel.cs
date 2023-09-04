using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;
using Microsoft.AspNetCore.Http;

namespace Core.Business.ViewModels.Transaction
{
    public class TransactionActionViewModel
    {
        public TransactionActionViewModel()
        {
            
        }
        public int ActionId { get; set; }
        public string ActionName { get; set; }
    }
}
