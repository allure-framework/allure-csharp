using System;
using System.IO;
using System.Reflection;
using System.Text;
using Allure.Net.Commons.Attributes;
using AspectInjector.Broker;
using HeyRed.Mime;

#nullable enable

namespace Allure.Net.Commons.Sdk.Aspects;

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

        if (!isGlobal && !AllureApi.HasTestOrFixture)
        {
            return;
        }

        var attachmentName = ResolveAttachmentName(attr, name, metadata, arguments);
        var contentType = ResolveContentType(attr, returnType);
        var extension = ResolveExtension(attr, contentType);
        byte[] content = ResolveContent(attr, returnValue);

        if (isGlobal)
        {
            AllureApi.AddGlobalAttachmentInternal(attachmentName, contentType, content, extension);
        }
        else
        {
            AllureApi.AddAttachmentInternal(attachmentName, contentType, content, extension);
        }
    }

    static string ResolveAttachmentName(
        AllureAttachmentAttribute? attr,
        string name,
        MethodBase methodInfo,
        object[] arguments
    )
        => string.IsNullOrEmpty(attr?.Name)
            ? name
            : Steps.AllureStepParameterHelper.GetStepName(
                attr!.Name,
                methodInfo,
                arguments,
                AllureApi.CurrentLifecycle.TypeFormatters
            );

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
                    : MimeTypesMap.GetExtension(contentType))
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