using System.ComponentModel;

namespace Amg.Authentication.Shared.Enums
{
    /// <summary>
    /// نوع مشتری
    /// </summary>
    public enum PersonType
    {
        [Description("حقوقی")]
        Legal = 1,

        [Description("حقیقی")]
        Individual = 2,

    }
}
