using System;
using Allure.TestingPlatform.Registration;
using Microsoft.Testing.Platform.Builder;
using Microsoft.Testing.Platform.Messages;
using Microsoft.Testing.Platform.Services;

namespace Allure.TestingPlatform;

public static class AllureMtpExtensions
{
    static IMessageBus? messageBus = null;
    static IAllureInfrastructure? allure = null;

    extension (ITestApplicationBuilder builder)
    {
        public void AddAllure(Action<IAllureRegistrationContext> allureRegistration)
        {
            var allureBuilder = new AllureInfrastructureBuilder();

            builder.CommandLine.AddProvider(() => new AllureCliOptionsProvider());

            allureRegistration(allureBuilder);

            builder.TestHost.AddDataConsumer((serviceProvider) =>
            {
                var options = serviceProvider.GetCommandLineOptions();

                allureBuilder.SetEnabled(
                    AllureCliOptionsProvider.IsAllureEnabled(options)
                );

                messageBus = serviceProvider.GetMessageBus();
                allure = allureBuilder.Build();
                return new AllureDataConsumer(allure);
            });
        }

        public void AddAllure() =>
            AddAllure(builder, static (_) => {});
    }

    extension (IAllureInfrastructure)
    {
        public static IMessageBus MessageBus =>
            messageBus ?? throw new InvalidOperationException("Allure is not initialized");

        public static IAllureInfrastructure Allure =>
            allure ?? throw new InvalidOperationException("Allure is not initialized");
    }
}
