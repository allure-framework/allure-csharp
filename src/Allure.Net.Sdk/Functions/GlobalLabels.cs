using System.Collections.Generic;
using System.Linq;
using Allure.Model;
using Allure.Sdk.Configuration;

namespace Allure.Sdk.Functions;

public static class GlobalLabels
{
    /// <summary>
    /// Returns a sequence of labels defined by the <c>globalLabels</c>
    /// configuration property.
    /// </summary>
    public static IEnumerable<Label> Enumerate(AllureConfiguration config) =>
        from kv in config.GlobalLabels ?? []
        where !string.IsNullOrEmpty(kv.Key) && !string.IsNullOrEmpty(kv.Value)
        select new Label { Name = kv.Key, Value = kv.Value };
}
