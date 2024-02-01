using System;

namespace Amg.Authentication.Infrastructure.Helpers
{
    public static class CodeGenerator
    {
        public static string Generate()
        {
            return new Random().Next(10000, 99999).ToString();
        }
    }
}
