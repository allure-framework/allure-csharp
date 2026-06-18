using System;
using System.Collections.Generic;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk;

namespace Allure.TestingPlatform;

public interface IAllureInfrastructure
{
    bool IsEnabled { get; }
    AllureConfiguration Config { get; }
    IAllureResultsWriter Writer { get; }
    AllureLifecycle Lifecycle { get; }
    IReadOnlyDictionary<Type, ITypeFormatter> TypeFormatters { get; }
}