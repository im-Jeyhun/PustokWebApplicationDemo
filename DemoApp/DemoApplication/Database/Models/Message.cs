using MimeKit;

namespace DemoApplication.Database.Models
{
    public class Message
    {
        public List<MailboxAddress> To { get; set; }

        public string Subject { get; set; }

        public string Content { get; set; }
        public Message(List<string> to, string subject, string content)
        {
            To = new List<MailboxAddress>();

            To.AddRange(to.Select(t => new MailboxAddress(string.Empty, t)));


            Subject = subject;
            Content = content;
        }
    }
}
