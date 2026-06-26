using System;
using Allure.Net.Commons.Configuration;

namespace Allure.TestingPlatform.Sdk;

public interface IAllureConfigurationBuilder
{
    AllureConfiguration BuildConfiguration(IServiceProvider serviceProvider);
}