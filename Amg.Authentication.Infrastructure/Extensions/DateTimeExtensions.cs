using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amg.Authentication.Infrastructure.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ConvertToTimestamp(this TimeSpan timeSpan)
        {
            return DateTimeOffset.UtcNow.Add(timeSpan).ToUnixTimeSeconds().ToString();
        }
    }
}
