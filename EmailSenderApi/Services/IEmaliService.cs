using EmailSenderApi.Models.Input;

namespace EmailSenderApi.Services
{
    public interface IEmaliService
    {
        Task SendEmail(EmailRequest emailRequest);
    }
}
