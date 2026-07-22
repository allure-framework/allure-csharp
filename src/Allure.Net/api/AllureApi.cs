using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Allure.Abstractions;
using Allure.Model;

namespace Allure;

/// <summary>
/// A facade that provides the API for test authors to enhance the Allure
/// report.
/// </summary>
public static partial class AllureApi
{
    static Stream ToStream(string text) =>
        new MemoryStream(
            Encoding.UTF8.GetBytes(text)
        );

    static void AddTestParameterFromObject(
        IAllureRuntimeEndpoint? endpoint,
        string name,
        object? value,
        ParameterMode? mode,
        bool excluded
    )
    {
        if (endpoint is null)
        {
            return;
        }

        endpoint.Operations.Sync.AddTestParameter(new()
        {
            Name = name,
            Value = endpoint.ParameterSerializer.Serialize(value),
            Mode = mode,
            Excluded = excluded,
        });
    }

    static Task AddTestParameterFromObjectAsync(
        IAllureRuntimeEndpoint? endpoint,
        string name,
        object? value,
        ParameterMode? mode,
        bool excluded,
        CancellationToken cancellationToken
    )
    {
        if (endpoint is null)
        {
            return Task.CompletedTask;
        }

        return endpoint.Operations.Async.AddTestParameterAsync(new()
        {
            Name = name,
            Value = endpoint.ParameterSerializer.Serialize(value),
            Mode = mode,
            Excluded = excluded,
        }, cancellationToken);
    }
}
