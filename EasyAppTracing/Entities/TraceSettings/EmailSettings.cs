namespace EasyAppTracing.Entities.TraceSettings
{
    internal class EmailSettings
    {
        public bool Enable { get; set; }
        public string FromEmail { get; set; }
        public string ToEmail { get; set; }
        public string SmtpServer { get; set; }
        public int MaxStringLength { get; set; }
    }
}