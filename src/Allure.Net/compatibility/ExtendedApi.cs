using System;
using System.ComponentModel;
using System.Threading.Tasks;

using NewAllureApi = Allure.AllureApi;

namespace Allure.Net.Commons;

/// <summary>
/// A legacy facade that provides some advanced Runtime API to enhance the report.
/// </summary>
/// <remarks>
/// Functions that affected the lifecycle were removed. For the rest, please, switch to <see cref="NewAllureApi"/>.
/// </remarks>
[EditorBrowsable(EditorBrowsableState.Never)]
[Obsolete("Allure.Net.Commons.ExtendedApi is a legacy API and will be removed in a future update. Please, switch to Allure.AllureApi.")]
public static class ExtendedApi
{
    /// <summary>
    /// Executes the action and reports the result as a new setup fixture.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.SetUp(string, Action)"/>.
    /// </remarks>
    public static void Before(string name, Action action)
    {
        NewAllureApi.SetUp(name, action);
    }

    /// <summary>
    /// Executes the function and reports the result as a new setup fixture.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.SetUp{TResult}(string, Func{TResult})"/>.
    /// </remarks>
    public static T Before<T>(string name, Func<T> function) =>
        NewAllureApi.SetUp(name, function);

    /// <summary>
    /// Executes the asynchronous action and reports the result as a new setup
    /// fixture.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.SetUpAsync(string, Func{Task})"/>.
    /// </remarks>
    public static Task Before(string name, Func<Task> action) =>
        NewAllureApi.SetUp(name, action);

    /// <summary>
    /// Executes the asynchronous function and reports the result as a new
    /// setup fixture.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.SetUpAsync{TResult}(string, Func{Task{TResult}})"/>.
    /// </remarks>
    public static Task<T> Before<T>(string name, Func<Task<T>> function) =>
        NewAllureApi.SetUp(name, function);

    /// <summary>
    /// Executes the action and reports the result as a new teardown fixture.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.TearDown(string, Action)"/>.
    /// </remarks>
    public static void After(string name, Action action) =>
        NewAllureApi.TearDown(name, action);

    /// <summary>
    /// Executes the function and reports the result as a new teardown fixture.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.TearDown{TResult}(string, Func{TResult})"/>.
    /// </remarks>
    public static T After<T>(string name, Func<T> function) =>
        NewAllureApi.TearDown(name, function);

    /// <summary>
    /// Executes the asynchronous action and reports the result as a new
    /// teardown fixture.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.TearDownAsync(string, Func{Task})"/>.
    /// </remarks>
    public static Task After(string name, Func<Task> action) =>
        NewAllureApi.TearDown(name, action);

    /// <summary>
    /// Executes the asynchronous function and reports the result as a new
    /// teardown fixture.
    /// </summary>
    /// <remarks>
    /// This is a part of the legacy API. Please, switch to <see cref="NewAllureApi.TearDownAsync{TResult}(string, Func{Task{TResult}})"/>.
    /// </remarks>
    public static Task<T> After<T>(string name, Func<Task<T>> function) =>
        NewAllureApi.TearDown(name, function);
}
