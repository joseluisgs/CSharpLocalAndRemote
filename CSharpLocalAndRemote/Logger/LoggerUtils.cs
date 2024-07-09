using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace CSharpLocalAndRemote.Logger;

public abstract class LoggerUtils<T>
{
    public static Serilog.Core.Logger GetLogger()
    {
        var customTheme = new AnsiConsoleTheme(new Dictionary<ConsoleThemeStyle, string>
        {
            [ConsoleThemeStyle.Text] = "\x1b[37m", // White for normal text
            [ConsoleThemeStyle.SecondaryText] = "\x1b[37m", // White for secondary text
            [ConsoleThemeStyle.TertiaryText] = "\x1b[90m", // Bright black for tertiary text
            [ConsoleThemeStyle.Invalid] = "\x1b[33m", // Yellow for invalid items
            [ConsoleThemeStyle.Null] = "\x1b[33m", // Yellow for null values
            [ConsoleThemeStyle.Name] = "\x1b[33m", // Yellow for property names
            [ConsoleThemeStyle.String] = "\x1b[36m", // Cyan for strings
            [ConsoleThemeStyle.Number] = "\x1b[35m", // Magenta for numbers
            [ConsoleThemeStyle.Boolean] = "\x1b[35m", // Magenta for booleans
            [ConsoleThemeStyle.Scalar] = "\x1b[35m", // Magenta for other scalar values
            [ConsoleThemeStyle.LevelVerbose] = "\x1b[37m", // White for verbose level
            [ConsoleThemeStyle.LevelDebug] = "\x1b[37m", // White for debug level
            [ConsoleThemeStyle.LevelInformation] = "\x1b[32m", // Green for information level
            [ConsoleThemeStyle.LevelWarning] = "\x1b[33m", // Yellow for warning level
            [ConsoleThemeStyle.LevelError] = "\x1b[31m", // Red for error level
            [ConsoleThemeStyle.LevelFatal] = "\x1b[35m" // Magenta for fatal level
        });

        return new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .WriteTo.Console(
                outputTemplate:
                "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}",
                theme: customTheme
            )
            .Enrich.WithProperty("SourceContext", typeof(T).Name) // Add class name as SourceContext
            .CreateLogger();
    }
}