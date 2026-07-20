using System;
using System.IO;
using System.Reflection;
using Allure.Internal;
using Allure.Runtime;
using AspectInjector.Broker;

namespace Allure.Aspects;

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

        if (!isGlobal && !AllureFrontend.IsAvailable)
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
        var path = attachmentFile.FullName;

        if (isGlobal)
        {
            AllureFrontend.Client.Operations.Sync.AddGlobalFileAttachment(
                name: attachmentName,
                path: path,
                mediaType: contentType,
                fileExtension: extension
            );
        }
        else
        {
            AllureFrontend.Client.Operations.Sync.AddFileAttachment(
                name: attachmentName,
                path: path,
                mediaType: contentType,
                fileExtension: extension
            );
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
    ) =>
        attr is { Name: { Length: >0 } name }
            ? methodInfo.ConstructAllureName(name, arguments)
            : attachmentFile.Name;

    static string ResolveExtension(FileInfo attachmentFile, string? contentType)
        => string.IsNullOrEmpty(attachmentFile.Extension)
            ? (contentType is null
                ? ""
                : MediaTypeExtensions.Get(contentType))
            : attachmentFile.Extension;
}
