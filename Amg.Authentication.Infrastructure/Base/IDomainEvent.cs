using System;

namespace Amg.Authentication.Infrastructure.Base
{
    public interface IDomainEvent
    {
        Guid EventId { get; set; }

    }
}
