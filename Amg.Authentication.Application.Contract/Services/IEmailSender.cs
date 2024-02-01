using System.Threading.Tasks;

namespace Amg.Authentication.Application.Contract.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string recipient, string subject, string message);
    }
}
