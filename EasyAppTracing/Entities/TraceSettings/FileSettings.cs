namespace EasyAppTracing.Entities.TraceSettings
{
    internal class FileSettings
    {
        public bool Enable { get; set; }
        public string FilePath { get; set; }
        public int MaxStringLength { get; set; }
    }
}