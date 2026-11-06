using System;
using Allure.Abstractions;
using Allure.Model;

namespace Allure;

/// <summary>
/// Documents preconditions in HTML.
/// </summary>
/// <remarks>
/// Prefer <see cref="AllurePreconditionsAttribute"/>, which supports markdown.
/// </remarks>
/// <param name="htmlText">Preconditions text in HTML.</param>
[AttributeUsage(ALLURE_METADATA_TARGETS, AllowMultiple = true, Inherited = true)]
public class AllurePreconditionsHtmlAttribute(string htmlText) : AllureApiAttribute
{
    /// <summary>
    /// Preconditions text in HTML.
    /// </summary>
    public string HtmlText { get; init; } = htmlText;

    /// <summary>
    /// If set to <c>true</c>, the preconditions are appended to the existing ones. No separator is
    /// inserted.
    /// Use a block element like &lt;p&gt; to separate the values.
    /// If set to <c>false</c> (which is the default), the existing preconditions will be
    /// overwritten with the new ones.
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
        if (this.HtmlText is null)
        {
            return;
        }

        if (this.Append && testResult.PreconditionsHtml is not null)
        {
            testResult.PreconditionsHtml += this.HtmlText;
        }
        else
        {
            testResult.PreconditionsHtml = this.HtmlText;
        }
    }
}
