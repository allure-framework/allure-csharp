using System;
using System.IO;
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
    public static void AddGlobalAttachmentFromFile(string path) =>
        AllureRuntimeRouter.ResolveGlobalScope()?.Operations.Sync.AddGlobalAttachmentFromFile(
            name: Path.GetFileName(path),
            mediaType: null,
            path: path,
            fileExtension: Path.GetExtension(path)
        );

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="path">The path to the attached file.</param>
    /// <param name="name">The name of the attachment.</param>
    public static void AddGlobalAttachmentFromFile(string path, string? name) =>
        AllureRuntimeRouter.ResolveGlobalScope()?.Operations.Sync.AddGlobalAttachmentFromFile(
            name: name ?? Path.GetFileName(path),
            mediaType: null,
            path: path,
            fileExtension: Path.GetExtension(path)
        );

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="mediaType">
    /// The media type of the attachment.
    /// Set to <see langword="null"/> to detect the type at report generation time.
    /// </param>
    /// <param name="path">The path to the attached file.</param>
    public static void AddGlobalAttachmentFromFile(
        string path,
        string name,
        string? mediaType
    ) =>
        AllureRuntimeRouter.ResolveGlobalScope()?.Operations.Sync.AddGlobalAttachmentFromFile(
            name: name ?? Path.GetFileName(path),
            mediaType: mediaType,
            path: path,
            fileExtension: Path.GetExtension(path)
        );

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="path">The path to the attached file.</param>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="mediaType">The media type of the attachment.</param>
    /// <param name="fileExtension">An extension of the attachment file.</param>
    ///
    public static void AddGlobalAttachmentFromFile(
        string path,
        string name,
        string? mediaType,
        string fileExtension
    ) =>
        AllureRuntimeRouter.ResolveGlobalScope()?.Operations.Sync.AddGlobalAttachmentFromFile(
            name: name ?? Path.GetFileName(path),
            mediaType: mediaType,
            path: path,
            fileExtension: fileExtension
        );

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    public static void AddGlobalAttachment(string name, Stream content)
    {
        AllureRuntimeRouter.ResolveGlobalScope()?.Operations.Sync.AddGlobalAttachment(
            name: name,
            content: content,
            mediaType: null,
            fileExtension: ""
        );
    }

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    public static void AddGlobalAttachment(
        string name,
        ReadOnlyMemory<byte> content
    )
    {
        using var stream = content.AsStream();
        AllureRuntimeRouter.ResolveGlobalScope()?.Operations.Sync.AddGlobalAttachment(
            name: name,
            content: stream,
            mediaType: null,
            fileExtension: ""
        );
    }

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="mediaType">The media type of the attachment.</param>
    public static void AddGlobalAttachment(
        string name,
        Stream content,
        string? mediaType
    )
    {
        AllureRuntimeRouter.ResolveGlobalScope()?.Operations.Sync.AddGlobalAttachment(
            name: name,
            content: content,
            mediaType: mediaType,
            fileExtension: ""
        );
    }

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="mediaType">The media type of the attachment.</param>
    public static void AddGlobalAttachment(
        string name,
        ReadOnlyMemory<byte> content,
        string? mediaType
    )
    {
        using var stream = content.AsStream();
        AllureRuntimeRouter.ResolveGlobalScope()?.Operations.Sync.AddGlobalAttachment(
            name: name,
            content: stream,
            mediaType: mediaType,
            fileExtension: ""
        );
    }

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="mediaType">The media type of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="fileExtension">An extension of the attachment file.</param>
    public static void AddGlobalAttachment(
        string name,
        Stream content,
        string? mediaType,
        string fileExtension
    )
    {
        AllureRuntimeRouter.ResolveGlobalScope()?.Operations.Sync.AddGlobalAttachment(
            name: name,
            content: content,
            mediaType: mediaType,
            fileExtension: fileExtension
        );
    }

    /// <summary>
    /// Adds a global attachment not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="mediaType">The media type of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="fileExtension">An extension of the attachment file.</param>
    public static void AddGlobalAttachment(
        string name,
        ReadOnlyMemory<byte> content,
        string? mediaType,
        string fileExtension
    )
    {
        using var stream = content.AsStream();
        AllureRuntimeRouter.ResolveGlobalScope()?.Operations.Sync.AddGlobalAttachment(
            name: name,
            content: stream,
            mediaType: mediaType,
            fileExtension: fileExtension
        );
    }

    /// <summary>
    /// Adds a global error not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="error">The error to persist.</param>
    public static void AddGlobalError(Exception error) =>
        AllureRuntimeRouter.ResolveGlobalScope()?.Operations.Sync.AddGlobalError(
            error.ToAllureGlobalError()
        );

    /// <summary>
    /// Adds a global error not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="message">The error message to persist.</param>
    public static void AddGlobalError(string message) =>
        AllureRuntimeRouter.ResolveGlobalScope()?.Operations.Sync.AddGlobalError(
            new()
            {
                Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                Message = message,
            }
        );

    /// <summary>
    /// Adds a global error not tied to the current fixture, test, or step.
    /// </summary>
    /// <param name="statusDetails">The error details to persist.</param>
    public static void AddGlobalError(StatusDetails statusDetails) =>
        AllureRuntimeRouter.ResolveGlobalScope()?.Operations.Sync.AddGlobalError(
            new()
            {
                Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                Message = statusDetails.Message,
                Trace = statusDetails.Trace,
                Flaky = statusDetails.Flaky,
                Known = statusDetails.Known,
                Muted = statusDetails.Muted,
            }
        );
}
