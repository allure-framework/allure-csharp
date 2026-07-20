using System;
using Allure.Aspects;
using AspectInjector.Broker;

namespace Allure;

/// <summary>
/// When applied to a method returning <c>byte[]</c>, <c>string</c>,
/// <see cref="System.IO.Stream"/>, or a corresponding async type
/// (e.g., <c>Task&lt;byte[]></c>), creates an attachment
/// from the return value each time the method is called.
/// </summary>
[Injection(typeof(AllureAttachmentAspect))]
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class AllureAttachmentAttribute : Attribute
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
    /// If the method returns a type other than <c>string</c> (or its async counterpart),
    /// this property has no effect.
    /// </remarks>
    public string? Encoding { get; init; }

    /// <summary>
    /// If set to <c>true</c>, creates a global attachment not tied to the current
    /// test, fixture, or step.
    /// </summary>
    public bool Global { get; init; }

    /// <summary>
    /// Creates attachments named after the method.
    /// </summary>
    public AllureAttachmentAttribute() { }

    /// <summary>
    /// Creates attachments with explicit names. Argument interpolation is supported.
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