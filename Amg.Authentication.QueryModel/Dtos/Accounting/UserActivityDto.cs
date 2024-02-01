using System;

namespace Amg.Authentication.QueryModel.Dtos.Accounting
{
    public class UserActivityDto
    {
        public string ActivityType { get; set; }

        public string Description { get; set; }
        
        public bool IsSuccess { get; set; }
        
        public ClientInfoDto ClientInfo { get; set; }
        
        public DateTime DateTime { get; set; }
    }
}
