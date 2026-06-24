using System;
using Allure.TestingPlatform.Implementation;
using Allure.TestingPlatform.Registration;
using Microsoft.Testing.Platform.Builder;
using Microsoft.Testing.Platform.Extensions;

namespace Allure.TestingPlatform;

public static class AllureMtpExtensions
{
    extension (ITestApplicationBuilder builder)
    {
        public void AddAllure(Action<IAllureRegistrationContext> allureRegistration)
        {
            var allureBuilder = new AllureRuntimeBuilder();

            builder.CommandLine.AddProvider(() => new AllureCliOptionsProvider());

            allureRegistration(allureBuilder);

            var factory =
                new CompositeExtensionFactory<AllureDataConsumer>((serviceProvider) =>
                    new AllureDataConsumer(serviceProvider)
                );

            builder.TestHost.AddTestHostApplicationLifetime((serviceProvider) =>
                new AllureMtpRuntimeProvider(serviceProvider, allureBuilder)
            );
            builder.TestHost.AddDataConsumer(factory);
            builder.TestHost.AddTestSessionLifetimeHandler(factory);
        }

        public void AddAllure() =>
            AddAllure(builder, static (_) => {});
    }
}
