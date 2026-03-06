using Microsoft.Extensions.Configuration;

namespace CloudFileServer.Applibs;

public static class ConfigHelper
{
    private static IConfiguration? _config;

    public static IConfiguration Config
    {
        get
        {
            if (_config == null)
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();

                _config = builder.Build();
            }

            return _config;
        }
    }

    public static ConnectionStringsSettings ConnectionStrings =>
        Config.GetSection(nameof(ConnectionStrings)).Get<ConnectionStringsSettings>()
            ?? throw new InvalidOperationException(
                "ConnectionStrings section is missing or invalid in configuration.");

    public static AppSettings AppSettings =>
        Config.GetSection(nameof(AppSettings)).Get<AppSettings>()
            ?? throw new InvalidOperationException(
                "AppSettings section is missing or invalid in configuration.");
}

public class ConnectionStringsSettings
{
    public string MSSQL { get; set; } = string.Empty;
}

public class AppSettings
{
    public string FileStoragePath { get; set; } = string.Empty;
}
