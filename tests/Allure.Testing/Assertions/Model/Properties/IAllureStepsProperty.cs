using System.Collections.Immutable;

namespace Allure.Testing.Assertions.Model.Properties;

[GenerateAllureAssertions]
public interface IAllureStepsProperty<TSelf> : IAllureObjectArrayProperty<AllureStepResult, TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllureStepsProperty<TSelf>
{
    static string IAllureProperty<ImmutableArray<AllureStepResult>, TSelf>.PropertyName { get; } =
        "steps";
}
