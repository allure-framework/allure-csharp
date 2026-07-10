using System;
using System.Reflection;
using Allure.TestingPlatform.Registration;
using Allure.TestingPlatform.Sdk;
using Allure.Xunit.Internal;
using Microsoft.Testing.Platform.Builder;

namespace Allure.Xunit;

/// <summary>
/// Provides extension methods that register Allure.Xunit.v3 with Microsoft Testing Platform.
/// </summary>
public static class AllureXunitExtensions
{
    extension (ITestApplicationBuilder builder)
    {
        /// <summary>
        /// Registers Allure.Xunit.v3 with Microsoft Testing Platform and applies
        /// additional Allure.TestingPlatform configuration.
        /// </summary>
        /// <param name="allureRegistration">
        /// A callback that configures the Allure.TestingPlatform registration.
        /// </param>
        public void AddAllureXunit(Action<IStandaloneAllureRegistrationContext> allureRegistration)
        {
            var allureRuntimeReferences = builder.AddEmbeddedAllure((ctx) =>
            {
                allureRegistration(ctx);
                if (IsAllureXunitAttributeApplied)
                {
                    ctx.UseTestNodeMetadataCorrelation();
                }
            });

            builder.TestHost.AddTestHostApplicationLifetime((serviceProvider) =>
                new AllureXunitMtpServices(
                    serviceProvider,
                    allureRuntimeReferences.GetRuntimeReference(serviceProvider)
                )
            );
        }

        /// <summary>
        /// Registers Allure.Xunit.v3 with Microsoft Testing Platform using the default configuration.
        /// </summary>
        public void AddAllureXunit() => AddAllureXunit(builder, static (_) => { });
    }

    static bool IsAllureXunitAttributeApplied =>
        Assembly.GetEntryAssembly().GetCustomAttribute<AllureXunitAttribute>() is not null;
}
