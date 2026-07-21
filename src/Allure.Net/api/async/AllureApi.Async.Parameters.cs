using System.Threading;
using System.Threading.Tasks;
using Allure.Abstractions;
using Allure.Model;
using Allure.Runtime;

namespace Allure;

/// <summary>
/// A facade that provides the API for test authors to enhance the Allure
/// report.
/// </summary>
public static partial class AllureApi
{
    /// <summary>
    /// Adds a new parameter to the current test from a CLR object.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="name">The name of the new parameter.</param>
    /// <param name="value">
    /// The value of the new parameter.
    /// </param>
    public static Task AddTestParameterFromObjectAsync(string name, object? value) =>
        AddTestParameterFromObjectAsync(
            AllureFrontend.Client.ResolveCurrentScope(),
            name,
            value,
            null,
            false,
            default
        );

    /// <summary>
    /// Adds a new parameter to the current test from a CLR object.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="name">The name of the new parameter.</param>
    /// <param name="value">
    /// The value of the new parameter.
    /// </param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddTestParameterFromObjectAsync(
        string name,
        object? value,
        CancellationToken cancellationToken
    ) =>
        AddTestParameterFromObjectAsync(
            AllureFrontend.Client.ResolveCurrentScope(),
            name,
            value,
            null,
            false,
            cancellationToken
        );

    /// <summary>
    /// Adds a new parameter to the current test from a CLR object.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="name">The name of the new parameter.</param>
    /// <param name="value">
    /// The value of the new parameter.
    /// </param>
    /// <param name="mode">The display mode of the new parameter.</param>
    public static Task AddTestParameterFromObjectAsync(
        string name,
        object? value,
        ParameterMode mode
    ) =>
        AddTestParameterFromObjectAsync(
            AllureFrontend.Client.ResolveCurrentScope(),
            name,
            value,
            mode,
            false,
            default
        );

    /// <summary>
    /// Adds a new parameter to the current test from a CLR object.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="name">The name of the new parameter.</param>
    /// <param name="value">
    /// The value of the new parameter.
    /// </param>
    /// <param name="mode">The display mode of the new parameter.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddTestParameterFromObjectAsync(
        string name,
        object? value,
        ParameterMode mode,
        CancellationToken cancellationToken
    ) =>
        AddTestParameterFromObjectAsync(
            AllureFrontend.Client.ResolveCurrentScope(),
            name,
            value,
            mode,
            false,
            cancellationToken
        );

    /// <summary>
    /// Adds a new parameter to the current test from a CLR object.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="name">The name of the new parameter.</param>
    /// <param name="value">
    /// The value of the new parameter.
    /// </param>
    /// <param name="excluded">
    /// The exclusion flag of the new parameter. If set to true, the parameter
    /// doesn't affect the test's history.
    /// </param>
    public static Task AddTestParameterFromObjectAsync(
        string name,
        object? value,
        bool excluded
    ) =>
        AddTestParameterFromObjectAsync(
            AllureFrontend.Client.ResolveCurrentScope(),
            name,
            value,
            null,
            excluded,
            default
        );

    /// <summary>
    /// Adds a new parameter to the current test from a CLR object.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="name">The name of the new parameter.</param>
    /// <param name="value">
    /// The value of the new parameter.
    /// </param>
    /// <param name="excluded">
    /// The exclusion flag of the new parameter. If set to true, the parameter
    /// doesn't affect the test's history.
    /// </param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddTestParameterFromObjectAsync(
        string name,
        object? value,
        bool excluded,
        CancellationToken cancellationToken
    ) =>
        AddTestParameterFromObjectAsync(
            AllureFrontend.Client.ResolveCurrentScope(),
            name,
            value,
            null,
            excluded,
            cancellationToken
        );

    /// <summary>
    /// Adds a new parameter to the current test from a CLR object.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="name">The name of the new parameter.</param>
    /// <param name="value">
    /// The value of the new parameter.
    /// </param>
    /// <param name="mode">The display mode of the new parameter.</param>
    /// <param name="excluded">
    /// The exclusion flag of the new parameter. If set to true, the parameter
    /// doesn't affect the test's history.
    /// </param>
    public static Task AddTestParameterFromObjectAsync(
        string name,
        object? value,
        ParameterMode mode,
        bool excluded
    ) =>
        AddTestParameterFromObjectAsync(
            AllureFrontend.Client.ResolveCurrentScope(),
            name,
            value,
            mode,
            excluded,
            default
        );

    /// <summary>
    /// Adds a new parameter to the current test from a CLR object.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="name">The name of the new parameter.</param>
    /// <param name="value">
    /// The value of the new parameter.
    /// </param>
    /// <param name="mode">The display mode of the new parameter.</param>
    /// <param name="excluded">
    /// The exclusion flag of the new parameter. If set to true, the parameter
    /// doesn't affect the test's history.
    /// </param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddTestParameterFromObjectAsync(
        string name,
        object? value,
        ParameterMode mode,
        bool excluded,
        CancellationToken cancellationToken
    ) =>
        AddTestParameterFromObjectAsync(
            AllureFrontend.Client.ResolveCurrentScope(),
            name,
            value,
            mode,
            excluded,
            cancellationToken
        );

    /// <summary>
    /// Adds a new parameter to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="name">The name of the new parameter.</param>
    /// <param name="value">
    /// The value of the new parameter. The value is used as is.
    /// </param>
    public static Task AddTestParameterAsync(string name, string value) =>
        AddTestParameterAsync(new()
        {
            Name = name,
            Value = value,
        });

    /// <summary>
    /// Adds a new parameter to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="name">The name of the new parameter.</param>
    /// <param name="value">
    /// The value of the new parameter. The value is used as is.
    /// </param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddTestParameterAsync(
        string name,
        string value,
        CancellationToken cancellationToken
    ) =>
        AddTestParameterAsync(new()
        {
            Name = name,
            Value = value,
        }, cancellationToken);

    /// <summary>
    /// Adds a new parameter to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="name">The name of the new parameter.</param>
    /// <param name="value">
    /// The value of the new parameter. The value is used as is.
    /// </param>
    /// <param name="mode">The display mode of the new parameter.</param>
    public static Task AddTestParameterAsync(
        string name,
        string value,
        ParameterMode mode
    ) =>
        AddTestParameterAsync(new()
        {
            Name = name,
            Value = value,
            Mode = mode,
        });

    /// <summary>
    /// Adds a new parameter to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="name">The name of the new parameter.</param>
    /// <param name="value">
    /// The value of the new parameter. The value is used as is.
    /// </param>
    /// <param name="mode">The display mode of the new parameter.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddTestParameterAsync(
        string name,
        string value,
        ParameterMode mode,
        CancellationToken cancellationToken
    ) =>
        AddTestParameterAsync(new()
        {
            Name = name,
            Value = value,
            Mode = mode,
        }, cancellationToken);

    /// <summary>
    /// Adds a new parameter to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="name">The name of the new parameter.</param>
    /// <param name="value">
    /// The value of the new parameter. The value is used as is.
    /// </param>
    /// <param name="excluded">
    /// The exclusion flag of the new parameter. If set to true, the parameter
    /// doesn't affect the test's history.
    /// </param>
    public static Task AddTestParameterAsync(
        string name,
        string value,
        bool excluded
    ) =>
        AddTestParameterAsync(new()
        {
            Name = name,
            Value = value,
            Excluded = excluded,
        });

    /// <summary>
    /// Adds a new parameter to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="name">The name of the new parameter.</param>
    /// <param name="value">
    /// The value of the new parameter. The value is used as is.
    /// </param>
    /// <param name="excluded">
    /// The exclusion flag of the new parameter. If set to true, the parameter
    /// doesn't affect the test's history.
    /// </param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddTestParameterAsync(
        string name,
        string value,
        bool excluded,
        CancellationToken cancellationToken
    ) =>
        AddTestParameterAsync(new()
        {
            Name = name,
            Value = value,
            Excluded = excluded,
        }, cancellationToken);

    /// <summary>
    /// Adds a new parameter to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="name">The name of the new parameter.</param>
    /// <param name="value">
    /// The value of the new parameter. The value is used as is.
    /// </param>
    /// <param name="mode">The display mode of the new parameter.</param>
    /// <param name="excluded">
    /// The exclusion flag of the new parameter. If set to true, the parameter
    /// doesn't affect the test's history.
    /// </param>
    public static Task AddTestParameterAsync(
        string name,
        string value,
        ParameterMode mode,
        bool excluded
    ) =>
        AddTestParameterAsync(new()
        {
            Name = name,
            Value = value,
            Mode = mode,
            Excluded = excluded,
        });

    /// <summary>
    /// Adds a new parameter to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="name">The name of the new parameter.</param>
    /// <param name="value">
    /// The value of the new parameter. The value is used as is.
    /// </param>
    /// <param name="mode">The display mode of the new parameter.</param>
    /// <param name="excluded">
    /// The exclusion flag of the new parameter. If set to true, the parameter
    /// doesn't affect the test's history.
    /// </param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddTestParameterAsync(
        string name,
        string value,
        ParameterMode mode,
        bool excluded,
        CancellationToken cancellationToken
    ) =>
        AddTestParameterAsync(new()
        {
            Name = name,
            Value = value,
            Mode = mode,
            Excluded = excluded,
        }, cancellationToken);

    /// <summary>
    /// Adds a new parameter to the current test. Use this overload if you
    /// want to manually control how the parameter's value should be displayed
    /// in the report.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="parameter">
    /// A new parameter instance.
    /// </param>
    public static Task AddTestParameterAsync(Parameter parameter) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddTestParameterAsync(parameter, default)
            ?? Task.CompletedTask;

    /// <summary>
    /// Adds a new parameter to the current test. Use this overload if you
    /// want to manually control how the parameter's value should be displayed
    /// in the report.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="parameter">
    /// A new parameter instance.
    /// </param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddTestParameterAsync(
        Parameter parameter,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddTestParameterAsync(parameter, cancellationToken)
            ?? Task.CompletedTask;

    static Task AddTestParameterFromObjectAsync(
        IAllureApiEndpoint? endpoint,
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
