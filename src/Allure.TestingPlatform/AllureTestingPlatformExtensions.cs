using System;
using Allure.TestingPlatform.Implementation;
using Allure.TestingPlatform.Registration;
using Allure.TestingPlatform.Sdk;
using Microsoft.Testing.Platform.Builder;
using Microsoft.Testing.Platform.Extensions;
using Microsoft.Testing.Platform.Services;

namespace Allure.TestingPlatform;

public static class AllureTestingPlatformExtensions
{
    extension (ITestApplicationBuilder builder)
    {
        public void AddAllure(Action<IAllureRegistrationContext> allureRegistration)
        {
            builder.CommandLine.AddProvider(() => new AllureCliOptionsProvider());

            var allureBuilder = new AllureRuntimeBuilder();
            allureRegistration(allureBuilder);

            var factory =
                new CompositeExtensionFactory<AllureDataConsumer>((serviceProvider) =>
                    new AllureDataConsumer(serviceProvider, serviceProvider.AllureExtensionSettings)
                );

            builder.TestHost.AddTestHostApplicationLifetime((serviceProvider) =>
            {
                var buildResult = allureBuilder.Build(serviceProvider);
                serviceProvider.AllureExtensionSettings = buildResult.ExtensionSettings;
                return new AllureRuntimeProvider(buildResult);
            });
            builder.TestHost.AddDataConsumer(factory);
            builder.TestHost.AddTestSessionLifetimeHandler(factory);
        }

        public void AddAllure() =>
            AddAllure(builder, static (_) => {});
    }
}
