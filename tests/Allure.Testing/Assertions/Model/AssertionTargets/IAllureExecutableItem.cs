using Allure.Testing.Assertions.Model.AssertionTargets.Properties;

namespace Allure.Testing.Assertions.Model.AssertionTargets;

public interface IAllureExecutableItem :
    IAllureJsonObject,
    IAllureNameProperty,
    IAllureStepsProperty,
    IAllureStatusDetailsProperty { }
