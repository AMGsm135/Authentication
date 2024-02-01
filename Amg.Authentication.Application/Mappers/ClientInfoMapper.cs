
using Amg.Authentication.Application.Contract.Dtos;

namespace Amg.Authentication.Application.Mappers
{
    public static class ClientInfoMapper
    {
        public static Application.Events.UserActivities.Items.ClientInfo ToEvent(
            this ClientInfo clientInfo)
        {
            if (clientInfo == null)
                return null;

            return new Application.Events.UserActivities.Items.ClientInfo()
            {
                Agent = clientInfo.Agent,
                Device = clientInfo.Device,
                IP = clientInfo.IP,
                OS = clientInfo.OS
            };
        }
    }
}