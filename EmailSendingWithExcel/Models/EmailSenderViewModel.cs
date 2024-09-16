using System.Collections.Generic;

namespace EmailSendingWithExcel.Models
{
    public class EmailSenderViewModel
    {
        public int SelectedTemplateId { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public IEnumerable<EmailTemplate> EmailTemplates { get; set; }
    }
}
