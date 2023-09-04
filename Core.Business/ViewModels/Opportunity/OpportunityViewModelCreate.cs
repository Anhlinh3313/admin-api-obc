using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;

namespace Core.Business.ViewModels.Opportunity
{
    public class OpportunityViewModelCreate : IEntityBase
    {
        public OpportunityViewModelCreate()
        {
            
        }
        public int Id { get; set; }
        public bool IsEnabled { get; set; }
        public int ReceiverId { get; set; }
        public string Type { get; set; }
        public string Information { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public int Level { get; set; }
        public int StatusOpportunityId { get; set; }
    }
    public class StatusOpportunityModel
    {
        public StatusOpportunityModel()
        {

        }
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
