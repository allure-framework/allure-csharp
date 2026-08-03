using System.Threading;
using System.Threading.Tasks;
using Allure.Runtime;

namespace Allure;

public static partial class AllureApi
{
    /// <summary>
    /// Sets the name of the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="newName">The new name of the test.</param>
    public static Task SetTestNameAsync(string newName) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Async.SetTestNameAsync(newName, default)
            ?? Task.CompletedTask;

    /// <summary>
    /// Sets the name of the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="newName">The new name of the test.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task SetTestNameAsync(string newName, CancellationToken cancellationToken) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Async.SetTestNameAsync(newName, cancellationToken)
            ?? Task.CompletedTask;

    /// <summary>
    /// Sets the name of the current fixture.
    /// </summary>
    /// <param name="newName">The new name of the fixture.</param>
    public static Task SetFixtureNameAsync(string newName) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Async.SetFixtureNameAsync(newName, default)
            ?? Task.CompletedTask;

    /// <summary>
    /// Sets the name of the current fixture.
    /// </summary>
    /// <param name="newName">The new name of the fixture.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task SetFixtureNameAsync(string newName, CancellationToken cancellationToken) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Async.SetFixtureNameAsync(newName, cancellationToken)
            ?? Task.CompletedTask;

    /// <summary>
    /// Sets the name of the current step, fixture, or test.
    /// </summary>
    /// <param name="newName">A new name.</param>
    /// <remarks>
    /// If a step is running, renames the step.
    /// Otherwise, if a fixture is running, renames the fixture.
    /// Otherwise, if a test is running, renames the test.
    /// Otherwise, does nothing.
    /// </remarks>
    public static Task SetNameAsync(string newName) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Async.SetNameAsync(newName, default)
            ?? Task.CompletedTask;

    /// <summary>
    /// Sets the name of the current step, fixture, or test.
    /// </summary>
    /// <param name="newName">A new name.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <remarks>
    /// If a step is running, renames the step.
    /// Otherwise, if a fixture is running, renames the fixture.
    /// Otherwise, if a test is running, renames the test.
    /// Otherwise, does nothing.
    /// </remarks>
    public static Task SetNameAsync(string newName, CancellationToken cancellationToken) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Async.SetNameAsync(newName, cancellationToken)
            ?? Task.CompletedTask;

    /// <summary>
    /// Sets the description of the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="description">The description of the test.</param>
    public static Task SetDescriptionAsync(string description) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Async.SetDescriptionAsync(description, default)
            ?? Task.CompletedTask;

    /// <summary>
    /// Sets the description of the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="description">The description of the test.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task SetDescriptionAsync(string description, CancellationToken cancellationToken) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Async.SetDescriptionAsync(description, cancellationToken)
            ?? Task.CompletedTask;

    /// <summary>
    /// Sets the description of the current test. Allows HTML to be used.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="descriptionHtml">
    /// The description in the HTML format.
    /// </param>
    public static Task SetDescriptionHtmlAsync(string descriptionHtml) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Async.SetDescriptionHtmlAsync(descriptionHtml, default)
            ?? Task.CompletedTask;


    /// <summary>
    /// Sets the description of the current test. Allows HTML to be used.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="descriptionHtml">
    /// The description in the HTML format.
    /// </param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task SetDescriptionHtmlAsync(string descriptionHtml, CancellationToken cancellationToken) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Async.SetDescriptionHtmlAsync(descriptionHtml, cancellationToken)
            ?? Task.CompletedTask;
}
