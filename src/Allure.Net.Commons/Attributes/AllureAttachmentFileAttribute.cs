using System;
using Allure.Net.Commons.Sdk.Aspects;
using AspectInjector.Broker;

#nullable enable

namespace Allure.Net.Commons.Attributes;

/// <summary>
/// When applied to a method returning <c>string</c>,
/// <see cref="System.IO.FileInfo"/>, or a corresponding async type
/// (e.g., <c>Task&lt;string></c>), interprets the return value of the method
/// as a path to a file and attaches it each time the method is called.
/// </summary>
[Injection(typeof(AllureAttachmentFileAspect))]
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class AllureAttachmentFileAttribute : Attribute
{
    /// <summary>
    /// A name of the attachment to display in the report. The <c>{paramName}</c> placeholders can
    /// be used to interpolate the method's arguments into the name.
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
    /// Creates attachments with the same names as the original files.
    /// </summary>
    public AllureAttachmentFileAttribute() { }

    /// <summary>
    /// Creates attachments with explicit names. Argument interpolation is supported.
    /// </summary>
    /// <param name="name">
    /// The attachment's name. Use the <c>{paramName}</c> placeholders to interpolate the
    /// arguments.
    /// </param>
    public AllureAttachmentFileAttribute(string name)
    {
        this.Name = name;
    }
}