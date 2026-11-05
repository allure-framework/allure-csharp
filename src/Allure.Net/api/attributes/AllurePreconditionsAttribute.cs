using System;
using Allure.Abstractions;
using Allure.Model;

namespace Allure;

/// <summary>
/// Documents preconditions.
/// </summary>
/// <param name="markdownText">Preconditions text. Markdown is supported.</param>
[AttributeUsage(ALLURE_METADATA_TARGETS, AllowMultiple = true, Inherited = true)]
public class AllurePreconditionsAttribute(string markdownText) : AllureApiAttribute
{
    /// <summary>
    /// Preconditions text in Markdown format.
    /// </summary>
    public string MarkdownText { get; init; } = markdownText;

    /// <summary>
    /// If set to <c>true</c>, the preconditions are appended to the existing ones with <c>"\n\n"</c>.
    /// Otherwise, the existing preconditions will be overwritten with the new ones.
    /// </summary>
    /// <remarks>
    /// Here is a list of guarantees about the order in which attribute targets are considered when
    /// the attributes are applied:
    /// <list type="number">
    /// <item>Interfaces before classes/structs.</item>
    /// <item>Base classes before derived classes.</item>
    /// <item>Classes/structs before methods.</item>
    /// <item>Base methods before method overrides.</item>
    /// </list>
    /// </remarks>
    public bool Append { get; init; }

    /// <inheritdoc/>
    public override void Apply(TestResult testResult)
    {
        if (this.MarkdownText is null)
        {
            return;
        }

        if (this.Append && !string.IsNullOrEmpty(testResult.Preconditions))
        {
            testResult.Preconditions += $"\n\n{this.MarkdownText}";
        }
        else
        {
            testResult.Preconditions = this.MarkdownText;
        }
    }
}
