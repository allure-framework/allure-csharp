using System.Collections.Generic;
using Allure.Net.Commons;

namespace Allure.TestingPlatform.Properties;

public sealed class AllureLinksProperty(IEnumerable<Link> links) : IAllureProperty<TestResult>
{
    public List<Link> Links { get; } = [..links];

    public void Apply(IAllureInfrastructure _, TestResult obj)
    {
        obj.links.AddRange(this.Links);
    }
}