using System.Text.Json;

namespace Allure.Testing.Assertions.Model.AssertionTargets;

public readonly record struct AllureStepResult(JsonElement Json) : IAllureExecutableItem { }
