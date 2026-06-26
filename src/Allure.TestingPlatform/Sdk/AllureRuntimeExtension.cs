// using System;
// using Allure.Net.Commons;
// using Allure.Net.Commons.Configuration;
// using Allure.Net.Commons.Sdk;
// using Microsoft.Testing.Platform.Logging;

// namespace Allure.TestingPlatform.Sdk;

// public abstract class AllureRuntimeExtension(
//     string uid,
//     string displayName,
//     string description,
//     IServiceProvider serviceProvider
// ) :
//     AllureTestingPlatformExtension(uid, displayName, description, serviceProvider)
// {
//     readonly Lazy<IAllureRuntimeProvider> runtimeProvider = new(runtimeProviderFactory);

//     protected IAllureRuntimeProvider RuntimeProvider => runtimeProvider.Value;

//     protected IAllureRuntime Allure => this.RuntimeProvider.Runtime;

//     protected ILogger Logger => this.Allure.Logger;

//     protected AllureConfiguration Config => this.Allure.Config;

//     protected IAllureResultsWriter Writer => this.Allure.Writer;

//     protected AllureLifecycle Lifecycle => this.Allure.Lifecycle;
// }