using System;
using System.Collections.Generic;
using Amg.Authentication.Infrastructure.Enums;

namespace Amg.Authentication.Infrastructure.Helpers
{
    public class RolesParser
    {
        public static List<RoleType> ToRoleTypes(IList<string> roleStrings)
        {
            var result = new List<RoleType>();
            foreach (var roleString in roleStrings)
            {
                if (Enum.TryParse(typeof(RoleType), roleString, out var role))
                    result.Add((RoleType)role);
            }

            return result;
        }
    }
}
