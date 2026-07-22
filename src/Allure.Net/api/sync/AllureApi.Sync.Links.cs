using Allure.Model;
using Allure.Runtime;
using System.Collections.Generic;

namespace Allure;

public static partial class AllureApi
{
    /// <summary>
    /// Adds a new link to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="url">The address of the link.</param>
    public static void AddLink(string url) =>
        AddLink(
            new Link { Url = url }
        );

    /// <summary>
    /// Adds a new link to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="url">The address of the link.</param>
    /// <param name="name">The display text of the link.</param>
    public static void AddLink(string url, string name) =>
        AddLink(
            new Link { Name = name, Url = url }
        );

    /// <summary>
    /// Adds a new link to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="url">The address of the link.</param>
    /// <param name="name">The display text of the link.</param>
    /// <param name="type">
    /// The type of the link. Used when matching link patterns. Might also
    /// affect how the link is rendered in the report.
    /// </param>
    public static void AddLink(string url, string name, string type) =>
        AddLink(
            new Link { Name = name, Type = type, Url = url }
        );

    /// <summary>
    /// Adds a new link to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="link">A link to add.</param>
    public static void AddLink(Link link) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Sync.AddLink(link);

    /// <summary>
    /// Adds new links to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="links">The link instances to add.</param>
    public static void AddLinks(params IEnumerable<Link> links) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Sync.AddLinks(links);

    /// <summary>
    /// Adds a new issue link to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="url">The URL of the issue.</param>
    public static void AddIssue(string url) =>
        AddLink(
            new Link { Type = LinkType.Issue, Url = url }
        );

    /// <summary>
    /// Adds a new issue link to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="url">The URL of the issue.</param>
    /// <param name="name">The display text of the issue link.</param>
    public static void AddIssue(string url, string name) =>
        AddLink(
            new Link { Name = name, Type = LinkType.Issue, Url = url }
        );

    /// <summary>
    /// Adds a new TMS item link to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="url">The URL of the TMS item.</param>
    public static void AddTmsItem(string url) =>
        AddLink(
            new Link { Type = LinkType.TmsItem, Url = url }
        );

    /// <summary>
    /// Adds a new TMS item link to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="url">The URL of the TMS item.</param>
    /// <param name="name">The display text of the TMS item link.</param>
    public static void AddTmsItem(string url, string name) =>
        AddLink(
            new Link { Name = name, Type = LinkType.TmsItem, Url = url }
        );
}
