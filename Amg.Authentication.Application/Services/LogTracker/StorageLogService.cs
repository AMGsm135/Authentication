using Amg.Authentication.Application.Services.LogTracker.Base;
using Amg.Authentication.Infrastructure.Base;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amg.Authentication.Application.Services.LogTracker
{
    public class StorageLogService<DomainModel, PrimaryKeyType> : LogServices<DomainModel>
        where PrimaryKeyType : struct
        where DomainModel : ICommandBase
    {
        protected readonly int _logBaseID;
        public StorageLogService(ILogger<DomainModel> logger,IServiceProvider serviceProvider, int logBaseID) : base(logger, serviceProvider)
        {
            _logBaseID = logBaseID;
        }

        /// <summary>
        /// در صورتی که هر دو خروجی خالی باشد به معنای ثیت لاگ موفقیت امیز میباشد اما اگر هر یک از ورودی ها پر شود به معنای ثبت لاگ خطا میباشد
        /// </summary>
        /// <typeparam name="Model"></typeparam>
        /// <param name="objectDescriptor">وقتی توضیح اضافی در خطای لاگ مربوطه یا وقتی اکسپشن رخ نداده اما به هر دلیلی لاگ از جنس اررور میباد این قسمت پر شود</param>
        /// <param name="ex">وقتی اکسپشن وجود دارد در این قسمت قرار داده شود</param>
        protected void LogAdd(string objectDescriptor = "", Exception? ex = null)
        {
            LogEnumType logType = LogEnumType.SuccessLog;
            string text = "";
            if (ex != null)
            {
                text = "Error in adding " + typeof(DomainModel).Name + " ,with exception: " + LogServices<DomainModel>.GetExceptionMessage(ex);
                logType = LogEnumType.ErrorLog;
            }
            else
            {
                text = text + "Successfully Adding " + typeof(DomainModel).Name;
                //text = text + " ,ID:" + model.ID;
            }
            if (!string.IsNullOrWhiteSpace(objectDescriptor))
            {
                text = text + " ,ExtraDescription From Service Layer : " + objectDescriptor;
            }
            Log(new EventId(_logBaseID + 1, "Add"), text, logType);
        }

        /// <summary>
        /// در صورتی که هر دو خروجی خالی باشد به معنای ثیت لاگ موفقیت امیز میباشد اما اگر هر یک از ورودی ها پر شود به معنای ثبت لاگ خطا میباشد Generic Log for different Domain Model
        /// </summary>
        /// <typeparam name="Model"></typeparam>
        /// <param name="objectDescriptor">وقتی توضیح اضافی در خطای لاگ مربوطه یا وقتی اکسپشن رخ نداده انا به هز دلیلی لاگ از جنس اررور میباد این قسمت پر شود</param>
        /// <param name="ex">وقتی اکسپشن وجود دارد در این قسمت قرار داده شود</param>
        protected void LogAdd<Model>(string objectDescriptor = "", Exception? ex = null)
        {
            LogEnumType logType = LogEnumType.SuccessLog;
            string text = "";
            if (ex != null || !string.IsNullOrEmpty(objectDescriptor))
            {
                text = "Error in adding " + typeof(Model).Name + " ,with exception: " + LogServices<Model>.GetExceptionMessage(ex);
                logType = LogEnumType.ErrorLog;
            }
            else
            {
                text = text + "Successfully Adding " + typeof(Model).Name;
                //text = text + " ,ID:" + model.ID;
            }
            if (!string.IsNullOrWhiteSpace(objectDescriptor))
            {
                text = text + " ,ExtraDescription From Service Layer : " + objectDescriptor;
            }
            Log<Model>(new EventId(_logBaseID + 1, "Add"), text, logType);
        }

        /// <summary>
        /// در صورتی که ورودی های دوم و سوم خالی باشد به معنای ثیت لاگ موفقیت امیز میباشد اما اگر هر یک از ورودی های دوم و سوم پر شود به معنای ثبت لاگ خطا میباشد
        /// </summary>
        /// <param name="itemID">ایدی انتیتی که قرار است ویرایش بر روی ان انجام شود</param>
        /// <param name="objectDescriptor">وقتی توضیح اضافی در خطای لاگ مربوطه وجود دارد یا وقتی اکسپشن رخ نداده اما به هر دلیلی لاگ از جنس اررور میباد این قسمت پر شود</param>
        /// <param name="ex">وقتی اکسپشن وجود دارد در این قسمت قرار داده شود</param>
        protected void LogModify(PrimaryKeyType? itemID = null, string? objectDescriptor = null, Exception? ex = null)
        {
            LogEnumType logType = LogEnumType.SuccessLog;
            string text = " ";
            if (ex != null || !string.IsNullOrEmpty(objectDescriptor))
            {
                text = text + "Error in updating " + typeof(DomainModel).Name + "With Exception : " + LogServices<DomainModel>.GetExceptionMessage(ex);
                logType = LogEnumType.ErrorLog;
            }
            else
            text = text + "Successfully updating " + typeof(DomainModel).Name;

            if(itemID != null)
            {
                text = text + " ,DomainModel ID:" + itemID.ToString();
            }

            if (!string.IsNullOrWhiteSpace(objectDescriptor))
            {
                text = text + " ,ExtraDescription From Service Layer : " + objectDescriptor;
            }
            Log(new EventId(_logBaseID + 2, "Modify"), text, logType);
        }

        /// <summary>
        /// در صورتی که ورودی های دوم و سوم خالی باشد به معنای ثیت لاگ موفقیت امیز میباشد اما اگر هر یک از ورودی های دوم و سوم پر شود به معنای ثبت لاگ خطا میباشد Generic Log for different Domain Model
        /// </summary>
        /// <param name="itemID">ایدی انتیتی که قرار است ویرایش بر روی ان انجام شود</param>
        /// <param name="objectDescriptor">وقتی توضیح اضافی در خطای لاگ مربوطه وجود دارد یا وقتی اکسپشن رخ نداده اما به هر دلیلی لاگ از جنس اررور میباد این قسمت پر شود</param>
        /// <param name="ex">وقتی اکسپشن وجود دارد در این قسمت قرار داده شود</param>
        protected void LogModify<Model>(PrimaryKeyType? itemID = null, string? objectDescriptor = null, Exception? ex = null)
        {
            LogEnumType logType = LogEnumType.SuccessLog;
            string text = " ";
            if (ex != null || !string.IsNullOrEmpty(objectDescriptor))
            {
                text = text + "Error in updating " + typeof(Model).Name + "With Exception : " + LogServices<Model>.GetExceptionMessage(ex);
                logType = LogEnumType.ErrorLog;
            }
            else
                text = text + "Successfully updating " + typeof(Model).Name;

            if (itemID != null)
            {
                text = text + " ,DomainModel ID:" + itemID.ToString();
            }

            if (!string.IsNullOrWhiteSpace(objectDescriptor))
            {
                text = text + " ,ExtraDescription From Service Layer : " + objectDescriptor;
            }
            Log<Model>(new EventId(_logBaseID + 2, "Modify"), text, logType);
        }

        /// <summary>
        /// در صورتی که ورودی های دوم و سوم خالی باشد به معنای ثیت لاگ موفقیت امیز میباشد اما اگر هر یک از ورودی های دوم و سوم پر شود به معنای ثبت لاگ خطا میباشد 
        /// </summary>
        /// <param name="itemID">ایدی انتیتی که قرار است حدف بر روی ان انجام شود</param>
        /// <param name="objectDescriptor">وقتی توضیح اضافی در خطای لاگ مربوطه وجود دارد یا وقتی اکسپشن رخ نداده اما به هر دلیلی لاگ از جنس اررور میباد این قسمت پر شود</param>
        /// <param name="ex">وقتی اکسپشن وجود دارد در این قسمت قرار داده شود</param>
        protected void LogRemove(PrimaryKeyType itemID, string objectDescriptor = null, Exception ex = null)
        {
            LogEnumType logType = LogEnumType.SuccessLog;
            string text = "";
            if (ex != null || !string.IsNullOrEmpty(objectDescriptor))
            {
                text = "Error in removing " + typeof(DomainModel).Name + " With Exception : " + LogServices<DomainModel>.GetExceptionMessage(ex);
                logType = LogEnumType.ErrorLog;
            }
            else
                text = text + "Successfully removing " + typeof(DomainModel).Name;

            text = text + " ,DomainModel ID:" + itemID;
            if (!string.IsNullOrWhiteSpace(objectDescriptor))
            {
                text = text + " ," + objectDescriptor;
            }
            Log(new EventId(_logBaseID + 3, "Remove"), text, logType);
        }

        /// <summary>
        /// در صورتی که ورودی های دوم و سوم خالی باشد به معنای ثیت لاگ موفقیت امیز میباشد اما اگر هر یک از ورودی های دوم و سوم پر شود به معنای ثبت لاگ خطا میباشد Generic Log for different Domain Model
        /// </summary>
        /// <param name="itemID">ایدی انتیتی که قرار است حدف بر روی ان انجام شود</param>
        /// <param name="objectDescriptor">وقتی توضیح اضافی در خطای لاگ مربوطه وجود دارد یا وقتی اکسپشن رخ نداده اما به هر دلیلی لاگ از جنس اررور میباد این قسمت پر شود</param>
        /// <param name="ex">وقتی اکسپشن وجود دارد در این قسمت قرار داده شود</param>
        protected void LogRemove<Model>(PrimaryKeyType itemID, string objectDescriptor = null, Exception ex = null)
        {
            LogEnumType logType = LogEnumType.SuccessLog;
            string text = "";
            if (ex != null || !string.IsNullOrEmpty(objectDescriptor))
            {
                text = "Error in removing " + typeof(Model).Name + " With Exception : " + LogServices<Model>.GetExceptionMessage(ex);
                logType = LogEnumType.ErrorLog;
            }
            else
                text = text + "Successfully removing " + typeof(Model).Name;

            text = text + " ,DomainModel ID:" + itemID;
            if (!string.IsNullOrWhiteSpace(objectDescriptor))
            {
                text = text + " ," + objectDescriptor;
            }
            Log<Model>(new EventId(_logBaseID + 3, "Remove"), text, logType);
        }

        protected void LogRetrieveMultiple(Dictionary<string, object> parameters = null, Exception ex = null)
        {
            LogEnumType logType = LogEnumType.SuccessLog;
            StringBuilder stringBuilder = new StringBuilder();
            if (ex == null)
            {
                stringBuilder.Append("Retrieve Multiple " + typeof(DomainModel).Name);
            }
            else
            {
                stringBuilder.Append("Error in Retrieve Multiple " + typeof(DomainModel).Name);
                logType = LogEnumType.ErrorLog;
            }

            if (parameters != null)
            {
                stringBuilder.Append(" ,with parameters::");
                foreach (KeyValuePair<string, object> parameter in parameters)
                {
                    stringBuilder.Append(" ," + parameter.Key + ":" + parameter.Value);
                }
            }

            Log(_logBaseID + 4, stringBuilder.ToString(), logType);
        }

        protected void LogRetrieveSingle(PrimaryKeyType requestedID, Exception? ex = null)
        {
            LogEnumType logType = LogEnumType.SuccessLog;
            string text = "";
            if (ex != null)
            {
                text = "Error in Retrieve Single " + typeof(DomainModel).Name;
                logType = LogEnumType.ErrorLog;
            }
            else
            {
                text = text + "Retrieve Single " + typeof(DomainModel).Name;
            }

            text = text + " ,ID:" + requestedID;
            Log(new EventId(_logBaseID + 5, "RetrieveSingle"), text, logType);
        }
    }
}
