using System;

namespace Amg.Authentication.Application.Events
{
    public interface IIntegrationEvent
    {
        Guid EventId { get; set; }

    }
}
