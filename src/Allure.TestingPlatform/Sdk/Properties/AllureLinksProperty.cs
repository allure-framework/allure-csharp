using System.Collections.Generic;
using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

public sealed class AllureLinksProperty(IEnumerable<Link> links) : IAllureProperty<TestResult>
{
    public List<Link> Links { get; } = [..links];

    public void Apply(LiveAllureTestingPlatformRuntime _, TestResult target)
    {
        target.links.AddRange(this.Links);
    }
}