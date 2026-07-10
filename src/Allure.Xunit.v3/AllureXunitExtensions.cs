using System;
using System.Reflection;
using Allure.TestingPlatform.Registration;
using Allure.TestingPlatform.Sdk;
using Allure.Xunit.Internal;
using Microsoft.Testing.Platform.Builder;

namespace Allure.Xunit;

public static class AllureXunitExtensions
{
    extension (ITestApplicationBuilder builder)
    {
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

        public void AddAllureXunit() => AddAllureXunit(builder, static (_) => { });
    }

    static bool IsAllureXunitAttributeApplied =>
        Assembly.GetEntryAssembly().GetCustomAttribute<AllureXunitAttribute>() is not null;
}
