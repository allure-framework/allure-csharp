using System;
using System.Collections.Generic;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk;
using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Registration;

/// <summary>
/// Configures a standalone Allure.TestingPlatform registration.
/// </summary>
public interface IStandaloneAllureRegistrationContext
{
    /// <summary>
    /// Disables the process watchdog that writes a global error when the test host crashes.
    /// </summary>
    IStandaloneAllureRegistrationContext DisableHostProcessWatchdog();

    /// <summary>
    /// Sets the factory used to create Allure configuration.
    /// </summary>
    IStandaloneAllureRegistrationContext UseConfiguration(
        Func<IServiceProvider, AllureConfiguration> configurationFactory
    );

    /// <summary>
    /// Sets the predicate used to decide whether Allure is enabled.
    /// </summary>
    IStandaloneAllureRegistrationContext SetIsEnabled(
        Func<IServiceProvider, AllureConfiguration, bool> isEnabled
    );

    /// <summary>
    /// Sets the factory used to create the Allure results writer.
    /// </summary>
    IStandaloneAllureRegistrationContext UseWriter(
        Func<IServiceProvider, AllureConfiguration, IAllureResultsWriter> writerFactory
    );

    /// <summary>
    /// Sets the factory used to create type formatters.
    /// </summary>
    IStandaloneAllureRegistrationContext UseTypeFormatters(
        Func<IServiceProvider, AllureConfiguration, IReadOnlyDictionary<Type, ITypeFormatter>> typeFormattersFactory
    );
}
