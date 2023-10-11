using System;
using System.Collections;
using System.Collections.Generic;
using Allure.Net.Commons.Functions;

namespace Allure.Net.Commons
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
        /// <summary>
        /// Creates a new parameter and puts it into the test parameter list.
        /// Uses
        /// <see cref="FormatFunctions.Format(object?, IReadOnlyDictionary{Type, ITypeFormatter})"/>
        /// to convert parameter's value to a string.
        /// </summary>
        /// <param name="name">The name of a new parameter.</param>
        /// <param name="value">The original value of a new parameter.</param>
        /// <param name="formatters">
        /// The list of formatters to use to serialize the parameter's value to
        /// a string. Usually, this comes from <see cref="AllureLifecycle.TypeFormatters"/>
        /// </param>
        public void AddParameter(
            string name,
            object value,
            IReadOnlyDictionary<Type, ITypeFormatter> formatters
        ) =>
            this.parameters.Add(
                new()
                {
                    name = name,
                    value = FormatFunctions.Format(value, formatters)
                }
            );

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
        public static Label TestType(string value)
        {
            return new Label {name = "testType", value = value};
        }

        public static Label ParentSuite(string value)
        {
            return new Label {name = "parentSuite", value = value};
        }

        public static Label Suite(string value)
        {
            return new Label {name = "suite", value = value};
        }

        public static Label SubSuite(string value)
        {
            return new Label {name = "subSuite", value = value};
        }

        public static Label Owner(string value)
        {
            return new Label {name = "owner", value = value};
        }

        public static Label Severity(SeverityLevel value)
        {
            return new Label {name = "severity", value = value.ToString()};
        }

        public static Label Tag(string value)
        {
            return new Label {name = "tag", value = value};
        }

        public static Label Epic(string value)
        {
            return new Label {name = "epic", value = value};
        }

        public static Label Feature(string value)
        {
            return new Label {name = "feature", value = value};
        }

        public static Label Framework(string value)
        {
            return new Label { name = "framework", value = value };
        }

        public static Label Story(string value)
        {
            return new Label {name = "story", value = value};
        }

        public static Label Package(string value)
        {
            return new Label {name = "package", value = value};
        }

        public static Label TestClass(string value)
        {
            return new Label {name = "testClass", value = value};
        }

        public static Label TestMethod(string value)
        {
            return new Label {name = "testMethod", value = value};
        }

        public static Label Thread()
        {
            return new Label
            {
                name = "thread",
                value = System.Threading.Thread.CurrentThread.Name ??
                        System.Threading.Thread.CurrentThread.ManagedThreadId.ToString()
            };
        }

        public static Label Host()
        {
            return new Label
            {
                name = "host",
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