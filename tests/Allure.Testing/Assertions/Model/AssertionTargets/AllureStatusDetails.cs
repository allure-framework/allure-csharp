using System.Text.Json;
using Allure.Testing.Assertions.Model.AssertionTargets.Properties;

namespace Allure.Testing.Assertions.Model.AssertionTargets;

public readonly record struct AllureStatusDetails(JsonElement Json) :
    IAllureJsonObject,
    IAllureMessageProperty { }
