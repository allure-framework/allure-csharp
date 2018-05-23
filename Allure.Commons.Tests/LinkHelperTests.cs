using Allure.Commons.Helpers;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Allure.Commons.Tests
{
    [TestFixture]
    class LinkHelperTests
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
                Assert.That(urls, Has.Exactly(1).Items.EqualTo("http://tms.com/"));
                Assert.That(urls, Has.Exactly(1).Items.EqualTo("http://issue.com/"));
                Assert.That(urls, Has.Exactly(1).Items.EqualTo("http://issue.com/Issue URL"));
            });
        }
    }
}
