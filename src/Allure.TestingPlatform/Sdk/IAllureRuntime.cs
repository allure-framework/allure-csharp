using System;
using System.Collections.Immutable;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk;
using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Sdk;

public interface IAllureRuntime
{
    AllureConfiguration Config { get; }

    ILogger Logger { get; }

    ICorrelationService CorrelationService { get; }

    IAllureResultsWriter Writer { get; }

    ImmutableDictionary<Type, ITypeFormatter> TypeFormatters { get; }

    AllureLifecycle Lifecycle { get; }
}