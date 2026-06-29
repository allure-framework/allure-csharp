using System.Collections.Generic;
using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.Runtime.AdapterState;

namespace Allure.TestingPlatform.Sdk.Properties;

public sealed class AllureLinksProperty(IEnumerable<Link> links) : IAllureProperty<TestResult>
{
    public List<Link> Links { get; } = [..links];

    public void Apply(ReadyAllureTestingPlatform _, TestResult obj)
    {
        obj.links.AddRange(this.Links);
    }
}