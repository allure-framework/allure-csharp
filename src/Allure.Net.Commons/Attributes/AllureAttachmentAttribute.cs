using System;
using Allure.Net.Commons.Sdk.Aspects;
using AspectInjector.Broker;

#nullable enable

namespace Allure.Net.Commons.Attributes;

/// <summary>
/// When applied to a function returning <c>byte[]</c> or <c>string</c>, creates an attachment
/// from the function's return value each time it's called.
/// </summary>
[Injection(typeof(AllureAttachmentAspect))]
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class AllureAttachmentAttribute : Attribute
{
    /// <summary>
    /// A name of the attachment to display in the report. The <c>{paramName}</c> placeholders can
    /// be used to interpolate the function's arguments into the name.
    /// </summary>
    public string? Name { get; }

    /// <summary>
    /// A content type of the attachment. It affects how the attachment is rendered in the report.
    /// By default, <c>application/octet-stream</c> is used for <c>byte[]</c> and <c>text/plain</c>
    /// for <c>string</c>.
    /// </summary>
    /// <remarks>
    /// Examples: <c>application/json</c>, <c>image/png</c>.
    /// </remarks>
    public string? ContentType { get; init; }

    /// <summary>
    /// A file extension to use when the attachment is downloaded.
    /// </summary>
    /// <remarks>
    /// By default, the extension is derived from the content type if possible.
    /// </remarks>
    public string? Extension { get; init; }

    /// <summary>
    /// Which encoding to use when converting a string into a byte array. By default,
    /// UTF-8 is used.
    /// </summary>
    /// <remarks>
    /// If the function returns <c>byte[]</c>, this property has no effect.
    /// </remarks>
    public string? Encoding { get; init; }

    /// <summary>
    /// Sets up the target function to create an attachment with the same name as the function.
    /// </summary>
    public AllureAttachmentAttribute() { }

    /// <summary>
    /// Sets up the target function to create an explicitly named attachment.
    /// </summary>
    /// <param name="name">
    /// The attachment's name. Use the <c>{paramName}</c> placeholders to interpolate the
    /// arguments.
    /// </param>
    public AllureAttachmentAttribute(string name)
    {
        this.Name = name;
    }
}