using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Allure.Internal;
using Allure.Model;
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
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="path">The path to the attached file.</param>
    public static Task AddGlobalFileAttachmentAsync(string path) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddGlobalFileAttachmentAsync(
            name: Path.GetFileName(path),
            mediaType: null,
            path: path,
            fileExtension: Path.GetExtension(path),
            cancellationToken: default
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="path">The path to the attached file.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddGlobalFileAttachmentAsync(
        string path,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddGlobalFileAttachmentAsync(
            name: Path.GetFileName(path),
            mediaType: null,
            path: path,
            fileExtension: Path.GetExtension(path),
            cancellationToken: cancellationToken
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="path">The path to the attached file.</param>
    /// <param name="name">The name of the attachment.</param>
    public static Task AddGlobalFileAttachmentAsync(string path, string name) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddGlobalFileAttachmentAsync(
            name: name ?? Path.GetFileName(path),
            mediaType: null,
            path: path,
            fileExtension: Path.GetExtension(path),
            cancellationToken: default
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="path">The path to the attached file.</param>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddGlobalFileAttachmentAsync(
        string path,
        string name,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddGlobalFileAttachmentAsync(
            name: name ?? Path.GetFileName(path),
            mediaType: null,
            path: path,
            fileExtension: Path.GetExtension(path),
            cancellationToken: cancellationToken
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="mediaType">
    /// The media type of the attachment.
    /// Set to <see langword="null"/> to detect the type at report generation time.
    /// </param>
    /// <param name="path">The path to the attached file.</param>
    public static Task AddGlobalFileAttachmentAsync(
        string path,
        string name,
        string mediaType
    ) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddGlobalFileAttachmentAsync(
            name: name ?? Path.GetFileName(path),
            mediaType: mediaType,
            path: path,
            fileExtension: Path.GetExtension(path),
            cancellationToken: default
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="mediaType">
    /// The media type of the attachment.
    /// Set to <see langword="null"/> to detect the type at report generation time.
    /// </param>
    /// <param name="path">The path to the attached file.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddGlobalFileAttachmentAsync(
        string path,
        string name,
        string mediaType,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddGlobalFileAttachmentAsync(
            name: name ?? Path.GetFileName(path),
            mediaType: mediaType,
            path: path,
            fileExtension: Path.GetExtension(path),
            cancellationToken: cancellationToken
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="path">The path to the attached file.</param>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="mediaType">The media type of the attachment.</param>
    /// <param name="fileExtension">An extension of the attachment file.</param>
    ///
    public static Task AddGlobalFileAttachmentAsync(
        string path,
        string name,
        string mediaType,
        string fileExtension
    ) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddGlobalFileAttachmentAsync(
            name: name ?? Path.GetFileName(path),
            mediaType: mediaType,
            path: path,
            fileExtension: fileExtension,
            cancellationToken: default
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="path">The path to the attached file.</param>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="mediaType">The media type of the attachment.</param>
    /// <param name="fileExtension">An extension of the attachment file.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    ///
    public static Task AddGlobalFileAttachmentAsync(
        string path,
        string name,
        string mediaType,
        string fileExtension,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddGlobalFileAttachmentAsync(
            name: name ?? Path.GetFileName(path),
            mediaType: mediaType,
            path: path,
            fileExtension: fileExtension,
            cancellationToken: cancellationToken
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    public static Task AddGlobalAttachmentAsync(string name, Stream content) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddGlobalAttachmentAsync(
            name: name,
            content: content,
            mediaType: null,
            fileExtension: "",
            cancellationToken: default
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddGlobalAttachmentAsync(
        string name,
        Stream content,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddGlobalAttachmentAsync(
            name: name,
            content: content,
            mediaType: null,
            fileExtension: "",
            cancellationToken: cancellationToken
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    public static Task AddGlobalAttachmentAsync(
        string name,
        ReadOnlyMemory<byte> content
    )
    {
        using var stream = content.AsStream();
        return AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddGlobalAttachmentAsync(
            name: name,
            content: stream,
            mediaType: null,
            fileExtension: "",
            cancellationToken: default
        ) ?? Task.CompletedTask;
    }

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddGlobalAttachmentAsync(
        string name,
        ReadOnlyMemory<byte> content,
        CancellationToken cancellationToken
    )
    {
        using var stream = content.AsStream();
        return AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddGlobalAttachmentAsync(
            name: name,
            content: stream,
            mediaType: null,
            fileExtension: "",
            cancellationToken: cancellationToken
        ) ?? Task.CompletedTask;
    }

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    public static Task AddGlobalAttachmentAsync(
        string name,
        string content
    )
    {
        using var stream = ToStream(content);
        return AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddGlobalAttachmentAsync(
            name: name,
            content: stream,
            mediaType: null,
            fileExtension: "",
            cancellationToken: default
        ) ?? Task.CompletedTask;
    }

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddGlobalAttachmentAsync(
        string name,
        string content,
        CancellationToken cancellationToken
    )
    {
        using var stream = ToStream(content);
        return AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddGlobalAttachmentAsync(
            name: name,
            content: stream,
            mediaType: null,
            fileExtension: "",
            cancellationToken: cancellationToken
        ) ?? Task.CompletedTask;
    }

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="mediaType">The media type of the attachment.</param>
    public static Task AddGlobalAttachmentAsync(
        string name,
        Stream content,
        string mediaType
    ) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddGlobalAttachmentAsync(
            name: name,
            content: content,
            mediaType: mediaType,
            fileExtension: "",
            cancellationToken: default
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="mediaType">The media type of the attachment.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddGlobalAttachmentAsync(
        string name,
        Stream content,
        string mediaType,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddGlobalAttachmentAsync(
            name: name,
            content: content,
            mediaType: mediaType,
            fileExtension: "",
            cancellationToken: cancellationToken
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="mediaType">The media type of the attachment.</param>
    public static Task AddGlobalAttachmentAsync(
        string name,
        ReadOnlyMemory<byte> content,
        string mediaType
    )
    {
        using var stream = content.AsStream();
        return AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddGlobalAttachmentAsync(
            name: name,
            content: stream,
            mediaType: mediaType,
            fileExtension: "",
            cancellationToken: default
        ) ?? Task.CompletedTask;
    }

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="mediaType">The media type of the attachment.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddGlobalAttachmentAsync(
        string name,
        ReadOnlyMemory<byte> content,
        string mediaType,
        CancellationToken cancellationToken
    )
    {
        using var stream = content.AsStream();
        return AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddGlobalAttachmentAsync(
            name: name,
            content: stream,
            mediaType: mediaType,
            fileExtension: "",
            cancellationToken: cancellationToken
        ) ?? Task.CompletedTask;
    }

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="mediaType">The media type of the attachment.</param>
    public static Task AddGlobalAttachmentAsync(
        string name,
        string content,
        string mediaType
    )
    {
        using var stream = ToStream(content);
        return AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddGlobalAttachmentAsync(
            name: name,
            content: stream,
            mediaType: mediaType,
            fileExtension: "",
            cancellationToken: default
        ) ?? Task.CompletedTask;
    }

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="mediaType">The media type of the attachment.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddGlobalAttachmentAsync(
        string name,
        string content,
        string mediaType,
        CancellationToken cancellationToken
    )
    {
        using var stream = ToStream(content);
        return AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddGlobalAttachmentAsync(
            name: name,
            content: stream,
            mediaType: mediaType,
            fileExtension: "",
            cancellationToken: cancellationToken
        ) ?? Task.CompletedTask;
    }

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="mediaType">The media type of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="fileExtension">An extension of the attachment file.</param>
    public static Task AddGlobalAttachmentAsync(
        string name,
        Stream content,
        string mediaType,
        string fileExtension
    ) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddGlobalAttachmentAsync(
            name: name,
            content: content,
            mediaType: mediaType,
            fileExtension: fileExtension,
            cancellationToken: default
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="mediaType">The media type of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="fileExtension">An extension of the attachment file.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddGlobalAttachmentAsync(
        string name,
        Stream content,
        string mediaType,
        string fileExtension,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddGlobalAttachmentAsync(
            name: name,
            content: content,
            mediaType: mediaType,
            fileExtension: fileExtension,
            cancellationToken: cancellationToken
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="mediaType">The media type of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="fileExtension">An extension of the attachment file.</param>
    public static Task AddGlobalAttachmentAsync(
        string name,
        ReadOnlyMemory<byte> content,
        string mediaType,
        string fileExtension
    )
    {
        using var stream = content.AsStream();
        return AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddGlobalAttachmentAsync(
            name: name,
            content: stream,
            mediaType: mediaType,
            fileExtension: fileExtension,
            cancellationToken: default
        ) ?? Task.CompletedTask;
    }

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="mediaType">The media type of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="fileExtension">An extension of the attachment file.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddGlobalAttachmentAsync(
        string name,
        ReadOnlyMemory<byte> content,
        string mediaType,
        string fileExtension,
        CancellationToken cancellationToken
    )
    {
        using var stream = content.AsStream();
        return AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddGlobalAttachmentAsync(
            name: name,
            content: stream,
            mediaType: mediaType,
            fileExtension: fileExtension,
            cancellationToken: cancellationToken
        ) ?? Task.CompletedTask;
    }

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="mediaType">The media type of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="fileExtension">An extension of the attachment file.</param>
    public static Task AddGlobalAttachmentAsync(
        string name,
        string content,
        string mediaType,
        string fileExtension
    )
    {
        using var stream = ToStream(content);
        return AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddGlobalAttachmentAsync(
            name: name,
            content: stream,
            mediaType: mediaType,
            fileExtension: fileExtension,
            cancellationToken: default
        ) ?? Task.CompletedTask;
    }

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="mediaType">The media type of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="fileExtension">An extension of the attachment file.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddGlobalAttachmentAsync(
        string name,
        string content,
        string mediaType,
        string fileExtension,
        CancellationToken cancellationToken
    )
    {
        using var stream = ToStream(content);
        return AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddGlobalAttachmentAsync(
            name: name,
            content: stream,
            mediaType: mediaType,
            fileExtension: fileExtension,
            cancellationToken: cancellationToken
        ) ?? Task.CompletedTask;
    }

    /// <summary>
    /// Adds a global error not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="error">The error to persist.</param>
    public static Task AddGlobalErrorAsync(Exception error) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddGlobalErrorAsync(
            error.ToAllureGlobalError(),
            cancellationToken: default
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Adds a global error not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="error">The error to persist.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddGlobalErrorAsync(Exception error, CancellationToken cancellationToken) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddGlobalErrorAsync(
            error.ToAllureGlobalError(),
            cancellationToken: cancellationToken
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Adds a global error not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="message">The error message to persist.</param>
    public static Task AddGlobalErrorAsync(string message) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddGlobalErrorAsync(
            error: new()
            {
                Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                Message = message,
            },
            cancellationToken: default
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Adds a global error not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="message">The error message to persist.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddGlobalErrorAsync(string message, CancellationToken cancellationToken) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddGlobalErrorAsync(
            error: new()
            {
                Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                Message = message,
            },
            cancellationToken: cancellationToken
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Adds a global error not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="statusDetails">The error details to persist.</param>
    public static Task AddGlobalErrorAsync(StatusDetails statusDetails) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddGlobalErrorAsync(
            error: new()
            {
                Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                Message = statusDetails.Message,
                Trace = statusDetails.Trace,
                Flaky = statusDetails.Flaky,
                Known = statusDetails.Known,
                Muted = statusDetails.Muted,
            },
            cancellationToken: default
        ) ?? Task.CompletedTask;


    /// <summary>
    /// Adds a global error not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="statusDetails">The error details to persist.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddGlobalErrorAsync(
        StatusDetails statusDetails,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.AddGlobalErrorAsync(
            error: new()
            {
                Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                Message = statusDetails.Message,
                Trace = statusDetails.Trace,
                Flaky = statusDetails.Flaky,
                Known = statusDetails.Known,
                Muted = statusDetails.Muted,
            },
            cancellationToken: cancellationToken
        ) ?? Task.CompletedTask;
}
