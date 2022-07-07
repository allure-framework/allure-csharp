using Allure.Net.Commons.Helpers;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Allure.Net.Commons.Tests
{
    [TestFixture]
    internal class LinkHelperTests
    {
        [Test]
        public void UpdateLinksTest()
        {
            var links = new List<Link>()
            {
                new Link() { url = "123456" },
                new Link() { type = "custom", url = "Custom Url"},
                new Link() { type = "", url = "Empty Type"},
                Link.Tms(""),
                Link.Issue(""),
                Link.Issue("issue1","Issue URL")
            };

            var patterns = new HashSet<string>()
            {
                "http://Issue.com/{ISSUE}",
                "http://TMS.com/{tMs}",
                "",
                "{",
                "}",
                "{}",
                "{ }"
            };

            LinkHelper.UpdateLinks(links, patterns);
            var urls = links.Select(x => x.url);
            Assert.Multiple(() =>
            {
                Assert.That(urls, Has.Exactly(1).Items.EqualTo("123456"));
                Assert.That(urls, Has.Exactly(1).Items.EqualTo("Custom Url"));
                Assert.That(urls, Has.Exactly(1).Items.EqualTo("Empty Type"));
                Assert.That(urls, Has.Exactly(1).Items.EqualTo("http://TMS.com/"));
                Assert.That(urls, Has.Exactly(1).Items.EqualTo("http://Issue.com/"));
                Assert.That(urls, Has.Exactly(1).Items.EqualTo("http://Issue.com/Issue%20URL"));
            });
        }
    }
}
