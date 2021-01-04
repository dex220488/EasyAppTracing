namespace EasyAppTracing.Entities.TraceSettings
{
    internal class GlobalSettings
    {
        public string TracingId { get; set; }
        public bool EnableInformationTrace { get; set; }
        public bool EnableErrorTrace { get; set; }
        public FileSettings FileSettings { get; set; }
        public EmailSettings EmailSettings { get; set; }
        public ElasticSearchSettings ElasticSearchSettings { get; set; }
    }
}