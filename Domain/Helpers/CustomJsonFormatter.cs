using Serilog.Events;
using Serilog.Formatting;
using System.Text.Json;

namespace Domain.Helpers;

public class CustomJsonFormatter : ITextFormatter
{
    public void Format(LogEvent logEvent, TextWriter output)
    {
        var logObject = new
        {
            Timestamp = logEvent.Timestamp.ToString("o"),
            LogLevel = logEvent.Level.ToString(),
            Message = logEvent.RenderMessage(),
            Exception = logEvent.Exception?.ToString(),
            Properties = logEvent.Properties
        };

        var json = JsonSerializer.Serialize(logObject, new JsonSerializerOptions { WriteIndented = true });
        output.WriteLine(json);
    }
}
