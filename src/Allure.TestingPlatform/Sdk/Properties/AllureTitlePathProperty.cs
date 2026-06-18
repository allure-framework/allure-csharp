using System.Collections.Generic;
using Allure.Net.Commons;

namespace Allure.TestingPlatform.Sdk.Properties;

public sealed class AllureTitlePathProperty(IEnumerable<string> titlePath) : IAllureProperty<TestResult>
{
    public List<string> TitlePath { get; } = [..titlePath];

    public void Apply(IAllureInfrastructure _, TestResult obj)
    {
        obj.titlePath = [..this.TitlePath];
    }
}