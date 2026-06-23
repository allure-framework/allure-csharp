using System;
using Microsoft.Testing.Platform.Builder;

namespace Allure.TestingPlatform;

/// <summary>
/// Wires the Allure data consumer into a Microsoft.Testing.Platform host.
/// </summary>
public static class TestApplicationBuilderExtensions
{
    /// <summary>
    /// Registers the Allure data consumer so the host writes Allure result
    /// files for every reported test.
    /// </summary>
    public static ITestApplicationBuilder AddAllure(this ITestApplicationBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.TestHost.AddDataConsumer(_ => new AllureDataConsumer());
        return builder;
    }
}
