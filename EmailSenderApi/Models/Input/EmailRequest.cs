namespace EmailSenderApi.Models.Input
{
    public class EmailRequest
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string? Body { get; set; }  // Optional
        public string? TemplateName { get; set; } // Use predefined template (optional)
        public Dictionary<string, string>? TemplateValues { get; set; } // Dynamic values
    }
}
