using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Allure.Commons
{
    public partial class Label
    {
        public static Label AddOwner(string value) => new Label() { name = "owner", value = value };
        public static Label AddSeverity(string value) => new Label() { name = "severity", value = value };
        public static Label AddIssue(string value) => new Label() { name = "issue", value = value };
        public static Label AddTag(string value) => new Label() { name = "tag", value = value };

        public static Label AddEpic(string value) => new Label() { name = "epic", value = value };
        public static Label AddFeature(string value) => new Label() { name = "feature", value = value };
        public static Label AddStory(string value) => new Label() { name = "story", value = value };

        public static Label AddThread() => new Label()
        {
            name = "thread",
            value = Thread.CurrentThread.Name ?? Thread.CurrentThread.ManagedThreadId.ToString()
        };
        public static Label AddHost() => new Label()
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
