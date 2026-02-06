using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Allure.Net.Commons.Helpers
{
    public class LinkHelper
    {
        public static void UpdateLinks(IEnumerable<Link> links, HashSet<string> patterns)
        {
            foreach (var linkTypeGroup in links
                .GroupBy(l => l.type ?? "link"))
            {
                var typePattern = $"{{{linkTypeGroup.Key}}}";
                var linkPattern = patterns.FirstOrDefault(x =>
                    x.IndexOf(typePattern, StringComparison.CurrentCultureIgnoreCase) >= 0);
                if (linkPattern != null)
                {
                    var linkArray = linkTypeGroup.ToArray();
                    foreach (var link in linkTypeGroup)
                    {
                        var replacedLink = Regex.Replace(
                            linkPattern,
                            typePattern,
                            link.url ?? string.Empty,
                            RegexOptions.IgnoreCase
                        );
                        link.url = Uri.EscapeUriString(replacedLink);
                    }
                }
            }
        }
    }
}