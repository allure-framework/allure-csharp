using System;

namespace Allure.Commons
{
    public enum SeverityLevel { normal, blocker, critical, minor, trivial }

    public partial class TestResultContainer
    {
        public override string ToString() => name ?? uuid;
    }

    public partial class TestResult
    {
        public override string ToString() => name ?? uuid;
    }

    public partial class FixtureResult
    {
        public override string ToString() => name;
    }

    public partial class Label
    {
        public static Label TestType(string value) => new Label() { name = "testType", value = value };

        public static Label ParentSuite(string value) => new Label() { name = "parentSuite", value = value };
        public static Label Suite(string value) => new Label() { name = "suite", value = value };
        public static Label SubSuite(string value) => new Label() { name = "subSuite", value = value };

        public static Label Owner(string value) => new Label() { name = "owner", value = value };
        public static Label Severity(SeverityLevel value) => new Label() { name = "severity", value = value.ToString() };
        public static Label Tag(string value) => new Label() { name = "tag", value = value };

        public static Label Epic(string value) => new Label() { name = "epic", value = value };
        public static Label Feature(string value) => new Label() { name = "feature", value = value };
        public static Label Story(string value) => new Label() { name = "story", value = value };

        public static Label Package(string value) => new Label() { name = "package", value = value };
        public static Label TestClass(string value) => new Label() { name = "testClass", value = value };
        public static Label TestMethod(string value) => new Label() { name = "testMethod", value = value };

        public static Label Thread() => new Label()
        {
            name = "thread",
            value = System.Threading.Thread.CurrentThread.Name ?? System.Threading.Thread.CurrentThread.ManagedThreadId.ToString()
        };
        public static Label Host() => new Label()
        {
            name = "host",
            value = Environment.MachineName ?? "Unknown host"
        };

    }

    public partial class Link
    {
        public static Link Issue(string name, string url) => new Link() { name = name, type = "issue", url = url };
        public static Link Issue(string name) => Issue(name, null);

        public static Link Tms(string name, string url) => new Link() { name = name, type = "tms", url = url };
        public static Link Tms(string name) => Tms(name, null);

    }
}
