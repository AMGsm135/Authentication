using System.ComponentModel;


namespace Amg.Authentication.Infrastructure.Enums
{
    public enum RegisteryStatus
    {
        [Description("")] None,
        [Description("ثبت نام شده")] Created,
        [Description("تایید شده")] Accepted,
        [Description("رد شده")] Rejected,
    }
}
