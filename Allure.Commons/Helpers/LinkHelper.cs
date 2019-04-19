using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Allure.Commons.Helpers
{
    public class LinkHelper
    {
        public static void UpdateLinks(IEnumerable<Link> links, HashSet<string> patterns)
        {
            foreach (var linkTypeGroup in links
                .Where(l => !string.IsNullOrWhiteSpace(l.type))
                .GroupBy(l => l.type))
            {
                var typePattern = $"{{{linkTypeGroup.Key}}}";
                var linkPattern = patterns.FirstOrDefault(x =>
                    x.IndexOf(typePattern, StringComparison.CurrentCultureIgnoreCase) >= 0);
                if (linkPattern != null)
                {
                    var linkArray = linkTypeGroup.ToArray();
                    for (var i = 0; i < linkArray.Length; i++)
                    {
                        var replacedLink = Regex.Replace(linkPattern, typePattern, linkArray[i].url ?? string.Empty,
                            RegexOptions.IgnoreCase);
                        linkArray[i].url = Uri.EscapeUriString(replacedLink);
                    }
                }
            }
        }
    }
}