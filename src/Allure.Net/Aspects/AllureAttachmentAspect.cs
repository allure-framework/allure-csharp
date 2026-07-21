using System;
using System.IO;
using System.Reflection;
using System.Text;
using Allure.Abstractions;
using Allure.Internal;
using Allure.Runtime;
using AspectInjector.Broker;

namespace Allure.Aspects;

/// <summary>
/// An aspect that creates attachments from a functions' return values.
/// </summary>
[Aspect(Scope.Global)]
public class AllureAttachmentAspect
{
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

        if (ShouldSkip(isGlobal))
        {
            return;
        }

        var endpoint = AllureFrontend.Client.ResolveCurrentScope();
        if (endpoint is null)
        {
            return;
        }

        var attachmentName = ResolveAttachmentName(endpoint, attr, name, metadata, arguments);
        var contentType = ResolveContentType(attr, returnType);
        var extension = ResolveExtension(attr, contentType);
        byte[] content = ResolveContent(attr, returnValue);

        using var contentStream = new MemoryStream(content);

        if (isGlobal)
        {
            endpoint.Operations.Sync.AddGlobalAttachment(
                name: attachmentName,
                content: contentStream,
                mediaType: contentType,
                fileExtension: extension
            );
        }
        else
        {
            endpoint.Operations.Sync.AddAttachment(
                name: attachmentName,
                content: contentStream,
                mediaType: contentType,
                fileExtension: extension
            );
        }
    }

    static bool ShouldSkip(bool isGlobal) =>
        isGlobal
            ? !AllureFrontend.IsAvailableInGlobalScope
            : !AllureFrontend.IsAvailableInCurrentScope;

    static string ResolveAttachmentName(
        IAllureApiEndpoint endpoint,
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

    static byte[] ResolveContent(AllureAttachmentAttribute? attr, object? value) => value switch
    {
        null => [],
        byte[] byteArray => byteArray,
        string text => Encoding.GetEncoding(attr?.Encoding ?? "UTF-8").GetBytes(text),
        Stream stream => ConsumeStream(stream),
        _ => throw new InvalidOperationException(
            $"Can't create an Allure attachment from {value.GetType().FullName}. "
                + "A string, byte[], or stream was expected."
        )
    };

    static byte[] ConsumeStream(Stream stream)
    {
        if (!stream.CanRead)
        {
            throw new InvalidOperationException(
                $"Can't create an Allure attachment from {stream.GetType().FullName}: "
                        + "this stream does not support the read operation."
            );
        }

        if (!stream.CanSeek)
        {
            throw new InvalidOperationException(
                $"Can't create an Allure attachment from {stream.GetType().FullName}: "
                        + "this stream does not support the seek operation."
            );
        }

        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        stream.Position = 0;
        return memoryStream.ToArray();
    }
}