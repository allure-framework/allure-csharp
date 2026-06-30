using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Tests.Stubs;

public class LoggerSpy : ILogger
{
    public record LogCallData(LogLevel Level, object State, Exception Exception);

    public List<LogCallData> Calls { get; } = [];

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        this.Calls.Add(new(logLevel, state, exception));
    }

    public Task LogAsync<TState>(LogLevel logLevel, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        this.Calls.Add(new(logLevel, state, exception));
        return Task.CompletedTask;
    }
}