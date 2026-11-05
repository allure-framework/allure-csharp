using Allure.Runtime;

namespace Allure;

public static partial class AllureApi
{
    /// <summary>
    /// Sets the description of the current test in Markdown.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="description">The description of the test. Markdown is supported.</param>
    public static void SetDescription(string description) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Sync.SetDescription(description);

    /// <summary>
    /// Sets the description of the current test. Allows HTML to be used.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="descriptionHtml">
    /// The description in the HTML format.
    /// </param>
    public static void SetDescriptionHtml(string descriptionHtml) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Sync.SetDescriptionHtml(descriptionHtml);

    /// <summary>
    /// Documents the preconditions of the current test in Markdown.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="preconditions">The preconditions of the test. Markdown is supported.</param>
    public static void SetPreconditions(string preconditions) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Sync.SetPreconditions(preconditions);

    /// <summary>
    /// Documents the preconditions of the current test in HTML.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="preconditionsHtml">The preconditions in the HTML format.</param>
    public static void SetPreconditionsHtml(string preconditionsHtml) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Sync.SetPreconditionsHtml(preconditionsHtml);

    /// <summary>
    /// Documents the expected results of the current test in Markdown.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="expectedResults">The expected results of the test. Markdown is supported.</param>
    public static void SetExpectedResults(string expectedResults) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Sync.SetExpectedResults(expectedResults);

    /// <summary>
    /// Documents the expected results of the current test in HTML.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="expectedResultsHtml">The expected results in the HTML format.</param>
    public static void SetExpectedResultsHtml(string expectedResultsHtml) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Sync.SetExpectedResultsHtml(expectedResultsHtml);
}
