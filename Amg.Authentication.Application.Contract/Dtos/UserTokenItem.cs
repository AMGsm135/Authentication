using System;

namespace Amg.Authentication.Application.Contract.Dtos
{
    public class UserTokenItem
    {
        public Guid TokenId { get; set; }

        public Guid UserId { get; set; }
        
        public ClientInfo ClientInfo { get; set; }
    }
}
