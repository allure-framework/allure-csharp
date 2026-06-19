using System;
using System.Collections.Generic;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk;
using Allure.TestingPlatform.Sdk;

namespace Allure.TestingPlatform;

public interface IAllureInfrastructure
{
    bool IsEnabled { get; }
    AllureConfiguration Config { get; }
    ICorrelationService CorrelationService { get; }
    IAllureResultsWriter Writer { get; }
    AllureLifecycle Lifecycle { get; }
    IReadOnlyDictionary<Type, ITypeFormatter> TypeFormatters { get; }
}