using DemoApplication.Contracts.Email;
using DemoApplication.Database.Models;

namespace DemoApplication.Services.Abstracts
{
    public interface IEmailService
    {
        public void Send(Message message);
    }
}
