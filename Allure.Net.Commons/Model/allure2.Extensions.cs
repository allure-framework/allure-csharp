using System;
using System.Collections.Generic;
using System.ComponentModel;
using Allure.Net.Commons.Functions;

#pragma warning disable IDE1006

namespace Allure.Net.Commons
{
    /// <summary>
    /// The test severity levels.
    /// </summary>
    public enum SeverityLevel
    {
        normal,
        blocker,
        critical,
        minor,
        trivial
    }

    /// <summary>
    /// The names of the well-known labels.
    /// </summary>
    public static class LabelName
    {
        #region Integration-specific labels

        public const string TEST_TYPE = "testType";
        public const string LANGUAGE = "language";
        public const string FRAMEWORK = "framework";
        public const string THREAD = "thread";
        public const string HOST = "host";
        public const string PACKAGE = "package";
        public const string TEST_CLASS = "testClass";
        public const string TEST_METHOD = "testMethod";

        #endregion

        #region Test-specific labels

        public const string OWNER = "owner";
        public const string SEVERITY = "severity";
        public const string ALLURE_ID = AllureConstants.NEW_ALLURE_ID_LABEL_NAME;
        public const string TAG = "tag";
        public const string PARENT_SUITE = "parentSuite";
        public const string SUITE = "suite";
        public const string SUB_SUITE = "subSuite";
        public const string EPIC = "epic";
        public const string FEATURE = "feature";
        public const string STORY = "story";

        #endregion
    }

    /// <summary>
    /// The names of the well-known link types.
    /// </summary>
    public static class LinkType
    {
        public const string LINK = "link";
        public const string ISSUE = "issue";
        public const string TMS_ITEM = "tms";
    }

    /// <summary>
    /// The parameter display modes.
    /// </summary>
    public enum ParameterMode
    {
        /// <summary>
        /// The parameter's value is revealed.
        /// </summary>
        Default,

        /// <summary>
        /// The parameter's value is masked with placeholder characters.
        /// </summary>
        Masked,

        /// <summary>
        /// The parameter isn't shown in the report at all (but is still used
        /// to correlate the history of the test's runs).
        /// </summary>
        Hidden
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
        [Obsolete("Please, use AllureApi.SetTestParameter instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
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
            return new Label {name = LabelName.TEST_TYPE, value = value};
        }

        public static Label ParentSuite(string value)
        {
            return new Label {name = LabelName.PARENT_SUITE, value = value};
        }

        public static Label Suite(string value)
        {
            return new Label {name = LabelName.SUITE, value = value};
        }

        public static Label SubSuite(string value)
        {
            return new Label {name = LabelName.SUB_SUITE, value = value};
        }

        public static Label Owner(string value)
        {
            return new Label {name = LabelName.OWNER, value = value};
        }

        public static Label Severity(SeverityLevel value)
        {
            return new Label {name = LabelName.SEVERITY, value = value.ToString()};
        }

        public static Label Tag(string value)
        {
            return new Label {name = LabelName.TAG, value = value};
        }

        public static Label Epic(string value)
        {
            return new Label {name = LabelName.EPIC, value = value};
        }

        public static Label Feature(string value)
        {
            return new Label {name = LabelName.FEATURE, value = value};
        }

        public static Label Framework(string value)
        {
            return new Label { name = LabelName.FRAMEWORK, value = value };
        }

        public static Label Language()
        {
            return new Label { name = LabelName.LANGUAGE, value = "C#" };
        }

        public static Label Story(string value)
        {
            return new Label {name = LabelName.STORY, value = value};
        }

        public static Label Package(string value)
        {
            return new Label {name = LabelName.PACKAGE, value = value};
        }

        public static Label TestClass(string value)
        {
            return new Label {name = LabelName.TEST_CLASS, value = value};
        }

        public static Label TestMethod(string value)
        {
            return new Label {name = LabelName.TEST_METHOD, value = value};
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
                name = LabelName.HOST,
                value = Environment.MachineName ?? "Unknown host"
            };
        }

        public static Label Host(string value)
        {
            return new Label
            {
                name = LabelName.HOST,
                value = value
            };
        }

        public static Label AllureId(int value) =>
            new()
            {
                name = LabelName.ALLURE_ID,
                value = value.ToString()
            };
    }

    public partial class Link
    {
        public static Link Issue(string name, string url)
        {
            return new Link {name = name, type = LinkType.ISSUE, url = url};
        }

        public static Link Issue(string url)
        {
            return Issue(null, url);
        }

        public static Link Tms(string name, string url)
        {
            return new Link {name = name, type = LinkType.TMS_ITEM, url = url};
        }

        public static Link Tms(string url)
        {
            return Tms(null, url);
        }
    }

    public partial class Parameter
    {
        public ParameterMode? mode { get; set; }
    }
}