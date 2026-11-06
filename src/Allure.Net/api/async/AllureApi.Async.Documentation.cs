using System.Threading;
using System.Threading.Tasks;
using Allure.Runtime;

namespace Allure;

public static partial class AllureApi
{
    /// <summary>
    /// Sets the description of the current test in Markdown.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="description">The description of the test. Markdown is supported.</param>
    public static Task SetDescriptionAsync(string description) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Async.SetDescriptionAsync(description, default)
            ?? Task.CompletedTask;

    /// <summary>
    /// Sets the description of the current test in Markdown.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="description">The description of the test. Markdown is supported.</param>
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

    /// <summary>
    /// Documents the preconditions of the current test in Markdown.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="preconditions">The preconditions of the test. Markdown is supported.</param>
    public static Task SetPreconditionsAsync(string preconditions) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Async.SetPreconditionsAsync(preconditions, default)
            ?? Task.CompletedTask;

    /// <summary>
    /// Documents the preconditions of the current test in Markdown.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="preconditions">The preconditions of the test. Markdown is supported.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task SetPreconditionsAsync(string preconditions, CancellationToken cancellationToken) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Async.SetPreconditionsAsync(preconditions, cancellationToken)
            ?? Task.CompletedTask;

    /// <summary>
    /// Documents the preconditions of the current test in HTML.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="preconditionsHtml">The preconditions in the HTML format.</param>
    public static Task SetPreconditionsHtmlAsync(string preconditionsHtml) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Async.SetPreconditionsHtmlAsync(preconditionsHtml, default)
            ?? Task.CompletedTask;

    /// <summary>
    /// Documents the preconditions of the current test in HTML.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="preconditionsHtml">The preconditions in the HTML format.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task SetPreconditionsHtmlAsync(string preconditionsHtml, CancellationToken cancellationToken) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Async.SetPreconditionsHtmlAsync(preconditionsHtml, cancellationToken)
            ?? Task.CompletedTask;

    /// <summary>
    /// Documents the expected results of the current test in Markdown.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="expectedResults">The expected results of the test. Markdown is supported.</param>
    public static Task SetExpectedResultsAsync(string expectedResults) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Async.SetExpectedResultsAsync(expectedResults, default)
            ?? Task.CompletedTask;

    /// <summary>
    /// Documents the expected results of the current test in Markdown.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="expectedResults">The expected results of the test. Markdown is supported.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task SetExpectedResultsAsync(string expectedResults, CancellationToken cancellationToken) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Async.SetExpectedResultsAsync(expectedResults, cancellationToken)
            ?? Task.CompletedTask;

    /// <summary>
    /// Documents the expected results of the current test in HTML.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="expectedResultsHtml">The expected results in the HTML format.</param>
    public static Task SetExpectedResultsHtmlAsync(string expectedResultsHtml) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Async.SetExpectedResultsHtmlAsync(expectedResultsHtml, default)
            ?? Task.CompletedTask;

    /// <summary>
    /// Documents the expected results of the current test in HTML.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="expectedResultsHtml">The expected results in the HTML format.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task SetExpectedResultsHtmlAsync(string expectedResultsHtml, CancellationToken cancellationToken) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Async.SetExpectedResultsHtmlAsync(expectedResultsHtml, cancellationToken)
            ?? Task.CompletedTask;
}
