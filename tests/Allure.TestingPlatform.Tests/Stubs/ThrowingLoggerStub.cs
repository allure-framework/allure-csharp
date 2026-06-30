using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Tests.Stubs;

public class ThrowingLoggerStub : ILogger
{
    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (exception is not null)
        {
            throw exception;
        }
    }

    public Task LogAsync<TState>(LogLevel logLevel, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (exception is not null)
        {
            throw exception;
        }
        return Task.CompletedTask;
    }
}