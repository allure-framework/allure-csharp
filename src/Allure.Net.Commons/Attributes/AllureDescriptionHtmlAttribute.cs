using System;
using Allure.Net.Commons.Sdk;

#nullable enable

namespace Allure.Net.Commons.Attributes;

/// <summary>
/// Applies a description in HTML.
/// </summary>
/// <remarks>
/// Prefer <see cref="AllureDescriptionAttribute"/>, which supports markdown.
/// </remarks>
/// <param name="htmlText">A description text in HTML.</param>
[AttributeUsage(ALLURE_METADATA_TARGETS, AllowMultiple = true, Inherited = true)]
public class AllureDescriptionHtmlAttribute(string htmlText) : AllureApiAttribute
{
    /// <summary>
    /// Description text in HTML.
    /// </summary>
    public string HtmlText { get; init; } = htmlText;

    /// <summary>
    /// If set to <c>true</c>, the description is appended to the existing one. No separator is
    /// inserted.
    /// Use a block element like &lt;p&gt; to separate the values.
    /// If set to <c>false</c> (which is the default), the existing description will be
    /// overwritten with the new one.
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

        if (this.Append && testResult.descriptionHtml is not null)
        {
            testResult.descriptionHtml += this.HtmlText;
        }
        else
        {
            testResult.descriptionHtml = this.HtmlText;
        }
    }
}
