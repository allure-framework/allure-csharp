using System;
using System.Reflection;
using System.Text;
using Allure.Net.Commons.Attributes;
using AspectInjector.Broker;

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
        if (!AllureApi.HasTestOrFixture)
        {
            return;
        }

        var attr = metadata.GetCustomAttribute<AllureAttachmentAttribute>();
        var attachmentName
            = string.IsNullOrEmpty(attr?.Name)
                ? name
                : Steps.AllureStepParameterHelper.GetStepName(
                    attr!.Name,
                    metadata,
                    arguments,
                    AllureApi.CurrentLifecycle.TypeFormatters
                );
        var contentType
            = attr?.ContentType
                ?? (returnType == typeof(string)
                    ? "text/plain"
                    : null);
        var extension
            = attr?.Extension
                ?? (contentType is null ? "" : HeyRed.Mime.MimeTypesMap.GetExtension(contentType))
                ?? "";

        extension
            = extension.Length == 0 || extension.StartsWith(".")
                ? extension
                : $".{extension}";

        byte[] content = returnValue switch
        {
            null => [],
            byte[] byteArray => byteArray,
            string text => Encoding.GetEncoding(attr?.Encoding ?? "UTF-8").GetBytes(text),
            _ => throw new InvalidOperationException(
                $"Can't create an attachment from {returnValue.GetType().Name}. "
                    + "String or byte[] expected."
            )
        };

        AllureApi.AddAttachmentInternal(attachmentName, contentType, content, extension);
    }
}