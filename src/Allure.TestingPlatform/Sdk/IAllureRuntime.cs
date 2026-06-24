using System;
using System.Collections.Generic;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk;
using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Sdk;

public interface IAllureRuntime
{
    AllureConfiguration Config { get; }
    ILogger Logger { get; }
    bool IsEnabled { get; }
    ICorrelationService CorrelationService { get; }
    IAllureResultsWriter Writer { get; }
    AllureLifecycle Lifecycle { get; }
    IReadOnlyDictionary<Type, ITypeFormatter> TypeFormatters { get; }
}