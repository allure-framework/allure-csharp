using System.Xml.Linq;
using Allure.TestingPlatform.Tests.Stubs;
using Microsoft.Testing.Platform.Builder;
using Microsoft.Testing.Platform.Capabilities.TestFramework;
using Microsoft.Testing.Platform.Services;

using DefaultConsumer = Allure.TestingPlatform.Sdk.TestingPlatformExtensions.AllureDataConsumer<
    Allure.TestingPlatform.Configuration.AllureTestingPlatformConfiguration,
    Allure.TestingPlatform.Sdk.Runtime.IAllureTestingPlatformRuntime<Allure.TestingPlatform.Configuration.AllureTestingPlatformConfiguration>
>;
using DefaultController = Allure.TestingPlatform.Sdk.TestingPlatformExtensions.AllureTestingPlatformInProcessRuntimeController<
    Allure.TestingPlatform.Configuration.AllureTestingPlatformConfiguration,
    Allure.TestingPlatform.Sdk.Runtime.IAllureTestingPlatformRuntime<Allure.TestingPlatform.Configuration.AllureTestingPlatformConfiguration>
>;

namespace Allure.TestingPlatform.Tests;

public class SelfRegistrationTests
{
    static readonly string[] DefaultArgs =
    [
        "--no-progress",
        "--no-ansi",
        "--output",
        "Normal",
        "--show-stdout",
        "None",
        "--show-stderr",
        "None",
        "--allure-watchdog",
        "off",
    ];

    [Test]
    public async Task ShouldRegisterAllureThroughBuilderHook()
    {
        IServiceProvider serviceProvider = null;

        var builder = await TestApplication.CreateBuilderAsync(
            DefaultArgs
        );
        AllureTestingPlatformBuilderHook.AddExtensions(builder, []);

        builder.RegisterTestFramework(
            _ => new TestFrameworkCapabilities(),
            (_, sp) =>
            {
                serviceProvider = sp;
                return new TestFrameworkStub();
            }
        );

        using var app = await builder.BuildAsync();
        var code = await app.RunAsync();

        await Assert.That(code).IsEqualTo(8); // test session run zero tests
        await Assert.That(serviceProvider.GetService<DefaultConsumer>()).IsNotNull();
        await Assert.That(serviceProvider.GetService<DefaultController>())
            .IsNotNull();
        await Assert.That(serviceProvider.GetService<DefaultConsumer>().IsEnabledAsync()).IsTrue();
    }

    [Test]
    public async Task ShouldAllowDisablingSelfRegisteredAllureThroughCliOption()
    {
        IServiceProvider serviceProvider = null;

        var builder = await TestApplication.CreateBuilderAsync(
            [..DefaultArgs, "--allure", "off"]
        );
        AllureTestingPlatformBuilderHook.AddExtensions(builder, []);

        builder.RegisterTestFramework(
            _ => new TestFrameworkCapabilities(),
            (_, sp) =>
            {
                serviceProvider = sp;
                return new TestFrameworkStub();
            }
        );

        using var app = await builder.BuildAsync();
        var code = await app.RunAsync();

        await Assert.That(code).IsEqualTo(8); // test session run zero tests
        await Assert.That(serviceProvider.GetService<DefaultConsumer>()).IsNull();
        await Assert.That(serviceProvider.GetService<DefaultController>())
            .IsNull();
    }

    [Test]
    public async Task ShouldEnablePackageSelfRegistrationByDefault()
    {
        var props = XDocument.Load(PackageFile("build", "Allure.TestingPlatform.props"));

        var property = props.Root
            .Element("PropertyGroup")
            .Element("Allure_TestingPlatformEnableSelfRegistration");

        await Assert.That(property.Value.Trim()).IsEqualTo("true");
        await Assert.That(property.Attribute("Condition").Value)
            .IsEqualTo(" '$(Allure_TestingPlatformEnableSelfRegistration)' == '' ");
    }

    [Test]
    public async Task ShouldShipTestingPlatformBuilderHookMetadata()
    {
        var targets = XDocument.Load(PackageFile("buildTransitive", "Allure.TestingPlatform.targets"));

        var itemGroup = targets.Root.Element("ItemGroup");
        var hook = itemGroup.Element("TestingPlatformBuilderHook");

        await Assert.That(itemGroup.Attribute("Condition").Value)
            .IsEqualTo(" '$(Allure_TestingPlatformEnableSelfRegistration)' == 'true' ");
        await Assert.That(hook.Attribute("Include").Value)
            .IsEqualTo("8ffa1264-cecb-419d-8a11-4f91f24ad5c5");
        await Assert.That(hook.Element("DisplayName").Value).IsEqualTo("Allure.TestingPlatform");
        await Assert.That(hook.Element("TypeFullName").Value)
            .IsEqualTo(typeof(AllureTestingPlatformBuilderHook).FullName);
    }

    static string PackageFile(string packageFolder, string fileName) =>
        Path.Combine(
            FindRepositoryRoot(),
            "src",
            "Allure.TestingPlatform",
            "package",
            packageFolder,
            fileName
        );

    static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "allure-csharp.slnx")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Could not find the repository root.");
    }
}
