using EmailSenderApi.Models.Input;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace EmailSenderApi.Services
{
    public class EmailService : IEmaliService
    {
        private readonly IConfiguration _config;
        public EmailService(IConfiguration config)
        {
            _config = config;
        }
        public async Task SendEmail(EmailRequest emailRequest)
        {
            var smtpServer = _config["EmailSettings:SmtpServer"];
            var port = int.Parse(_config["EmailSettings:Port"]);
            var username = _config["EmailSettings:Username"];
            var password = Environment.GetEnvironmentVariable("EMAIL_SENDER_API_PWD")
                   ?? throw new Exception("EMAIL_PASSWORD environment variable not set.");
            var enableSsl = bool.Parse(_config["EmailSettings:EnableSsl"]);
            // Check if the body is provided, otherwise use the template
            string emailBody = !string.IsNullOrEmpty(emailRequest.Body)
                ? emailRequest.Body
                : LoadTemplate(emailRequest.TemplateName, emailRequest.TemplateValues);

            if (string.IsNullOrEmpty(emailBody))
            {
                throw new Exception("Email body is empty. Please provide a valid body or template.");
            }

            //crating and instance of SmptClient class
            var smtpClient = new SmtpClient(smtpServer)
            {
                Port = port,
                Credentials = new NetworkCredential(username, password),
                EnableSsl = enableSsl
            };

            //MailMessage is a class in System.Net.Mail that represents an email message in C#.
            //It holds details like sender, recipient(s), subject, body, and attachments.
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_config["EmailSettings:Username"]),
                Subject = emailRequest.Subject,
                Body = emailBody,
                IsBodyHtml = true
            };

            mailMessage.To.Add(emailRequest.To);
            await smtpClient.SendMailAsync(mailMessage);
        }

        private string LoadTemplate(string? templateName, Dictionary<string, string>? values) {

            if (string.IsNullOrEmpty(templateName))
            {
                throw new ArgumentException("Template name is required.", nameof(templateName));
            }

            string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", $"{templateName}.html");

            if (!File.Exists(templatePath)) throw new Exception($"Template {templateName} not found.");

            string templateContent = File.ReadAllText(templatePath);

            if (values != null && values.Count > 0)
            {
                foreach (var entry in values)
                {
                    // Only replace if the value is not null or empty
                    if (!string.IsNullOrEmpty(entry.Value))
                    {
                        templateContent = Regex.Replace(templateContent, $"{{{{{entry.Key}}}}}", entry.Value, RegexOptions.IgnoreCase);
                    }
                    else
                    {
                        // If the value is empty or null, optionally log it or set a default value
                        // For now, we are just logging the key that has no value
                        Console.WriteLine($"Warning: Missing value for key '{entry.Key}' in template.");
                    }
                }
            }

            return templateContent;
        }
    }
}
