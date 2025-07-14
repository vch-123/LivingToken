using System.Net.Mail;
using System.Net;
using System.Linq.Expressions;

namespace WebApplication1.Service
{
    public class EmailService
    {
        private readonly SmtpClient _client;


        public EmailService(IConfiguration configuration)
        {
            _client = new SmtpClient
            {
                Host = configuration["Smtp:Host"],
                Port = int.Parse(configuration["Smtp:Port"]),
                EnableSsl = bool.Parse(configuration["Smtp:EnableSsl"]),
                Credentials = new NetworkCredential(configuration["Smtp:Username"], configuration["Smtp:Password"])
            };
        }

        public async Task SendEmailAsync(string sender, string receiver, string subject, string message, List<IFormFile> attachments)
        {
            try
            {
                var mailMessage = new MailMessage(sender, receiver, subject, message);

                // 存储流的列表，用于在邮件发送后关闭它们
                var streamsToDispose = new List<Stream>();

                if (attachments != null)
                {
                    foreach (var attachment in attachments)
                    {
                        var stream = new MemoryStream();
                        await attachment.CopyToAsync(stream);
                        // 重置流的位置
                        stream.Position = 0;
                        mailMessage.Attachments.Add(new Attachment(stream, attachment.FileName));
                        // 将流添加到待关闭列表
                        streamsToDispose.Add(stream);
                    }
                }

                try
                {
                    await _client.SendMailAsync(mailMessage);
                }
                finally
                {
                    // 发送完成后，关闭所有流
                    foreach (var stream in streamsToDispose)
                    {
                        stream.Dispose();
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            }

    }

}
