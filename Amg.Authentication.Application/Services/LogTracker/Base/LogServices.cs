using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amg.Authentication.Application.Services.LogTracker.Base
{
    public abstract class LogServices<LogCategory>
    {
        private readonly ILogger<LogCategory> _logger;
        private readonly IServiceProvider _serviceProvider;
        public LogServices(ILogger<LogCategory> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected static string GetExceptionMessage(Exception ex)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (ex != null)
            {
                stringBuilder.Append(ex.Message);
            }

            Exception ex2 = ex?.InnerException;
            int num = 1;
            while (ex2 != null)
            {
                stringBuilder.Append(" ,InnerException" + num + " Message:" + ex2.Message);
                ex2 = ex2.InnerException;
                num++;
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// ثبت متد پایه لاگ برای لاگ های غیر از عملیات کراد که در مواقع استفاده مستقیم از این کد باید لاگ مسیج را به صورت کامل و تا جایی که ممکن هست با جزییات وارد کنیم 
        /// </summary>
        /// <typeparam name="Model"></typeparam>
        /// <param name="eventId">LogEvent</param>
        /// <param name="logMessage">Message Of Log </param>
        /// <param name="LogType">LogType</param>
        protected void Log(EventId? eventId, string logMessage, LogEnumType LogType)
        {
            logMessage = logMessage.Replace("{", "[");
            logMessage = logMessage.Replace("}", "]");

            if (LogType == LogEnumType.ErrorLog)
            {
                if (!eventId.HasValue)
                {
                    _logger.LogError(logMessage + "logType:{LogType}", LogType);
                }
                else
                {
                    _logger.LogError(eventId.Value, logMessage + " ,EventID:{eventId},LogType:{LogType}", eventId, LogType);
                }
            }
            else if (!eventId.HasValue)
            {
                _logger.LogInformation(logMessage + " ,LogType:{LogType}", LogType);
            }
            else
            {
                _logger.LogInformation(eventId.Value, logMessage + " ,EventID:{eventId},LogType:{LogType}", eventId.Value, LogType);
            }
        }

        /// <summary>
        /// ثبت متد پایه لاگ برای لاگ های غیر از عملیات کراد که در مواقع استفاده مستقیم از این کد باید لاگ مسیج را به صورت کامل و تا جایی که ممکن هست با جزییات وارد کنیم Generic Log for different Domain Model
        /// </summary>
        /// <typeparam name="Model"></typeparam>
        /// <param name="eventId">LogEvent</param>
        /// <param name="logMessage">Message Of Log </param>
        /// <param name="LogType">LogType</param>
        protected void Log<Model>(EventId? eventId, string logMessage, LogEnumType LogType)
        {
            var _loggerModel = _serviceProvider.GetService(typeof(ILogger<Model>));
            var T = (ILogger<Model>)_loggerModel;
            logMessage = logMessage.Replace("{", "[");
            logMessage = logMessage.Replace("}", "]");

            if (LogType == LogEnumType.ErrorLog)
            {
                if (!eventId.HasValue)
                {
                    T.LogError(logMessage + "logType:{LogType}", LogType);
                }
                else
                {
                    T.LogError(eventId.Value, logMessage + " ,EventID:{eventId},LogType:{LogType}", eventId, LogType);
                }
            }
            else if (!eventId.HasValue)
            {
                T.LogInformation(logMessage + " ,LogType:{LogType}", LogType);
            }
            else
            {
                T.LogInformation(eventId.Value, logMessage + " ,EventID:{eventId},LogType:{LogType}", eventId, LogType);
            }
        }
    }

    public enum LogEnumType
    {
        SuccessLog =1,
        ErrorLog =2,
        UnknownStateLog = 3
    }
}
