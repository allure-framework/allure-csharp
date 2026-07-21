using System.Reflection;
using Allure.Abstractions;
using Allure.Model;

namespace Allure.Net.Tests.Infrastructure;

sealed record CapturedAttachment(
    string Operation,
    string Name,
    byte[]? Content,
    string? Path,
    string? MediaType,
    string FileExtension,
    CancellationToken CancellationToken
);

sealed record CapturedScreenDiff(
    string Operation,
    byte[]? Expected,
    byte[]? Actual,
    byte[]? Diff,
    string? ExpectedPath,
    string? ActualPath,
    string? DiffPath,
    CancellationToken CancellationToken
);

sealed class AttachmentRecorder
{
    public RecordingInterface<IAllureOperations<IAllureStepContext, IAllureFixtureContext>> Sync { get; } =
        RecordingInterface<IAllureOperations<IAllureStepContext, IAllureFixtureContext>>.Create();

    public RecordingInterface<IAllureAsyncOperations<IAllureAsyncStepContext, IAllureAsyncFixtureContext>> Async { get; } =
        RecordingInterface<IAllureAsyncOperations<IAllureAsyncStepContext, IAllureAsyncFixtureContext>>.Create();

    public List<CapturedAttachment> Attachments { get; } = [];

    public List<CapturedScreenDiff> ScreenDiffs { get; } = [];

    public List<GlobalError> GlobalErrors { get; } = [];

    public AttachmentRecorder()
    {
        this.Sync.Handler = this.Capture;
        this.Async.Handler = this.Capture;
    }

    public TestApiEndpoint Endpoint => new(this.Sync.Instance, this.Async.Instance);

    object? Capture(MethodInfo method, object?[] arguments)
    {
        var token = method.Name.EndsWith("Async", StringComparison.Ordinal)
            ? (CancellationToken)arguments[^1]!
            : default;

        if (method.Name is "AddAttachment" or "AddAttachmentAsync" or "AddGlobalAttachment" or "AddGlobalAttachmentAsync")
        {
            this.Attachments.Add(new(
                method.Name,
                (string)arguments[0]!,
                Read((Stream)arguments[1]!),
                null,
                (string?)arguments[2],
                (string)arguments[3]!,
                token
            ));
        }
        else if (method.Name is "AddFileAttachment" or "AddFileAttachmentAsync" or "AddGlobalFileAttachment" or "AddGlobalFileAttachmentAsync")
        {
            this.Attachments.Add(new(
                method.Name,
                (string)arguments[0]!,
                null,
                (string)arguments[1]!,
                (string?)arguments[2],
                (string)arguments[3]!,
                token
            ));
        }
        else if (method.Name is "AddScreenDiff" or "AddScreenDiffAsync")
        {
            this.ScreenDiffs.Add(new(
                method.Name,
                Read((Stream)arguments[0]!),
                Read((Stream)arguments[1]!),
                Read((Stream)arguments[2]!),
                null,
                null,
                null,
                token
            ));
        }
        else if (method.Name is "AddFileScreenDiff" or "AddFileScreenDiffAsync")
        {
            this.ScreenDiffs.Add(new(
                method.Name,
                null,
                null,
                null,
                (string)arguments[0]!,
                (string)arguments[1]!,
                (string)arguments[2]!,
                token
            ));
        }
        else if (method.Name is "AddGlobalError" or "AddGlobalErrorAsync")
        {
            this.GlobalErrors.Add((GlobalError)arguments[0]!);
        }

        return method.ReturnType == typeof(Task) ? Task.CompletedTask : null;
    }

    static byte[] Read(Stream stream)
    {
        using var copy = new MemoryStream();
        stream.CopyTo(copy);
        return copy.ToArray();
    }
}
