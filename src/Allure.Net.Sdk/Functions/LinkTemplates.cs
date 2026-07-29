using System;
using System.Collections.Generic;
using Allure.Model;
using Allure.Sdk.Configuration;

namespace Allure.Sdk.Functions;

public static class LinkTemplates
{
    public static void Apply(
        IReadOnlyDictionary<string, AllureLinkTemplate> templates,
        Link link
    )
    {
        if (templates.TryGetValue(link.Type ?? "link", out var template))
        {
            ApplyLinkTemplate(template, link);
        }
    }

    static void ApplyLinkTemplate(AllureLinkTemplate template, Link link)
    {
        if (Uri.IsWellFormedUriString(link.Url, UriKind.Absolute))
        {
            return;
        }

        var (urlTemplate, nameTemplate) = template;

        var urlInput = link.Url;
        link.Url = string.Format(urlTemplate, urlInput);

        if (nameTemplate is null || !string.IsNullOrEmpty(link.Name))
        {
            return;
        }

        link.Name = string.Format(nameTemplate, urlInput);
    }
}
