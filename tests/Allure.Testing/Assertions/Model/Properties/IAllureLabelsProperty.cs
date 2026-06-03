using System.Collections.Immutable;

namespace Allure.Testing.Assertions.Model.Properties;

[GenerateAllureAssertions]
public interface IAllureLabelsProperty<TSelf> : IAllureObjectArrayProperty<AllureLabel, TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllureLabelsProperty<TSelf>
{
}
