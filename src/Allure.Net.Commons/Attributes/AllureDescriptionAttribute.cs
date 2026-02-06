using System;
using Allure.Net.Commons.Sdk;

#nullable enable

namespace Allure.Net.Commons.Attributes;

/// <summary>
/// Applies a description.
/// </summary>
/// <param name="markdownText">A description text. Markdown is supported.</param>
[AttributeUsage(ALLURE_METADATA_TARGETS, AllowMultiple = true, Inherited = true)]
public class AllureDescriptionAttribute(string markdownText) : AllureApiAttribute
{
    /// <summary>
    /// Description text in Markdown format.
    /// </summary>
    public string MarkdownText { get; init; } = markdownText;

    /// <summary>
    /// If set to <c>true</c>, the description is appended to the existing one with <c>"\n\n"</c>.
    /// Otherwise, the existing description will be overwritten with the new one.
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

        if (this.Append && !string.IsNullOrEmpty(testResult.description))
        {
            testResult.description += $"\n\n{this.MarkdownText}";
        }
        else
        {
            testResult.description = this.MarkdownText;
        }
    }
}
