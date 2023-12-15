using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text.Json;

namespace BinaryFileManager.Logging;

public static class FileLoggerFactory
{
    private static ILogger? _loggerInstance;
    private static readonly object _lock = new();

    public static ILogger GetLogger()
    {
        if (_loggerInstance == null)
            lock (_lock)
                if (_loggerInstance == null)
                    _loggerInstance = CreateLoggerInstance();

        return _loggerInstance;
    }

    private static ILogger CreateLoggerInstance()
    {
        try
        {
            var filePath = Path.Combine(GetProjectPath(), "logging.json");
            var loggingConfig = LoadLoggingConfiguration(filePath);

            if (loggingConfig.Enabled)
                return new FileLoggerProvider(loggingConfig).CreateLogger("");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Configuration reading error.: {ex.Message}");
            throw;
        }

        throw new Exception("Logger creation failed.");
    }

    public static string GetProjectPath()
    {
        /// TODO: Znaleźć rozwiązanie pobierania ścieżki do projektu w lepszy sposób
        // Cofanie do ścieżki projektu przez problem z uzyskaniem ścieżki względnej projektu. Ścieżka jest do plików w folderze bin. 
        return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\.."));
    }

    private static FileLoggerConfiguration LoadLoggingConfiguration(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("Configuration file does not exist.", filePath);

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        using var fileStream = new FileStream(filePath, FileMode.Open);
        return JsonSerializer.Deserialize<FileLoggerConfiguration>(fileStream, options);
    }
}