using Microsoft.Extensions.Logging;
using System;
using System.Text.Json.Serialization;

namespace BinaryFileManager.Logging;

public class FileLoggerConfiguration
{
    public bool Enabled { get; set; }

    public string ToFileLogLevel { get; set; }

    public string LogFilePath { get; set; }

    public long MaxFileSizeBytes { get; set; }

    public int MaxFileCount { get; set; }

    [JsonIgnore]
    public LogLevel LogLevel
    {
        get
        {
            if (Enum.TryParse(ToFileLogLevel, out LogLevel logLevel))
                return logLevel;

            return LogLevel.Information;
        }
    }
}