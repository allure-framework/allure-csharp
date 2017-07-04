using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Allure.Commons
{
    public partial class Label
    {
        public static Label TestType(string value) => new Label() { name = "testType", value = value };

        public static Label ParentSuite(string value) => new Label() { name = "parentSuite", value = value };
        public static Label Suite(string value) => new Label() { name = "suite", value = value };
        public static Label SubSuite(string value) => new Label() { name = "subSuite", value = value };

        public static Label Owner(string value) => new Label() { name = "owner", value = value };
        public static Label Severity(string value) => new Label() { name = "severity", value = value };
        public static Label Issue(string value) => new Label() { name = "issue", value = value };
        public static Label Tag(string value) => new Label() { name = "tag", value = value };

        public static Label Epic(string value) => new Label() { name = "epic", value = value };
        public static Label Feature(string value) => new Label() { name = "feature", value = value };
        public static Label Story(string value) => new Label() { name = "story", value = value };

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
        public static Link AddIssue(string name, string url) => new Link() { name = name, type = "issue", url = url };
        public static Link AddTms(string name, string url) => new Link() { name = name, type = "tms", url = url };

    }
}
