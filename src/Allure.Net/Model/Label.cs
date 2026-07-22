using System;
using System.Globalization;
using Allure.Internal;

namespace Allure.Model;

using SeverityLevel = Severity;
using SystemThread = System.Threading.Thread;

/// <summary>
/// Describes a label associated with a test result.
/// </summary>
public sealed class Label
{
    /// <summary>
    /// Gets or sets the label name.
    /// </summary>
    required public string Name { get; set; }

    /// <summary>
    /// Gets or sets the label value.
    /// </summary>
    required public string Value { get; set; }

    /// <summary>
    /// Creates a label with the specified name and value.
    /// </summary>
    public static Label Create(string name, string value) =>
        new() { Name = name, Value = value };

    /// <summary>
    /// Creates an Allure ID label.
    /// </summary>
    public static Label AllureId(int allureId) =>
        Create(LabelName.AllureId, allureId.ToString(CultureInfo.InvariantCulture));

    /// <summary>
    /// Creates a suite label.
    /// </summary>
    public static Label Suite(string suite) =>
        Create(LabelName.Suite, suite);

    /// <summary>
    /// Creates a parent suite label.
    /// </summary>
    public static Label ParentSuite(string parentSuite) =>
        Create(LabelName.ParentSuite, parentSuite);

    /// <summary>
    /// Creates a sub-suite label.
    /// </summary>
    public static Label SubSuite(string subSuite) =>
        Create(LabelName.SubSuite, subSuite);

    /// <summary>
    /// Creates an epic label.
    /// </summary>
    public static Label Epic(string epic) =>
        Create(LabelName.Epic, epic);

    /// <summary>
    /// Creates a feature label.
    /// </summary>
    public static Label Feature(string feature) =>
        Create(LabelName.Feature, feature);

    /// <summary>
    /// Creates a story label.
    /// </summary>
    public static Label Story(string story) =>
        Create(LabelName.Story, story);

    /// <summary>
    /// Creates a severity label.
    /// </summary>
    public static Label Severity(SeverityLevel severity) =>
        Create(LabelName.Severity, severity.ToLabelValue());

    /// <summary>
    /// Creates a tag label.
    /// </summary>
    public static Label Tag(string tag) =>
        Create(LabelName.Tag, tag);

    /// <summary>
    /// Creates an owner label.
    /// </summary>
    public static Label Owner(string owner) =>
        Create(LabelName.Owner, owner);

    /// <summary>
    /// Creates a lead label.
    /// </summary>
    public static Label Lead(string lead) =>
        Create(LabelName.Lead, lead);

    /// <summary>
    /// Creates a host label with the specified host name.
    /// </summary>
    public static Label Host(string hostName) =>
        Create(LabelName.Host, hostName);

    /// <summary>
    /// Creates a host label for the current machine.
    /// </summary>
    public static Label Host() =>
        Create(LabelName.Host, Environment.MachineName ?? "Unknown host");

    /// <summary>
    /// Creates a thread label with the specified thread name.
    /// </summary>
    public static Label Thread(string threadName) =>
        Create(LabelName.Thread, threadName);

    /// <summary>
    /// Creates a label for the current managed thread.
    /// </summary>
    public static Label Thread() =>
        Create(
            LabelName.Thread,
            SystemThread.CurrentThread.Name
                ?? SystemThread.CurrentThread.ManagedThreadId.ToString()
        );

    /// <summary>
    /// Creates a test method label.
    /// </summary>
    public static Label TestMethod(string testMethod) =>
        Create(LabelName.TestMethod, testMethod);

    /// <summary>
    /// Creates a test class label.
    /// </summary>
    public static Label TestClass(string testClass) =>
        Create(LabelName.TestClass, testClass);

    /// <summary>
    /// Creates a package label.
    /// </summary>
    public static Label Package(string package) =>
        Create(LabelName.Package, package);

    /// <summary>
    /// Creates a test framework label.
    /// </summary>
    public static Label Framework(string framework) =>
        Create(LabelName.Framework, framework);

    /// <summary>
    /// Creates a programming language label.
    /// </summary>
    public static Label Language(string language) =>
        Create(LabelName.Language, language);

    /// <summary>
    /// Creates a C# programming language label.
    /// </summary>
    public static Label Language() =>
        Create(LabelName.Language, "C#");

    /// <summary>
    /// Creates a test layer label.
    /// </summary>
    public static Label Layer(string layer) =>
        Create(LabelName.Layer, layer);
}
