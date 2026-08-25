using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Allure.TestingPlatform.Sdk.Correlation;

namespace Allure.Xunit.Internal.Functions;

static class XunitTraits
{
    public static bool TryGetCorrelationUid(
        IReadOnlyDictionary<string, IReadOnlyCollection<string>> traits,
        [MaybeNullWhen(false)] out CorrelationUid correlationUid
    )
    {
        if (traits.TryGetValue(
                TestNodeMetadataCorrelationStrategy.MetadataKey,
                out var metadataValue
            )
                && metadataValue.Count == 1
        )
        {
            correlationUid = new(metadataValue.First());
            return true;
        }

        correlationUid = default;
        return false;
    }
}
