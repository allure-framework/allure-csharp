namespace Allure.Model;

using System;
using Allure.Internal;
using SeverityLevel = Severity;
using SystemThread = System.Threading.Thread;

public sealed class Label
{
    required public string Name { get; set; }

    required public string Value { get; set; }

    public static Label Create(string name, string value) =>
        new() { Name = name, Value = value };

    public static Label AllureId(int allureId) =>
        Create(LabelName.AllureId, allureId.ToString());

    public static Label Suite(string suite) =>
        Create(LabelName.Suite, suite);

    public static Label ParentSuite(string parentSuite) =>
        Create(LabelName.ParentSuite, parentSuite);

    public static Label SubSuite(string subSuite) =>
        Create(LabelName.SubSuite, subSuite);

    public static Label Epic(string epic) =>
        Create(LabelName.Epic, epic);

    public static Label Feature(string feature) =>
        Create(LabelName.Feature, feature);

    public static Label Story(string story) =>
        Create(LabelName.Story, story);

    public static Label Severity(SeverityLevel severity) =>
        Create(LabelName.Severity, severity.ToLabelValue());

    public static Label Tag(string tag) =>
        Create(LabelName.Tag, tag);

    public static Label Owner(string owner) =>
        Create(LabelName.Owner, owner);

    public static Label Lead(string lead) =>
        Create(LabelName.Lead, lead);

    public static Label Host(string hostName) =>
        Create(LabelName.Host, hostName);

    public static Label Host() =>
        Create(LabelName.Host, Environment.MachineName ?? "Unknown host");

    public static Label Thread(string threadName) =>
        Create(LabelName.Thread, threadName);

    public static Label Thread() =>
        Create(
            LabelName.Thread,
            SystemThread.CurrentThread.Name
                ?? SystemThread.CurrentThread.ManagedThreadId.ToString()
        );

    public static Label TestMethod(string testMethod) =>
        Create(LabelName.TestMethod, testMethod);

    public static Label TestClass(string testClass) =>
        Create(LabelName.TestClass, testClass);

    public static Label Package(string package) =>
        Create(LabelName.Package, package);

    public static Label Framework(string framework) =>
        Create(LabelName.Framework, framework);

    public static Label Language(string language) =>
        Create(LabelName.Language, language);

    public static Label Language() =>
        Create(LabelName.Language, "C#");

    public static Label Layer(string layer) =>
        Create(LabelName.Layer, layer);
}
