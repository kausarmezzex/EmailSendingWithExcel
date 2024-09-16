using System.IO;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ClosedXML.Excel;
using EmailSendingWithExcel.Data;
using EmailSendingWithExcel.Models;

namespace EmailSendingWithExcel.Controllers
{
    public class EmailController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SmtpSettings _smtpSettings;

        public EmailController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            // Bind SMTP settings from appsettings.json
            _smtpSettings = configuration.GetSection("Smtp").Get<SmtpSettings>();
        }

        public IActionResult Index()
        {
            var templates = _context.EmailTemplates.ToList();

            var model = new EmailSenderViewModel
            {
                EmailTemplates = templates
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UploadExcel(IFormFile file, EmailSenderViewModel model)
        {
            if (file == null || file.Length <= 0)
            {
                ModelState.AddModelError("", "Please upload a valid Excel file.");
                return RedirectToAction("Index");
            }

            var template = await _context.EmailTemplates.FindAsync(model.SelectedTemplateId);
            if (template == null)
            {
                ModelState.AddModelError("", "Invalid template selected.");
                return RedirectToAction("Index");
            }

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1);
                    var emails = worksheet
                        .RowsUsed()
                        .Select(row => row.Cell(1).GetString())
                        .Where(email => !string.IsNullOrEmpty(email))
                        .ToList();

                    foreach (var email in emails)
                    {
                        // Decode the HTML content from the database before sending
                        string decodedHtmlBody = WebUtility.HtmlDecode(template.HtmlContent);

                        // Send email using the decoded HTML content from the database
                        SendEmail(email, model.Subject, template.Header, decodedHtmlBody, template.Footer);
                    } 
                }
            }

            return RedirectToAction("Index");
        }

        private void SendEmail(string email, string subject, string header, string htmlBody, string footer)
        {
            string baseTemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "email_template.html");
            string emailTemplate = System.IO.File.ReadAllText(baseTemplatePath);

            // Final email content using decoded HTML content
            string finalEmailContent = emailTemplate
                .Replace("{Header}", header)
                .Replace("{Body}", htmlBody)  // htmlBody should contain valid HTML content
                .Replace("{Footer}", footer);

            var fromAddress = new MailAddress(_smtpSettings.From, "Your Name");
            var toAddress = new MailAddress(email);

            using (var message = new MailMessage(fromAddress, toAddress))
            {
                message.Subject = subject;
                message.Body = finalEmailContent;  // This should be the HTML content
                message.IsBodyHtml = true;         // Make sure this is set to true

                using (var smtpClient = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port))
                {
                    smtpClient.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
                    smtpClient.EnableSsl = _smtpSettings.EnableSsl;

                    try
                    {
                        smtpClient.Send(message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error sending email: {ex.Message}");
                    }
                }
            }
        }

    }
}
