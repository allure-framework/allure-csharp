using System;
using Allure.TestingPlatform.Registration;

namespace Allure.TestingPlatform.Sdk;

public interface IAllureRuntimeBuilder : IAllureRegistrationContext
{
    IAllureRuntime Build(IServiceProvider serviceProvider);
}