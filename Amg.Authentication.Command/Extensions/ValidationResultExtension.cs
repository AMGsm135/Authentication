using System.Linq;
using FluentValidation;
using FluentValidation.Results;

namespace Amg.Authentication.Command.Extensions
{
    public static class ValidationResultExtension
    {
        public static void RaiseExceptionIfRequired(this ValidationResult validationResult)
        {
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors.First().ErrorMessage);
        }
    }
}
