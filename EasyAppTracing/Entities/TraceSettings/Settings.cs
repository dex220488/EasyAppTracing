using Newtonsoft.Json;
using System;
using System.IO;

namespace EasyAppTracing.Entities.TraceSettings
{
    internal class Settings
    {
        public GlobalSettings GlobalSettings { get; set; }

        public Settings()
        {
            string jsonConfig = System.IO.File.ReadAllText($"{Directory.GetCurrentDirectory()}/EasyAppTracingSettings.json");
            GlobalSettings = JsonConvert.DeserializeObject<GlobalSettings>(jsonConfig);
        }

        public string GetPath(Entities.Enums.TraceFileType fileType)
        {
            string filePath;
            if (fileType == Enums.TraceFileType.Serilog)
            {
                filePath = GlobalSettings.SerilogFilePath;
            }
            else
            {
                filePath = GlobalSettings.FileSettings.FilePath;
            }

            if (String.IsNullOrEmpty(filePath))
            {
                filePath = $"{Directory.GetCurrentDirectory()}/EasyAppTracing/";
            }

            Directory.CreateDirectory(filePath);
            filePath = $"{filePath}{fileType.ToString()}_.log";
            return filePath;
        }
    }
}