namespace Allure.Sdk.Configuration;

/// <summary>
/// Defines templates used to expand a link URL and, optionally, its display name.
/// </summary>
/// <param name="UrlTemplate">
/// The composite format string used to build the URL. Placeholder <c>{0}</c>
/// represents the original link value.
/// </param>
/// <param name="NameTemplate">
/// The optional composite format string used to build the display name.
/// </param>
public record class AllureLinkTemplate(
    string UrlTemplate,
    string? NameTemplate
);
