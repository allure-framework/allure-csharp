using System.Collections.Generic;
using Allure.Net.Commons;

namespace Allure.TestingPlatform.Sdk.Properties;

public sealed class AllureLabelsProperty(IEnumerable<Label> labels) : IAllureProperty<TestResult>
{
    public List<Label> Labels { get; } = [..labels];

    public void Apply(IAllureInfrastructure _, TestResult obj)
    {
        obj.labels.AddRange(this.Labels);
    }
}