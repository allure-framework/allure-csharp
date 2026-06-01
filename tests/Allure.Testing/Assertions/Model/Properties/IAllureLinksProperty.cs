using System.Collections.Immutable;

namespace Allure.Testing.Assertions.Model.Properties;

[GenerateAllureAssertions]
public interface IAllureLinksProperty<TSelf> : IAllureObjectArrayProperty<AllureLink, TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllureLinksProperty<TSelf>
{
}
