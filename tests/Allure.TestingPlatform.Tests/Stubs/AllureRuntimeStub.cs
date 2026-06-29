// using System.Collections.Immutable;
// using Allure.Net.Commons;
// using Allure.Net.Commons.Configuration;
// using Allure.Net.Commons.Sdk;
// using Allure.Net.Commons.Sdk.Writers;
// using Allure.TestingPlatform.Sdk;
// using Microsoft.Testing.Platform.Logging;

// namespace Allure.TestingPlatform.Tests.Stubs;

// public class AllureRuntimeStub(
//     AllureConfiguration config,
//     ILogger logger,
//     ICorrelationSource correlationService,
//     InMemoryResultsWriter writer,
//     AllureLifecycle lifecycle,
//     ImmutableDictionary<Type, ITypeFormatter> typeFormatters
// )
//     : IAllureRuntime
// {
//     public AllureConfiguration Config => config;

//     public ILogger Logger => logger;

//     public ICorrelationSource CorrelationService => correlationService;

//     public IAllureResultsWriter Writer => writer;

//     public AllureLifecycle Lifecycle => lifecycle;

//     public ImmutableDictionary<Type, ITypeFormatter> TypeFormatters => typeFormatters;
// }