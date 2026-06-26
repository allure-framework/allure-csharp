using System;
using Allure.TestingPlatform.Registration;

namespace Allure.TestingPlatform.Sdk;

public interface IAllureRuntimeBuilder : IAllureRegistrationContext
{
    IAllureRuntimeBuildResult Build(IServiceProvider serviceProvider);
}
