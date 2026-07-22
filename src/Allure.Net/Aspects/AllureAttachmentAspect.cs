using System;
using System.IO;
using System.Reflection;
using System.Text;
using Allure.Abstractions;
using Allure.Internal;
using Allure.Runtime;
using AspectInjector.Broker;
using CommunityToolkit.HighPerformance;

namespace Allure.Aspects;

/// <summary>
/// Creates attachments from annotated method return values.
/// </summary>
[Aspect(Scope.Global)]
public class AllureAttachmentAspect
{
    /// <summary>
    /// Attaches the annotated method's return value.
    /// </summary>
    [Advice(Kind.After)]
    public void AttachReturnValue(
        [Argument(Source.Name)] string name,
        [Argument(Source.Metadata)] MethodBase metadata,
        [Argument(Source.Arguments)] object[] arguments,
        [Argument(Source.ReturnType)] Type returnType,
        [Argument(Source.ReturnValue)] object? returnValue
    )
    {
        var attr = metadata.GetCustomAttribute<AllureAttachmentAttribute>();
        var isGlobal = attr?.Global == true;

        var endpoint = isGlobal
            ? AllureRuntimeRouter.ResolveGlobalScope()
            : AllureRuntimeRouter.ResolveCurrentScope();

        if (endpoint is null)
        {
            return;
        }

        var attachmentName = ResolveAttachmentName(endpoint, attr, name, metadata, arguments);
        var contentType = ResolveContentType(attr, returnType);
        var extension = ResolveExtension(attr, contentType);
        using var contentGuard = ResolveContent(attr, returnValue);

        if (isGlobal)
        {
            endpoint.Operations.Sync.AddGlobalAttachment(
                name: attachmentName,
                content: contentGuard.Stream,
                mediaType: contentType,
                fileExtension: extension
            );
        }
        else
        {
            endpoint.Operations.Sync.AddAttachment(
                name: attachmentName,
                content: contentGuard.Stream,
                mediaType: contentType,
                fileExtension: extension
            );
        }
    }

    static string ResolveAttachmentName(
        IAllureRuntimeEndpoint endpoint,
        AllureAttachmentAttribute? attr,
        string methodName,
        MethodBase methodInfo,
        object[] arguments
    ) =>
        attr is { Name: { Length: >0 } name }
            ? methodInfo.ConstructAllureName(endpoint, name, arguments)
            : methodName;

    static string? ResolveContentType(AllureAttachmentAttribute? attr, Type valueType)
        => attr?.ContentType
            ?? (valueType == typeof(string)
                ? "text/plain"
                : null);

    static string ResolveExtension(AllureAttachmentAttribute? attr, string? contentType)
    {
        var extension
            = attr?.Extension
                ?? (contentType is null
                    ? ""
                    : MediaTypeExtensions.Get(contentType))
                ?? "";
        return extension.Length == 0 || extension.StartsWith(".")
            ? extension
            : $".{extension}";
    }

    static StreamGuard ResolveContent(AllureAttachmentAttribute? attr, object? value) => value switch
    {
        null => StreamGuard.Own(new MemoryStream()),
        byte[] byteArray => StreamGuard.Own(new MemoryStream(byteArray, writable: false)),
        string text => StreamGuard.Own(
            new MemoryStream(
                Encoding.GetEncoding(attr?.Encoding ?? "UTF-8").GetBytes(text),
                writable: false
            )
        ),
        ReadOnlyMemory<byte> memory => StreamGuard.Own(memory.AsStream()),
        Stream stream => StreamGuard.NotOwn(stream),
        _ => throw new InvalidOperationException(
            $"Can't create an Allure attachment from {value.GetType().FullName}. "
                + "A string, byte[], or stream was expected."
        ),
    };

    sealed class StreamGuard(Stream stream, bool own) : IDisposable
    {
        readonly long position = stream.CanSeek ? stream.Position : -1;

        public Stream Stream => stream;


        public void Dispose()
        {
            if (own)
            {
                stream.Dispose();
            }
            else if (this.Stream.CanSeek && this.position >= 0)
            {
                this.Stream.Position = this.position;
            }
        }

        public static StreamGuard Own(Stream stream) => new(stream, true);

        public static StreamGuard NotOwn(Stream stream) => new(stream, false);
    }
}
