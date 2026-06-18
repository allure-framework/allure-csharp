using System;
using Allure.TestingPlatform.Registration;
using Microsoft.Testing.Platform.Builder;
using Microsoft.Testing.Platform.Services;

namespace Allure.TestingPlatform;

public static class AllureMtpExtensions
{
    public static void AddAllure(
        this ITestApplicationBuilder builder,
        Action<IAllureRegistrationContext> allureRegistration
    )
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

            return new AllureDataConsumer(
                allureBuilder.Build()
            );
        });
    }

    public static void AddAllure(this ITestApplicationBuilder builder) =>
        AddAllure(builder, static (_) => {});
}