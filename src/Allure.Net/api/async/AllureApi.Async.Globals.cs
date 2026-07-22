using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Allure.Internal;
using Allure.Model;
using Allure.Runtime;
using CommunityToolkit.HighPerformance;

namespace Allure;

public static partial class AllureApi
{
    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="path">The path to the attached file.</param>
    public static Task AddGlobalFileAttachmentAsync(string path) =>
        AllureRuntimeRouter.ResolveGlobalScope()?.Operations.Async.AddGlobalFileAttachmentAsync(
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
        AllureRuntimeRouter.ResolveGlobalScope()?.Operations.Async.AddGlobalFileAttachmentAsync(
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
        AllureRuntimeRouter.ResolveGlobalScope()?.Operations.Async.AddGlobalFileAttachmentAsync(
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
        AllureRuntimeRouter.ResolveGlobalScope()?.Operations.Async.AddGlobalFileAttachmentAsync(
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
        AllureRuntimeRouter.ResolveGlobalScope()?.Operations.Async.AddGlobalFileAttachmentAsync(
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
        AllureRuntimeRouter.ResolveGlobalScope()?.Operations.Async.AddGlobalFileAttachmentAsync(
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
        AllureRuntimeRouter.ResolveGlobalScope()?.Operations.Async.AddGlobalFileAttachmentAsync(
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
        AllureRuntimeRouter.ResolveGlobalScope()?.Operations.Async.AddGlobalFileAttachmentAsync(
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
        AllureRuntimeRouter.ResolveGlobalScope()?.Operations.Async.AddGlobalAttachmentAsync(
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
        AllureRuntimeRouter.ResolveGlobalScope()?.Operations.Async.AddGlobalAttachmentAsync(
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
    public static async Task AddGlobalAttachmentAsync(
        string name,
        ReadOnlyMemory<byte> content
    )
    {
        var endpoint = AllureRuntimeRouter.ResolveGlobalScope();
        if (endpoint is null)
        {
            return;
        }

        using var stream = content.AsStream();

        await endpoint.Operations.Async.AddGlobalAttachmentAsync(
            name: name,
            content: stream,
            mediaType: null,
            fileExtension: "",
            cancellationToken: default
        );
    }

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static async Task AddGlobalAttachmentAsync(
        string name,
        ReadOnlyMemory<byte> content,
        CancellationToken cancellationToken
    )
    {
        var endpoint = AllureRuntimeRouter.ResolveGlobalScope();
        if (endpoint is null)
        {
            return;
        }

        using var stream = content.AsStream();

        await endpoint.Operations.Async.AddGlobalAttachmentAsync(
            name: name,
            content: stream,
            mediaType: null,
            fileExtension: "",
            cancellationToken: cancellationToken
        );
    }

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    public static async Task AddGlobalAttachmentAsync(
        string name,
        string content
    )
    {
        var endpoint = AllureRuntimeRouter.ResolveGlobalScope();
        if (endpoint is null)
        {
            return;
        }

        using var stream = ToStream(content);

        await endpoint.Operations.Async.AddGlobalAttachmentAsync(
            name: name,
            content: stream,
            mediaType: null,
            fileExtension: "",
            cancellationToken: default
        );
    }

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static async Task AddGlobalAttachmentAsync(
        string name,
        string content,
        CancellationToken cancellationToken
    )
    {
        var endpoint = AllureRuntimeRouter.ResolveGlobalScope();
        if (endpoint is null)
        {
            return;
        }

        using var stream = ToStream(content);

        await endpoint.Operations.Async.AddGlobalAttachmentAsync(
            name: name,
            content: stream,
            mediaType: null,
            fileExtension: "",
            cancellationToken: cancellationToken
        );
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
        AllureRuntimeRouter.ResolveGlobalScope()?.Operations.Async.AddGlobalAttachmentAsync(
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
        AllureRuntimeRouter.ResolveGlobalScope()?.Operations.Async.AddGlobalAttachmentAsync(
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
    public static async Task AddGlobalAttachmentAsync(
        string name,
        ReadOnlyMemory<byte> content,
        string mediaType
    )
    {
        var endpoint = AllureRuntimeRouter.ResolveGlobalScope();
        if (endpoint is null)
        {
            return;
        }

        using var stream = content.AsStream();

        await endpoint.Operations.Async.AddGlobalAttachmentAsync(
            name: name,
            content: stream,
            mediaType: mediaType,
            fileExtension: "",
            cancellationToken: default
        );
    }

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="mediaType">The media type of the attachment.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static async Task AddGlobalAttachmentAsync(
        string name,
        ReadOnlyMemory<byte> content,
        string mediaType,
        CancellationToken cancellationToken
    )
    {
        var endpoint = AllureRuntimeRouter.ResolveGlobalScope();
        if (endpoint is null)
        {
            return;
        }

        using var stream = content.AsStream();

        await endpoint.Operations.Async.AddGlobalAttachmentAsync(
            name: name,
            content: stream,
            mediaType: mediaType,
            fileExtension: "",
            cancellationToken: cancellationToken
        );
    }

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="mediaType">The media type of the attachment.</param>
    public static async Task AddGlobalAttachmentAsync(
        string name,
        string content,
        string mediaType
    )
    {
        var endpoint = AllureRuntimeRouter.ResolveGlobalScope();
        if (endpoint is null)
        {
            return;
        }

        using var stream = ToStream(content);

        await endpoint.Operations.Async.AddGlobalAttachmentAsync(
            name: name,
            content: stream,
            mediaType: mediaType,
            fileExtension: "",
            cancellationToken: default
        );
    }

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="mediaType">The media type of the attachment.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static async Task AddGlobalAttachmentAsync(
        string name,
        string content,
        string mediaType,
        CancellationToken cancellationToken
    )
    {
        var endpoint = AllureRuntimeRouter.ResolveGlobalScope();
        if (endpoint is null)
        {
            return;
        }

        using var stream = ToStream(content);

        await endpoint.Operations.Async.AddGlobalAttachmentAsync(
            name: name,
            content: stream,
            mediaType: mediaType,
            fileExtension: "",
            cancellationToken: cancellationToken
        );
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
        AllureRuntimeRouter.ResolveGlobalScope()?.Operations.Async.AddGlobalAttachmentAsync(
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
        AllureRuntimeRouter.ResolveGlobalScope()?.Operations.Async.AddGlobalAttachmentAsync(
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
    public static async Task AddGlobalAttachmentAsync(
        string name,
        ReadOnlyMemory<byte> content,
        string mediaType,
        string fileExtension
    )
    {
        var endpoint = AllureRuntimeRouter.ResolveGlobalScope();
        if (endpoint is null)
        {
            return;
        }

        using var stream = content.AsStream();

        await endpoint.Operations.Async.AddGlobalAttachmentAsync(
            name: name,
            content: stream,
            mediaType: mediaType,
            fileExtension: fileExtension,
            cancellationToken: default
        );
    }

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="mediaType">The media type of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="fileExtension">An extension of the attachment file.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static async Task AddGlobalAttachmentAsync(
        string name,
        ReadOnlyMemory<byte> content,
        string mediaType,
        string fileExtension,
        CancellationToken cancellationToken
    )
    {
        var endpoint = AllureRuntimeRouter.ResolveGlobalScope();
        if (endpoint is null)
        {
            return;
        }

        using var stream = content.AsStream();

        await endpoint.Operations.Async.AddGlobalAttachmentAsync(
            name: name,
            content: stream,
            mediaType: mediaType,
            fileExtension: fileExtension,
            cancellationToken: cancellationToken
        );
    }

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="mediaType">The media type of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="fileExtension">An extension of the attachment file.</param>
    public static async Task AddGlobalAttachmentAsync(
        string name,
        string content,
        string mediaType,
        string fileExtension
    )
    {
        var endpoint = AllureRuntimeRouter.ResolveGlobalScope();
        if (endpoint is null)
        {
            return;
        }

        using var stream = ToStream(content);

        await endpoint.Operations.Async.AddGlobalAttachmentAsync(
            name: name,
            content: stream,
            mediaType: mediaType,
            fileExtension: fileExtension,
            cancellationToken: default
        );
    }

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="mediaType">The media type of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="fileExtension">An extension of the attachment file.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static async Task AddGlobalAttachmentAsync(
        string name,
        string content,
        string mediaType,
        string fileExtension,
        CancellationToken cancellationToken
    )
    {
        var endpoint = AllureRuntimeRouter.ResolveGlobalScope();
        if (endpoint is null)
        {
            return;
        }

        using var stream = ToStream(content);

        await endpoint.Operations.Async.AddGlobalAttachmentAsync(
            name: name,
            content: stream,
            mediaType: mediaType,
            fileExtension: fileExtension,
            cancellationToken: cancellationToken
        );
    }

    /// <summary>
    /// Adds a global error not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="error">The error to persist.</param>
    public static Task AddGlobalErrorAsync(Exception error) =>
        AllureRuntimeRouter.ResolveGlobalScope()?.Operations.Async.AddGlobalErrorAsync(
            error.ToAllureGlobalError(),
            cancellationToken: default
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Adds a global error not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="error">The error to persist.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task AddGlobalErrorAsync(Exception error, CancellationToken cancellationToken) =>
        AllureRuntimeRouter.ResolveGlobalScope()?.Operations.Async.AddGlobalErrorAsync(
            error.ToAllureGlobalError(),
            cancellationToken: cancellationToken
        ) ?? Task.CompletedTask;

    /// <summary>
    /// Adds a global error not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="message">The error message to persist.</param>
    public static Task AddGlobalErrorAsync(string message) =>
        AllureRuntimeRouter.ResolveGlobalScope()?.Operations.Async.AddGlobalErrorAsync(
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
        AllureRuntimeRouter.ResolveGlobalScope()?.Operations.Async.AddGlobalErrorAsync(
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
        AllureRuntimeRouter.ResolveGlobalScope()?.Operations.Async.AddGlobalErrorAsync(
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
        AllureRuntimeRouter.ResolveGlobalScope()?.Operations.Async.AddGlobalErrorAsync(
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
