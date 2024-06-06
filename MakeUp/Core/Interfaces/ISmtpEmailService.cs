using Core.Models.Account;

namespace Core.Interfaces
{
    public interface ISmtpEmailService
    {
        void Send(Message message);
    }
}
