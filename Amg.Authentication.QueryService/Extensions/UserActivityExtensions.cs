using Amg.Authentication.DomainModel.Modules.UserActivities;
using Amg.Authentication.DomainModel.Modules.UserActivities.ValueObjects;
using Amg.Authentication.QueryModel.Dtos.Accounting;

namespace Amg.Authentication.QueryService.Extensions
{
    public static class UserActivityExtensions
    {
        public static UserActivityDto ToDto(this UserActivity userActivity)
        {
            return new UserActivityDto()
            {
                IsSuccess = userActivity.IsSuccess,
                DateTime = userActivity.ActivityDateTime,
                ClientInfo = userActivity.ClientInfo.ToDto(),
                ActivityType = userActivity.Activity.GetType().Name,
                Description = userActivity.Activity.GetDescription()
            };
        }

        public static ClientInfoDto ToDto(this ClientInfo clientInfo)
        {
            return new ClientInfoDto()
            {
                Agent = clientInfo.Agent,
                Device = clientInfo.Device,
                IP = clientInfo.IP,
                OS = clientInfo.OS,
            };
        }
    }
}
