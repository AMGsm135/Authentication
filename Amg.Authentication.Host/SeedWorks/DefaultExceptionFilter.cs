using System.ComponentModel.DataAnnotations;
using Amg.Authentication.Application.Contract.Exceptions;
using Amg.Authentication.DomainModel.SeedWorks.Exceptions;
using Amg.Authentication.Host.Exceptions;
using Amg.Authentication.QueryModel.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Amg.Authentication.Host.SeedWorks
{
    public class DefaultExceptionFilter : IExceptionFilter
    {

        public void OnException(ExceptionContext context)
        {
            
            IActionResult result;      
            switch (context.Exception.GetType().Name)
            {
                case nameof(ApiException):
                    result = new ObjectResult(new ResponseMessage(context.Exception.Message)) { StatusCode = 400 };
                    break;
                case nameof(ValidationException):
                    result = new ObjectResult(new ResponseMessage(context.Exception.Message)) { StatusCode = 400 };
                    break;
                case nameof(DomainException):
                    result = new ObjectResult(new ResponseMessage(context.Exception.Message)) { StatusCode = 400 };
                    break;
                case nameof(ServiceException):
                    result = new ObjectResult(new ResponseMessage(context.Exception.Message)) { StatusCode = 400 };
                    break;
                case nameof(QueryServiceNotFoundException):
                    result = new ObjectResult(new ResponseMessage(context.Exception.Message)) { StatusCode = 404 };
                    break;
                default:             
                    result = new ObjectResult(new ResponseMessage(context.Exception.Message)) { StatusCode = 500 };
                    break;
            }

            context.Result = result;
            context.ExceptionHandled = true;
        }
    }
}
