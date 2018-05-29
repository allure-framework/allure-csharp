using System;
using System.Collections.Generic;
using System.Linq;

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
                var linkType = linkTypeGroup.Key.ToLower();
                var match = $"{{{linkType}}}";
                var pattern = patterns.FirstOrDefault(x => x.ToLower().Contains(match));
                if (pattern != null)
                {
                    pattern = pattern.ToLower();
                    var linkArray = linkTypeGroup.ToArray();
                    for (int i = 0; i < linkArray.Length; i++)
                    {
                        linkArray[i].url = Uri.EscapeUriString(pattern.Replace(match, linkArray[i].url));
                    }
                }
            }
        }
    }
}
