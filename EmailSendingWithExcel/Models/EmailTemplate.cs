namespace EmailSendingWithExcel.Models
{
    public class EmailTemplate
    {
        public int Id { get; set; }
        public string TemplateName { get; set; }
        public string Header { get; set; }
        public string Footer { get; set; }
        public string HtmlContent { get; set; }
    }

}
