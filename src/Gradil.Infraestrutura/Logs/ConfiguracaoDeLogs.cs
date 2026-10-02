using Microsoft.Extensions.Logging;
using Serilog;

namespace Gradil.Infraestrutura.Logs;

public static class ConfiguracaoDeLogs
{
    public static ILoggerFactory Criar(string pastaDosLogs)
    {
        var serilog = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File(
                Path.Combine(pastaDosLogs, "gradil-.log"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 14,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        return LoggerFactory.Create(builder => builder.AddSerilog(serilog, dispose: true));
    }
}
