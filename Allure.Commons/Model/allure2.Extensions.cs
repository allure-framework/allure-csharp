using System;
using System.Reflection.Emit;

namespace Allure.Commons
{
    public enum SeverityLevel
    {
        normal,
        blocker,
        critical,
        minor,
        trivial
    }

    public partial class TestResultContainer
    {
        public override string ToString()
        {
            return name ?? uuid;
        }
    }

    public partial class TestResult
    {
        public override string ToString()
        {
            return name ?? uuid;
        }
    }

    public partial class FixtureResult
    {
        public override string ToString()
        {
            return name;
        }
    }

    public partial class Label
    {
        public  static class LabelNames
        {
            public const string TestType = "testType";
            public const string ParentSuite = "parentSuite";
            public const string Suite = "suite";
            public const string SubSuite = "subSuite";
            public const string Owner = "owner";
            public const string Severity = "severity";
            public const string Tag = "tag";
            public const string Epic = "epic";
            public const string Feature = "feature";
            public const string Story = "story";
            public const string Package = "package";
            public const string TestClass = "testClass";
            public const string TestMethod = "testMethod";
            public const string Thread = "thread";
            public const string Host = "host";
        }

        public static Label TestType(string value)
        {
            return new Label {name = LabelNames.TestType , value = value};
        }

        public static Label ParentSuite(string value)
        {
            return new Label {name = LabelNames.ParentSuite, value = value};
        }

        public static Label Suite(string value)
        {
            return new Label {name = LabelNames.Suite, value = value};
        }

        public static Label SubSuite(string value)
        {
            return new Label {name = LabelNames.SubSuite, value = value};
        }

        public static Label Owner(string value)
        {
            return new Label {name = LabelNames.Owner, value = value};
        }

        public static Label Severity(SeverityLevel value)
        {
            return new Label {name = LabelNames.Severity, value = value.ToString()};
        }

        public static Label Tag(string value)
        {
            return new Label {name = LabelNames.Tag, value = value};
        }

        public static Label Epic(string value)
        {
            return new Label {name = LabelNames.Epic, value = value};
        }

        public static Label Feature(string value)
        {
            return new Label {name = LabelNames.Feature, value = value};
        }

        public static Label Story(string value)
        {
            return new Label {name = LabelNames.Story, value = value};
        }

        public static Label Package(string value)
        {
            return new Label {name = LabelNames.Package, value = value};
        }

        public static Label TestClass(string value)
        {
            return new Label {name = LabelNames.TestClass, value = value};
        }

        public static Label TestMethod(string value)
        {
            return new Label {name = LabelNames.TestMethod, value = value};
        }

        public static Label Thread()
        {
            return new Label
            {
                name = LabelNames.Thread,
                value = System.Threading.Thread.CurrentThread.Name ??
                        System.Threading.Thread.CurrentThread.ManagedThreadId.ToString()
            };
        }

        public static Label Host()
        {
            return new Label
            {
                name = LabelNames.Host,
                value = Environment.MachineName ?? "Unknown host"
            };
        }

        public static Label Host(string value)
        {
            return new Label
            {
                name = "host",
                value = value
            };
        }
    }

    public partial class Link
    {
        public static Link Issue(string name, string url)
        {
            return new Link {name = name, type = "issue", url = url};
        }

        public static Link Issue(string name)
        {
            return Issue(name, null);
        }

        public static Link Tms(string name, string url)
        {
            return new Link {name = name, type = "tms", url = url};
        }

        public static Link Tms(string name)
        {
            return Tms(name, null);
        }
    }
}