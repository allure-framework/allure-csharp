using System;
using System.Collections.Immutable;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk;

namespace Allure.TestingPlatform.Registration;

/// <summary>
/// Provides dependencies for creating an <see cref="AllureLifecycle"/>.
/// </summary>
/// <param name="Config">The resolved Allure configuration.</param>
/// <param name="Writer">The resolved Allure results writer.</param>
/// <param name="TypeFormatters">The resolved type formatters.</param>
public record class AllureLifecycleFactoryContext(
    AllureConfiguration Config,
    IAllureResultsWriter Writer,
    ImmutableDictionary<Type, ITypeFormatter> TypeFormatters
);
