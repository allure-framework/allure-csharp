using Allure.Model;
using Allure.Runtime;

namespace Allure;

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
    public static void AddTestParameterFromObject(string name, object? value) =>
        AddTestParameterFromObject(
            AllureRuntimeRouter.ResolveCurrentScope(),
            name,
            value,
            null,
            false
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
    public static void AddTestParameterFromObject(string name, object? value, ParameterMode mode) =>
        AddTestParameterFromObject(
            AllureRuntimeRouter.ResolveCurrentScope(),
            name,
            value,
            mode,
            false
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
    public static void AddTestParameterFromObject(string name, object? value, bool excluded) =>
        AddTestParameterFromObject(
            AllureRuntimeRouter.ResolveCurrentScope(),
            name,
            value,
            null,
            excluded
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
    public static void AddTestParameterFromObject(
        string name,
        object? value,
        ParameterMode mode,
        bool excluded
    ) =>
        AddTestParameterFromObject(
            AllureRuntimeRouter.ResolveCurrentScope(),
            name,
            value,
            mode,
            excluded
        );

    /// <summary>
    /// Adds a new parameter to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="name">The name of the new parameter.</param>
    /// <param name="value">
    /// The value of the new parameter. The value is used as is.
    /// </param>
    public static void AddTestParameter(string name, string value) =>
        AddTestParameter(new()
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
    /// <param name="mode">The display mode of the new parameter.</param>
    public static void AddTestParameter(string name, string value, ParameterMode mode) =>
        AddTestParameter(new()
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
    /// <param name="excluded">
    /// The exclusion flag of the new parameter. If set to true, the parameter
    /// doesn't affect the test's history.
    /// </param>
    public static void AddTestParameter(string name, string value, bool excluded) =>
        AddTestParameter(new()
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
    /// <param name="mode">The display mode of the new parameter.</param>
    /// <param name="excluded">
    /// The exclusion flag of the new parameter. If set to true, the parameter
    /// doesn't affect the test's history.
    /// </param>
    public static void AddTestParameter(
        string name,
        string value,
        ParameterMode mode,
        bool excluded
    ) =>
        AddTestParameter(new()
        {
            Name = name,
            Value = value,
            Mode = mode,
            Excluded = excluded,
        });

    /// <summary>
    /// Adds a new parameter to the current test. Use this overload if you
    /// want to manually control how the parameter's value should be displayed
    /// in the report.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="parameter">
    /// A new parameter instance.
    /// </param>
    public static void AddTestParameter(Parameter parameter) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Sync.AddTestParameter(parameter);
}
