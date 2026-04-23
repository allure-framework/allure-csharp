using System;
using System.IO;
using System.Reflection;
using Allure.Net.Commons.Attributes;
using AspectInjector.Broker;
using HeyRed.Mime;

#nullable enable

namespace Allure.Net.Commons.Sdk.Aspects;

/// <summary>
/// An aspect that creates attachments from files pointed by a functions' return values.
/// </summary>
[Aspect(Scope.Global)]
public class AllureAttachmentFileAspect
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
        var attr = metadata.GetCustomAttribute<AllureAttachmentFileAttribute>();
        var isGlobal = attr?.Global == true;

        if (!isGlobal && !AllureApi.HasTestOrFixture)
        {
            return;
        }

        var attachmentFile = ResolveFile(returnValue);
        if (attachmentFile is null)
        {
            return;
        }

        var attachmentName = ResolveAttachmentName(attachmentFile, attr, metadata, arguments);
        var contentType = attr?.ContentType;
        var extension = ResolveExtension(attachmentFile, contentType);
        var content = File.ReadAllBytes(attachmentFile.FullName);

        if (isGlobal)
        {
            AllureApi.AddGlobalAttachmentInternal(attachmentName, contentType, content, extension);
        }
        else
        {
            AllureApi.AddAttachmentInternal(attachmentName, contentType, content, extension);
        }
    }

    static FileInfo? ResolveFile(object? value) => value switch
    {
        null => null,
        string path => new FileInfo(path),
        FileInfo fInfo => fInfo,
        _ => throw new InvalidOperationException(
            $"Can't create an Allure file attachment from {value.GetType().FullName}. "
                + "A string or System.IO.FileInfo was expected."),
    };

    static string ResolveAttachmentName(
        FileInfo attachmentFile,
        AllureAttachmentFileAttribute? attr,
        MethodBase methodInfo,
        object[] arguments
    )
        => string.IsNullOrEmpty(attr?.Name)
            ? attachmentFile.Name
            : Steps.AllureStepParameterHelper.GetStepName(
                attr!.Name,
                methodInfo,
                arguments,
                AllureApi.CurrentLifecycle.TypeFormatters
            );

    static string ResolveExtension(FileInfo attachmentFile, string? contentType)
        => string.IsNullOrEmpty(attachmentFile.Extension)
            ? (contentType is null
                ? ""
                : $".{MimeTypesMap.GetExtension(contentType)}")
            : attachmentFile.Extension;
}