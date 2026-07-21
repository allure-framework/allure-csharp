using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Allure.Runtime;
using CommunityToolkit.HighPerformance;

namespace Allure;

/// <summary>
/// A facade that provides the API for test authors to enhance the Allure
/// report.
/// </summary>
public static partial class AllureApi
{
    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="path">The path to the attached file.</param>
    public static Task AddFileAttachmentAsync(string path) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddFileAttachmentAsync(
            name: Path.GetFileName(path),
            path: path,
            mediaType: null,
            fileExtension: "",
            cancellationToken: default
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="path">The path to the attached file.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddFileAttachmentAsync(string path, CancellationToken cancellationToken) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddFileAttachmentAsync(
            name: Path.GetFileName(path),
            path: path,
            mediaType: null,
            fileExtension: "",
            cancellationToken: cancellationToken
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="path">The path to the attached file.</param>
    /// <param name="name">A display name of the attachment.</param>
    public static Task AddFileAttachmentAsync(string path, string name) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddFileAttachmentAsync(
            name: name ?? Path.GetFileName(path),
            path: path,
            mediaType: null,
            fileExtension: "",
            cancellationToken: default
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="path">The path to the attached file.</param>
    /// <param name="name">A display name of the attachment.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddFileAttachmentAsync(
        string path,
        string name,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddFileAttachmentAsync(
            name: name ?? Path.GetFileName(path),
            path: path,
            mediaType: null,
            fileExtension: "",
            cancellationToken: cancellationToken
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="path">The path to the attached file.</param>
    /// <param name="name">A display name of the attachment.</param>
    /// <param name="mediaType">A media type of the attachment.</param>
    public static Task AddFileAttachmentAsync(string path, string name, string mediaType) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddFileAttachmentAsync(
            name: name ?? Path.GetFileName(path),
            path: path,
            mediaType: mediaType,
            fileExtension: "",
            cancellationToken: default
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="path">The path to the attached file.</param>
    /// <param name="name">A display name of the attachment.</param>
    /// <param name="mediaType">A media type of the attachment.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddFileAttachmentAsync(
        string path,
        string name,
        string mediaType,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddFileAttachmentAsync(
            name: name ?? Path.GetFileName(path),
            path: path,
            mediaType: mediaType,
            fileExtension: "",
            cancellationToken: cancellationToken
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="path">The path to the attached file.</param>
    /// <param name="name">A display name of the attachment.</param>
    /// <param name="mediaType">A media type of the attachment.</param>
    /// <param name="fileExtension">An extension of the attachment file.</param>
    public static Task AddFileAttachmentAsync(
        string path,
        string name,
        string? mediaType,
        string fileExtension
    ) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddFileAttachmentAsync(
            name: name ?? Path.GetFileName(path),
            path: path,
            mediaType: mediaType,
            fileExtension: fileExtension,
            cancellationToken: default
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="path">The path to the attached file.</param>
    /// <param name="name">A display name of the attachment.</param>
    /// <param name="mediaType">A media type of the attachment.</param>
    /// <param name="fileExtension">An extension of the attachment file.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddFileAttachmentAsync(
        string path,
        string name,
        string? mediaType,
        string fileExtension,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddFileAttachmentAsync(
            name: name ?? Path.GetFileName(path),
            path: path,
            mediaType: mediaType,
            fileExtension: fileExtension,
            cancellationToken: cancellationToken
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    public static Task AddAttachmentAsync(string name, Stream content) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddAttachmentAsync(
            name: name,
            content: content,
            mediaType: null,
            fileExtension: "",
            cancellationToken: default
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddAttachmentAsync(
        string name,
        Stream content,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddAttachmentAsync(
            name: name,
            content: content,
            mediaType: null,
            fileExtension: "",
            cancellationToken: cancellationToken
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    public static Task AddAttachmentAsync(string name, ReadOnlyMemory<byte> content)
    {
        using var stream = content.AsStream();
        return AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddAttachmentAsync(
            name: name,
            content: stream,
            mediaType: null,
            fileExtension: "",
            cancellationToken: default
        ) ?? Task.CompletedTask;
    }

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddAttachmentAsync(
        string name,
        ReadOnlyMemory<byte> content,
        CancellationToken cancellationToken
    )
    {
        using var stream = content.AsStream();
        return AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddAttachmentAsync(
            name: name,
            content: stream,
            mediaType: null,
            fileExtension: "",
            cancellationToken: cancellationToken
        ) ?? Task.CompletedTask;
    }

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    public static Task AddAttachmentAsync(string name, string content)
    {
        using var stream = ToStream(content);
        return AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddAttachmentAsync(
            name: name,
            content: stream,
            mediaType: null,
            fileExtension: "",
            cancellationToken: default
        ) ?? Task.CompletedTask;
    }

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddAttachmentAsync(
        string name,
        string content,
        CancellationToken cancellationToken
    )
    {
        using var stream = ToStream(content);
        return AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddAttachmentAsync(
            name: name,
            content: stream,
            mediaType: null,
            fileExtension: "",
            cancellationToken: cancellationToken
        ) ?? Task.CompletedTask;
    }

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="mediaType">A media type of the attachment.</param>
    public static Task AddAttachmentAsync(
        string name,
        Stream content,
        string mediaType
    ) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddAttachmentAsync(
            name: name,
            content: content,
            mediaType: mediaType,
            fileExtension: "",
            cancellationToken: default
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="mediaType">A media type of the attachment.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddAttachmentAsync(
        string name,
        Stream content,
        string mediaType,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddAttachmentAsync(
            name: name,
            content: content,
            mediaType: mediaType,
            fileExtension: "",
            cancellationToken: cancellationToken
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="mediaType">A media type of the attachment.</param>
    public static Task AddAttachmentAsync(
        string name,
        ReadOnlyMemory<byte> content,
        string mediaType
    )
    {
        using var stream = content.AsStream();
        return AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddAttachmentAsync(
            name: name,
            content: stream,
            mediaType: mediaType,
            fileExtension: "",
            cancellationToken: default
        ) ?? Task.CompletedTask;
    }

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="mediaType">A media type of the attachment.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddAttachmentAsync(
        string name,
        ReadOnlyMemory<byte> content,
        string mediaType,
        CancellationToken cancellationToken
    )
    {
        using var stream = content.AsStream();
        return AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddAttachmentAsync(
            name: name,
            content: stream,
            mediaType: mediaType,
            fileExtension: "",
            cancellationToken: cancellationToken
        ) ?? Task.CompletedTask;
    }

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="mediaType">A media type of the attachment.</param>
    public static Task AddAttachmentAsync(
        string name,
        string content,
        string mediaType
    )
    {
        using var stream = ToStream(content);
        return AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddAttachmentAsync(
            name: name,
            content: stream,
            mediaType: mediaType,
            fileExtension: "",
            cancellationToken: default
        ) ?? Task.CompletedTask;
    }

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="mediaType">A media type of the attachment.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddAttachmentAsync(
        string name,
        string content,
        string mediaType,
        CancellationToken cancellationToken
    )
    {
        using var stream = ToStream(content);
        return AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddAttachmentAsync(
            name: name,
            content: stream,
            mediaType: mediaType,
            fileExtension: "",
            cancellationToken: cancellationToken
        ) ?? Task.CompletedTask;
    }

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="mediaType">A media type of the attachment.</param>
    /// <param name="fileExtension">An extension of the attachment file.</param>
    public static Task AddAttachmentAsync(
        string name,
        Stream content,
        string? mediaType,
        string fileExtension
    ) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddAttachmentAsync(
            name: name,
            content: content,
            mediaType: mediaType,
            fileExtension: fileExtension,
            cancellationToken: default
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="mediaType">A media type of the attachment.</param>
    /// <param name="fileExtension">An extension of the attachment file.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddAttachmentAsync(
        string name,
        Stream content,
        string? mediaType,
        string fileExtension,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddAttachmentAsync(
            name: name,
            content: content,
            mediaType: mediaType,
            fileExtension: fileExtension,
            cancellationToken: cancellationToken
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="mediaType">A media type of the attachment.</param>
    /// <param name="fileExtension">An extension of the attachment file.</param>
    public static Task AddAttachmentAsync(
        string name,
        ReadOnlyMemory<byte> content,
        string? mediaType,
        string fileExtension
    )
    {
        using var stream = content.AsStream();
        return AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddAttachmentAsync(
            name: name,
            content: stream,
            mediaType: mediaType,
            fileExtension: fileExtension,
            cancellationToken: default
        ) ?? Task.CompletedTask;
    }

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="mediaType">A media type of the attachment.</param>
    /// <param name="fileExtension">An extension of the attachment file.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddAttachmentAsync(
        string name,
        ReadOnlyMemory<byte> content,
        string? mediaType,
        string fileExtension,
        CancellationToken cancellationToken
    )
    {
        using var stream = content.AsStream();
        return AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddAttachmentAsync(
            name: name,
            content: stream,
            mediaType: mediaType,
            fileExtension: fileExtension,
            cancellationToken: cancellationToken
        ) ?? Task.CompletedTask;
    }

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="mediaType">A media type of the attachment.</param>
    /// <param name="fileExtension">An extension of the attachment file.</param>
    public static Task AddAttachmentAsync(
        string name,
        string content,
        string? mediaType,
        string fileExtension
    )
    {
        using var stream = ToStream(content);
        return AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddAttachmentAsync(
            name: name,
            content: stream,
            mediaType: mediaType,
            fileExtension: fileExtension,
            cancellationToken: default
        ) ?? Task.CompletedTask;
    }

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="mediaType">A media type of the attachment.</param>
    /// <param name="fileExtension">An extension of the attachment file.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddAttachmentAsync(
        string name,
        string content,
        string? mediaType,
        string fileExtension,
        CancellationToken cancellationToken
    )
    {
        using var stream = ToStream(content);
        return AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddAttachmentAsync(
            name: name,
            content: stream,
            mediaType: mediaType,
            fileExtension: fileExtension,
            cancellationToken: cancellationToken
        ) ?? Task.CompletedTask;
    }

    /// <summary>
    /// Attaches screen diff images to the current fixture, test, or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="expectedScreenPath">A path to the actual screen.</param>
    /// <param name="actuaScreenPath">A path to the expected screen.</param>
    /// <param name="screenDiffPath">A path to the screen diff.</param>
    public static Task AddFileScreenDiffAsync(
        string expectedScreenPath,
        string actuaScreenPath,
        string screenDiffPath
    ) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddFileScreenDiffAsync(
            expectedPath: expectedScreenPath,
            actualPath: actuaScreenPath,
            diffPath: screenDiffPath,
            cancellationToken: default
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Attaches screen diff images to the current fixture, test, or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="expectedScreenPath">A path to the actual screen.</param>
    /// <param name="actuaScreenPath">A path to the expected screen.</param>
    /// <param name="screenDiffPath">A path to the screen diff.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddFileScreenDiffAsync(
        string expectedScreenPath,
        string actuaScreenPath,
        string screenDiffPath,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddFileScreenDiffAsync(
            expectedPath: expectedScreenPath,
            actualPath: actuaScreenPath,
            diffPath: screenDiffPath,
            cancellationToken: cancellationToken
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Attaches screen diff images to the current fixture, test, or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="expected">An actual screen bytes.</param>
    /// <param name="actual">An expected screen bytes.</param>
    /// <param name="diff">A screen diff bytes.</param>
    public static Task AddScreenDiffAsync(
        Stream expected,
        Stream actual,
        Stream diff
    ) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddScreenDiffAsync(
            expected,
            actual,
            diff,
            default
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Attaches screen diff images to the current fixture, test, or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="expected">An actual screen bytes.</param>
    /// <param name="actual">An expected screen bytes.</param>
    /// <param name="diff">A screen diff bytes.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddScreenDiffAsync(
        Stream expected,
        Stream actual,
        Stream diff,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddScreenDiffAsync(
            expected,
            actual,
            diff,
            cancellationToken
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Attaches screen diff images to the current fixture, test, or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="expected">An actual screen bytes.</param>
    /// <param name="actual">An expected screen bytes.</param>
    /// <param name="diff">A screen diff bytes.</param>
    public static Task AddScreenDiffAsync(
        ReadOnlyMemory<byte> expected,
        ReadOnlyMemory<byte> actual,
        ReadOnlyMemory<byte> diff
    )
    {
        using var expectedStream = expected.AsStream();
        using var actualStream = actual.AsStream();
        using var diffStream = diff.AsStream();

        return AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddScreenDiffAsync(
            expected: expectedStream,
            actual: actualStream,
            diff: diffStream,
            cancellationToken: default
        ) ?? Task.CompletedTask;
    }


    /// <summary>
    /// Attaches screen diff images to the current fixture, test, or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="expected">An actual screen bytes.</param>
    /// <param name="actual">An expected screen bytes.</param>
    /// <param name="diff">A screen diff bytes.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddScreenDiffAsync(
        ReadOnlyMemory<byte> expected,
        ReadOnlyMemory<byte> actual,
        ReadOnlyMemory<byte> diff,
        CancellationToken cancellationToken
    )
    {
        using var expectedStream = expected.AsStream();
        using var actualStream = actual.AsStream();
        using var diffStream = diff.AsStream();

        return AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddScreenDiffAsync(
            expected: expectedStream,
            actual: actualStream,
            diff: diffStream,
            cancellationToken: cancellationToken
        ) ?? Task.CompletedTask;
    }
}
