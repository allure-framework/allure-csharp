using System;
using System.Collections.Immutable;
using System.Text.Json;

namespace Allure.Testing.Assertions.Model.AssertionTargets.Properties;

public interface IAllureStepsProperty : IAllureArrayProperty<AllureStepResult, IAllureStepsProperty>
{
    static string IAllureProperty<ImmutableArray<AllureStepResult>, IAllureStepsProperty>.PropertyName { get; } =
        "steps";

    static Func<JsonElement, AllureStepResult> IAllureArrayProperty<AllureStepResult, IAllureStepsProperty>.Factory { get; }
        = json => new(json);
}
