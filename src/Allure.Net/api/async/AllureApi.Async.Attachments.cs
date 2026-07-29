using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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
    public static Task AddAttachmentFromFileAsync(string path) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Async.AddAttachmentFromFileAsync(
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
    public static Task AddAttachmentFromFileAsync(string path, CancellationToken cancellationToken) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Async.AddAttachmentFromFileAsync(
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
    public static Task AddAttachmentFromFileAsync(string path, string name) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Async.AddAttachmentFromFileAsync(
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
    public static Task AddAttachmentFromFileAsync(
        string path,
        string name,
        CancellationToken cancellationToken
    ) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Async.AddAttachmentFromFileAsync(
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
    public static Task AddAttachmentFromFileAsync(string path, string name, string mediaType) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Async.AddAttachmentFromFileAsync(
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
    public static Task AddAttachmentFromFileAsync(
        string path,
        string name,
        string mediaType,
        CancellationToken cancellationToken
    ) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Async.AddAttachmentFromFileAsync(
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
    public static Task AddAttachmentFromFileAsync(
        string path,
        string name,
        string? mediaType,
        string fileExtension
    ) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Async.AddAttachmentFromFileAsync(
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
    public static Task AddAttachmentFromFileAsync(
        string path,
        string name,
        string? mediaType,
        string fileExtension,
        CancellationToken cancellationToken
    ) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Async.AddAttachmentFromFileAsync(
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
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Async.AddAttachmentAsync(
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
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Async.AddAttachmentAsync(
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
    public static async Task AddAttachmentAsync(string name, ReadOnlyMemory<byte> content)
    {
        var endpoint = AllureRuntimeRouter.ResolveCurrentScope();
        if (endpoint is null)
        {
            return;
        }

        using var stream = content.AsStream();

        await endpoint.Operations.Async.AddAttachmentAsync(
            name: name,
            content: stream,
            mediaType: null,
            fileExtension: "",
            cancellationToken: default
        );
    }

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static async Task AddAttachmentAsync(
        string name,
        ReadOnlyMemory<byte> content,
        CancellationToken cancellationToken
    )
    {
        var endpoint = AllureRuntimeRouter.ResolveCurrentScope();
        if (endpoint is null)
        {
            return;
        }

        using var stream = content.AsStream();

        await endpoint.Operations.Async.AddAttachmentAsync(
            name: name,
            content: stream,
            mediaType: null,
            fileExtension: "",
            cancellationToken: cancellationToken
        );
    }

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    public static async Task AddAttachmentAsync(string name, string content)
    {
        var endpoint = AllureRuntimeRouter.ResolveCurrentScope();
        if (endpoint is null)
        {
            return;
        }

        using var stream = ToStream(content);

        await endpoint.Operations.Async.AddAttachmentAsync(
            name: name,
            content: stream,
            mediaType: null,
            fileExtension: "",
            cancellationToken: default
        );
    }

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static async Task AddAttachmentAsync(
        string name,
        string content,
        CancellationToken cancellationToken
    )
    {
        var endpoint = AllureRuntimeRouter.ResolveCurrentScope();
        if (endpoint is null)
        {
            return;
        }

        using var stream = ToStream(content);

        await endpoint.Operations.Async.AddAttachmentAsync(
            name: name,
            content: stream,
            mediaType: null,
            fileExtension: "",
            cancellationToken: cancellationToken
        );
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
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Async.AddAttachmentAsync(
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
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Async.AddAttachmentAsync(
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
    public static async Task AddAttachmentAsync(
        string name,
        ReadOnlyMemory<byte> content,
        string mediaType
    )
    {
        var endpoint = AllureRuntimeRouter.ResolveCurrentScope();
        if (endpoint is null)
        {
            return;
        }

        using var stream = content.AsStream();

        await endpoint.Operations.Async.AddAttachmentAsync(
            name: name,
            content: stream,
            mediaType: mediaType,
            fileExtension: "",
            cancellationToken: default
        );
    }

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="mediaType">A media type of the attachment.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static async Task AddAttachmentAsync(
        string name,
        ReadOnlyMemory<byte> content,
        string mediaType,
        CancellationToken cancellationToken
    )
    {
        var endpoint = AllureRuntimeRouter.ResolveCurrentScope();
        if (endpoint is null)
        {
            return;
        }

        using var stream = content.AsStream();

        await endpoint.Operations.Async.AddAttachmentAsync(
            name: name,
            content: stream,
            mediaType: mediaType,
            fileExtension: "",
            cancellationToken: cancellationToken
        );
    }

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="mediaType">A media type of the attachment.</param>
    public static async Task AddAttachmentAsync(
        string name,
        string content,
        string mediaType
    )
    {
        var endpoint = AllureRuntimeRouter.ResolveCurrentScope();
        if (endpoint is null)
        {
            return;
        }

        using var stream = ToStream(content);

        await endpoint.Operations.Async.AddAttachmentAsync(
            name: name,
            content: stream,
            mediaType: mediaType,
            fileExtension: "",
            cancellationToken: default
        );
    }

    /// <summary>
    /// Adds an attachment to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="content">The content of the attachment.</param>
    /// <param name="mediaType">A media type of the attachment.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static async Task AddAttachmentAsync(
        string name,
        string content,
        string mediaType,
        CancellationToken cancellationToken
    )
    {
        var endpoint = AllureRuntimeRouter.ResolveCurrentScope();
        if (endpoint is null)
        {
            return;
        }

        using var stream = ToStream(content);

        await endpoint.Operations.Async.AddAttachmentAsync(
            name: name,
            content: stream,
            mediaType: mediaType,
            fileExtension: "",
            cancellationToken: cancellationToken
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
    public static Task AddAttachmentAsync(
        string name,
        Stream content,
        string? mediaType,
        string fileExtension
    ) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Async.AddAttachmentAsync(
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
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Async.AddAttachmentAsync(
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
    public static async Task AddAttachmentAsync(
        string name,
        ReadOnlyMemory<byte> content,
        string? mediaType,
        string fileExtension
    )
    {
        var endpoint = AllureRuntimeRouter.ResolveCurrentScope();
        if (endpoint is null)
        {
            return;
        }

        using var stream = content.AsStream();

        await endpoint.Operations.Async.AddAttachmentAsync(
            name: name,
            content: stream,
            mediaType: mediaType,
            fileExtension: fileExtension,
            cancellationToken: default
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
    /// <param name="cancellationToken">A cancellation token.</param>
    public static async Task AddAttachmentAsync(
        string name,
        ReadOnlyMemory<byte> content,
        string? mediaType,
        string fileExtension,
        CancellationToken cancellationToken
    )
    {
        var endpoint = AllureRuntimeRouter.ResolveCurrentScope();
        if (endpoint is null)
        {
            return;
        }

        using var stream = content.AsStream();

        await endpoint.Operations.Async.AddAttachmentAsync(
            name: name,
            content: stream,
            mediaType: mediaType,
            fileExtension: fileExtension,
            cancellationToken: cancellationToken
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
    public static async Task AddAttachmentAsync(
        string name,
        string content,
        string? mediaType,
        string fileExtension
    )
    {
        var endpoint = AllureRuntimeRouter.ResolveCurrentScope();
        if (endpoint is null)
        {
            return;
        }

        using var stream = ToStream(content);

        await endpoint.Operations.Async.AddAttachmentAsync(
            name: name,
            content: stream,
            mediaType: mediaType,
            fileExtension: fileExtension,
            cancellationToken: default
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
    /// <param name="cancellationToken">A cancellation token.</param>
    public static async Task AddAttachmentAsync(
        string name,
        string content,
        string? mediaType,
        string fileExtension,
        CancellationToken cancellationToken
    )
    {
        var endpoint = AllureRuntimeRouter.ResolveCurrentScope();
        if (endpoint is null)
        {
            return;
        }

        using var stream = ToStream(content);

        await endpoint.Operations.Async.AddAttachmentAsync(
            name: name,
            content: stream,
            mediaType: mediaType,
            fileExtension: fileExtension,
            cancellationToken: cancellationToken
        );
    }

    /// <summary>
    /// Attaches screen diff images to the current fixture, test, or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="expectedScreenPath">A path to the actual screen.</param>
    /// <param name="actuaScreenPath">A path to the expected screen.</param>
    /// <param name="screenDiffPath">A path to the screen diff.</param>
    public static Task AddScreenDiffFromFilesAsync(
        string expectedScreenPath,
        string actuaScreenPath,
        string screenDiffPath
    ) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Async.AddScreenDiffFromFilesAsync(
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
    public static Task AddScreenDiffFromFilesAsync(
        string expectedScreenPath,
        string actuaScreenPath,
        string screenDiffPath,
        CancellationToken cancellationToken
    ) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Async.AddScreenDiffFromFilesAsync(
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
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Async.AddScreenDiffAsync(
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
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Async.AddScreenDiffAsync(
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
    public static async Task AddScreenDiffAsync(
        ReadOnlyMemory<byte> expected,
        ReadOnlyMemory<byte> actual,
        ReadOnlyMemory<byte> diff
    )
    {
        var endpoint = AllureRuntimeRouter.ResolveCurrentScope();
        if (endpoint is null)
        {
            return;
        }

        using var expectedStream = expected.AsStream();
        using var actualStream = actual.AsStream();
        using var diffStream = diff.AsStream();

        await endpoint.Operations.Async.AddScreenDiffAsync(
            expected: expectedStream,
            actual: actualStream,
            diff: diffStream,
            cancellationToken: default
        );
    }


    /// <summary>
    /// Attaches screen diff images to the current fixture, test, or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="expected">An actual screen bytes.</param>
    /// <param name="actual">An expected screen bytes.</param>
    /// <param name="diff">A screen diff bytes.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static async Task AddScreenDiffAsync(
        ReadOnlyMemory<byte> expected,
        ReadOnlyMemory<byte> actual,
        ReadOnlyMemory<byte> diff,
        CancellationToken cancellationToken
    )
    {
        var endpoint = AllureRuntimeRouter.ResolveCurrentScope();
        if (endpoint is null)
        {
            return;
        }

        using var expectedStream = expected.AsStream();
        using var actualStream = actual.AsStream();
        using var diffStream = diff.AsStream();

        await endpoint.Operations.Async.AddScreenDiffAsync(
            expected: expectedStream,
            actual: actualStream,
            diff: diffStream,
            cancellationToken: cancellationToken
        );
    }
}
