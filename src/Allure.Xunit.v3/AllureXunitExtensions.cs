using System;
using System.Reflection;
using Allure.TestingPlatform.Sdk;
using Allure.TestingPlatform.Sdk.Registration;
using Allure.Xunit.Configuration;
using Allure.Xunit.Internal.Registration;
using Allure.Xunit.Runtime;
using Allure.Xunit.Registration;
using Microsoft.Testing.Platform.Builder;

namespace Allure.Xunit;

using IAllureXunitRegistration =
    IAllureTestingPlatformRegistration<AllureXunitConfiguration, AllureXunitRuntime>;

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
        /// <param name="registrationCallback">
        /// A callback that configures the Allure.TestingPlatform registration.
        /// </param>
        public IAllureXunitRegistration AddAllureXunit(
            Action<IAllureXunitRegistrationContext> registrationCallback
        )
        {
            if (AllureXunitRegistration.IsRegistered)
            {
                throw new InvalidOperationException(
                    "Allure.Xunit.v3 is already registered. "
                        + "Multiple registrations are not supported."
                );
            }

            var registration = builder.AddEmbeddedAllure(
                "Allure.Xunit.v3",
                () => new AllureXunitRegistrationSession(),
                (ctx, _) =>
                {
                    registrationCallback(ctx);
                    if (IsAllureXunitAttributeApplied)
                    {
                        ctx.UseTestNodeMetadataCorrelation();
                    }
                }
            );
            AllureXunitRegistration.Bind(registration);
            return registration;
        }

        /// <summary>
        /// Registers Allure.Xunit.v3 with Microsoft Testing Platform using the default configuration.
        /// </summary>
        public IAllureXunitRegistration AddAllureXunit() =>
            AddAllureXunit(builder, static (_) => { });
    }

    static bool IsAllureXunitAttributeApplied =>
        Assembly.GetEntryAssembly()
            .GetCustomAttribute<AllureXunitAttribute>() is not null;
}
