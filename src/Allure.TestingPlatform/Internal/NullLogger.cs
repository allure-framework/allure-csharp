using System;
using System.Threading.Tasks;
using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Internal;

sealed class NullLogger : ILogger
{
    public bool IsEnabled(LogLevel logLevel) => false;

    public void Log<TState>(LogLevel logLevel, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
    }

    public Task LogAsync<TState>(LogLevel logLevel, TState state, Exception? exception, Func<TState, Exception?, string> formatter) =>
        Task.CompletedTask;

    public static NullLogger Instance { get; } = new();
}
