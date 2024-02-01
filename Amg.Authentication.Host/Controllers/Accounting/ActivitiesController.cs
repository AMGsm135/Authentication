using System;
using System.Linq;
using System.Threading.Tasks;
using Amg.Authentication.Application.Contract;
using Amg.Authentication.Host.SeedWorks;
using Amg.Authentication.QueryModel.Services.Accounting;
using Gridify;
using Microsoft.AspNetCore.Mvc;

namespace Amg.Authentication.Host.Controllers.Accounting
{
    [Route(Constants.ApiPrefix + "/v1/[controller]")]
    public class ActivitiesController : ApiControllerBase
    {
        private readonly IUserActivityQueryService _userActivityQueryService;

        public ActivitiesController(IUserActivityQueryService userActivityQueryService)
        {
            _userActivityQueryService = userActivityQueryService;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetUserActivities([FromQuery] GridifyQuery query)
        {
            var activities = await _userActivityQueryService.GetUserActivitiesByUser(UserInfo.UserId, query);
            return OkResult(activities);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetActivitiesByAdmin(Guid userId, [FromQuery] GridifyQuery query)
        {
            var activities = await _userActivityQueryService.GetUserActivitiesByUser(userId, query);
            return OkResult(activities);
        }
    }
}
