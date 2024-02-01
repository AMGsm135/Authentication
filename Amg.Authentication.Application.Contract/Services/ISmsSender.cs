using System.Threading.Tasks;

namespace Amg.Authentication.Application.Contract.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string phoneNumber, string message);
        Task<(bool isSuccess, string message)> SendSmsAsyncWithResult(string phoneNumber, string message);
    }


}
