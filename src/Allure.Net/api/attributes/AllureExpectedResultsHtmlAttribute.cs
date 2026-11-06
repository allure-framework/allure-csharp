using System;
using Allure.Abstractions;
using Allure.Model;

namespace Allure;

/// <summary>
/// Documents expected results in HTML.
/// </summary>
/// <remarks>
/// Prefer <see cref="AllureExpectedResultsAttribute"/>, which supports markdown.
/// </remarks>
/// <param name="htmlText">Expected results text in HTML.</param>
[AttributeUsage(ALLURE_METADATA_TARGETS, AllowMultiple = true, Inherited = true)]
public class AllureExpectedResultsHtmlAttribute(string htmlText) : AllureApiAttribute
{
    /// <summary>
    /// Expected results text in HTML.
    /// </summary>
    public string HtmlText { get; init; } = htmlText;

    /// <summary>
    /// If set to <c>true</c>, the expected results are appended to the existing ones. No separator is
    /// inserted.
    /// Use a block element like &lt;p&gt; to separate the values.
    /// If set to <c>false</c> (which is the default), the existing expected results will be
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

        if (this.Append && testResult.ExpectedResultsHtml is not null)
        {
            testResult.ExpectedResultsHtml += this.HtmlText;
        }
        else
        {
            testResult.ExpectedResultsHtml = this.HtmlText;
        }
    }
}
