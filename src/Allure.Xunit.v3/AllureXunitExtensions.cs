using System;
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
                ctx.UseTestNodeMetadataCorrelation();
            });

            builder.TestHost.AddTestHostApplicationLifetime((serviceProvider) =>
                new AllureTestingPlatformServices(
                    serviceProvider,
                    allureRuntimeReferences.GetRuntimeReference(serviceProvider)
                )
            );
        }

        public void AddAllureXunit() => AddAllureXunit(builder, static (_) => { });
    }
}
