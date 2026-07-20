using Allure.Model;
using Allure.Runtime;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Allure;

/// <summary>
/// A facade that provides the API for test authors to enhance the Allure
/// report.
/// </summary>
public static partial class AllureApi
{
    /// <summary>
    /// Adds a new link to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="url">The address of the link.</param>
    public static Task AddLinkAsync(string url) =>
        AddLinkAsync(
            new Link { Url = url }
        );

    /// <summary>
    /// Adds a new link to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="url">The address of the link.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddLinkAsync(string url, CancellationToken cancellationToken) =>
        AddLinkAsync(
            new Link { Url = url },
            cancellationToken
        );

    /// <summary>
    /// Adds a new link to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="url">The address of the link.</param>
    /// <param name="name">The display text of the link.</param>
    public static Task AddLinkAsync(string url, string name) =>
        AddLinkAsync(
            new Link { Name = name, Url = url }
        );

    /// <summary>
    /// Adds a new link to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="url">The address of the link.</param>
    /// <param name="name">The display text of the link.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddLinkAsync(
        string url,
        string name,
        CancellationToken cancellationToken
    ) =>
        AddLinkAsync(
            new Link { Name = name, Url = url },
            cancellationToken
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
    public static Task AddLinkAsync(string url, string name, string type) =>
        AddLinkAsync(
            new Link { Name = name, Type = type, Url = url }
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
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddLinkAsync(
        string url,
        string name,
        string type,
        CancellationToken cancellationToken
    ) =>
        AddLinkAsync(
            new Link { Name = name, Type = type, Url = url },
            cancellationToken
        );

    /// <summary>
    /// Adds a new link to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="link">A link to add.</param>
    public static Task AddLinkAsync(Link link) =>
        AllureFrontend.Runtime.TestApi.Async.AddLink(link, default);

    /// <summary>
    /// Adds a new link to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="link">A link to add.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddLinkAsync(Link link, CancellationToken cancellationToken) =>
        AllureFrontend.Runtime.TestApi.Async.AddLink(link, cancellationToken);

    /// <summary>
    /// Adds new links to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="links">The link instances to add.</param>
    public static Task AddLinksAsync(params IEnumerable<Link> links) =>
        AllureFrontend.Runtime.TestApi.Async.AddLinks(links, default);

    /// <summary>
    /// Adds new links to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="links">The link instances to add.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddLinksAsync(
        IEnumerable<Link> links,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Runtime.TestApi.Async.AddLinks(links, cancellationToken);

    /// <summary>
    /// Adds a new issue link to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="url">The URL of the issue.</param>
    public static Task AddIssueAsync(string url) =>
        AddLinkAsync(
            new Link { Type = LinkType.Issue, Url = url }
        );

    /// <summary>
    /// Adds a new issue link to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="url">The URL of the issue.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddIssueAsync(string url, CancellationToken cancellationToken) =>
        AddLinkAsync(
            new Link { Type = LinkType.Issue, Url = url },
            cancellationToken
        );

    /// <summary>
    /// Adds a new issue link to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="url">The URL of the issue.</param>
    /// <param name="name">The display text of the issue link.</param>
    public static Task AddIssueAsync(string url, string name) =>
        AddLinkAsync(
            new Link { Name = name, Type = LinkType.Issue, Url = url }
        );

    /// <summary>
    /// Adds a new issue link to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="url">The URL of the issue.</param>
    /// <param name="name">The display text of the issue link.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddIssueAsync(
        string url,
        string name,
        CancellationToken cancellationToken
    ) =>
        AddLinkAsync(
            new Link { Name = name, Type = LinkType.Issue, Url = url },
            cancellationToken
        );

    /// <summary>
    /// Adds a new TMS item link to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="url">The URL of the TMS item.</param>
    public static Task AddTmsItemAsync(string url) =>
        AddLinkAsync(
            new Link { Type = LinkType.TmsItem, Url = url }
        );

    /// <summary>
    /// Adds a new TMS item link to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="url">The URL of the TMS item.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddTmsItemAsync(string url, CancellationToken cancellationToken) =>
        AddLinkAsync(
            new Link { Type = LinkType.TmsItem, Url = url },
            cancellationToken
        );

    /// <summary>
    /// Adds a new TMS item link to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="url">The URL of the TMS item.</param>
    /// <param name="name">The display text of the TMS item link.</param>
    public static Task AddTmsItemAsync(string url, string name) =>
        AddLinkAsync(
            new Link { Name = name, Type = LinkType.TmsItem, Url = url }
        );


    /// <summary>
    /// Adds a new TMS item link to the current test.
    /// </summary>
    /// <remarks>If no test is running, does nothing.</remarks>
    /// <param name="url">The URL of the TMS item.</param>
    /// <param name="name">The display text of the TMS item link.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddTmsItemAsync(
        string url,
        string name,
        CancellationToken cancellationToken
    ) =>
        AddLinkAsync(
            new Link { Name = name, Type = LinkType.TmsItem, Url = url },
            cancellationToken
        );
}
