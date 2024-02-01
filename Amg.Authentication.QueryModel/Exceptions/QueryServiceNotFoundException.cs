using System;

namespace Amg.Authentication.QueryModel.Exceptions
{
    public class QueryServiceNotFoundException : Exception
    {
        public QueryServiceNotFoundException(string message) : base(message)
        {
        }

        public QueryServiceNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
