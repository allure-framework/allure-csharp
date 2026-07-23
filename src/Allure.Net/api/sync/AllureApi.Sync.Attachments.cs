using System;
using System.IO;
using Allure.Runtime;
using CommunityToolkit.HighPerformance;

namespace Allure;

public static partial class AllureApi
{
    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="path">The path to the attached file.</param>
    public static void AddFileAttachment(string path) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Sync.AddFileAttachment(
            name: Path.GetFileName(path),
            path: path,
            mediaType: null,
            fileExtension: ""
        );

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="path">The path to the attached file.</param>
    /// <param name="name">A display name of the attachment.</param>
    public static void AddFileAttachment(string path, string? name) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Sync.AddFileAttachment(
            name: name ?? Path.GetFileName(path),
            path: path,
            mediaType: null,
            fileExtension: ""
        );

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="path">The path to the attached file.</param>
    /// <param name="name">A display name of the attachment.</param>
    /// <param name="mediaType">A media type of the attachment.</param>
    public static void AddFileAttachment(string path, string name, string? mediaType) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Sync.AddFileAttachment(
            name: name ?? Path.GetFileName(path),
            path: path,
            mediaType: mediaType,
            fileExtension: ""
        );

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="path">The path to the attached file.</param>
    /// <param name="name">A display name of the attachment.</param>
    /// <param name="mediaType">A media type of the attachment.</param>
    /// <param name="fileExtension">An extension of the attachment file.</param>
    public static void AddFileAttachment(
        string path,
        string name,
        string? mediaType,
        string fileExtension
    ) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Sync.AddFileAttachment(
            name: name ?? Path.GetFileName(path),
            path: path,
            mediaType: mediaType,
            fileExtension: fileExtension
        );

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    public static void AddAttachment(string name, Stream content)
    {
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Sync.AddAttachment(
            name: name,
            content: content,
            mediaType: null,
            fileExtension: ""
        );
    }

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    public static void AddAttachment(string name, ReadOnlyMemory<byte> content)
    {
        using var stream = content.AsStream();
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Sync.AddAttachment(
            name: name,
            content: stream,
            mediaType: null,
            fileExtension: ""
        );
    }

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    public static void AddAttachment(string name, string content)
    {
        using var stream = ToStream(content);
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Sync.AddAttachment(
            name: name,
            content: stream,
            mediaType: null,
            fileExtension: ""
        );
    }

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="mediaType">A media type of the attachment.</param>
    public static void AddAttachment(
        string name,
        Stream content,
        string? mediaType
    )
    {
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Sync.AddAttachment(
            name: name,
            content: content,
            mediaType: mediaType,
            fileExtension: ""
        );
    }

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="mediaType">A media type of the attachment.</param>
    public static void AddAttachment(
        string name,
        ReadOnlyMemory<byte> content,
        string? mediaType
    )
    {
        using var stream = content.AsStream();
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Sync.AddAttachment(
            name: name,
            content: stream,
            mediaType: mediaType,
            fileExtension: ""
        );
    }

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="mediaType">A media type of the attachment.</param>
    public static void AddAttachment(
        string name,
        string content,
        string? mediaType
    )
    {
        using var stream = ToStream(content);
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Sync.AddAttachment(
            name: name,
            content: stream,
            mediaType: mediaType,
            fileExtension: ""
        );
    }

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="mediaType">A media type of the attachment.</param>
    /// <param name="fileExtension">An extension of the attachment file.</param>
    public static void AddAttachment(
        string name,
        Stream content,
        string? mediaType,
        string fileExtension
    )
    {
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Sync.AddAttachment(
            name: name,
            content: content,
            mediaType: mediaType,
            fileExtension: fileExtension
        );
    }

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="mediaType">A media type of the attachment.</param>
    /// <param name="fileExtension">An extension of the attachment file.</param>
    public static void AddAttachment(
        string name,
        ReadOnlyMemory<byte> content,
        string? mediaType,
        string fileExtension
    )
    {
        using var stream = content.AsStream();
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Sync.AddAttachment(
            name: name,
            content: stream,
            mediaType: mediaType,
            fileExtension: fileExtension
        );
    }

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="mediaType">A media type of the attachment.</param>
    /// <param name="fileExtension">An extension of the attachment file.</param>
    public static void AddAttachment(
        string name,
        string content,
        string? mediaType,
        string fileExtension
    )
    {
        using var stream = ToStream(content);
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Sync.AddAttachment(
            name: name,
            content: stream,
            mediaType: mediaType,
            fileExtension: fileExtension
        );
    }

    /// <summary>
    /// Attaches screen diff images to the current fixture, test, or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="expectedScreenPath">A path to the actual screen.</param>
    /// <param name="actuaScreenPath">A path to the expected screen.</param>
    /// <param name="screenDiffPath">A path to the screen diff.</param>
    public static void AddFileScreenDiff(
        string expectedScreenPath,
        string actuaScreenPath,
        string screenDiffPath
    ) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Sync.AddFileScreenDiff(
            expectedPath: expectedScreenPath,
            actualPath: actuaScreenPath,
            diffPath: screenDiffPath
        );

    /// <summary>
    /// Attaches screen diff images to the current fixture, test, or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="expected">An actual screen bytes.</param>
    /// <param name="actual">An expected screen bytes.</param>
    /// <param name="diff">A screen diff bytes.</param>
    public static void AddScreenDiff(
        Stream expected,
        Stream actual,
        Stream diff
    )
    {
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Sync.AddScreenDiff(expected, actual, diff);
    }

    /// <summary>
    /// Attaches screen diff images to the current fixture, test, or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="expected">An actual screen bytes.</param>
    /// <param name="actual">An expected screen bytes.</param>
    /// <param name="diff">A screen diff bytes.</param>
    public static void AddScreenDiff(
        ReadOnlyMemory<byte> expected,
        ReadOnlyMemory<byte> actual,
        ReadOnlyMemory<byte> diff
    )
    {
        using var expectedStream = expected.AsStream();
        using var actualStream = actual.AsStream();
        using var diffStream = diff.AsStream();

        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Sync.AddScreenDiff(
            expectedStream,
            actualStream,
            diffStream
        );
    }
}
