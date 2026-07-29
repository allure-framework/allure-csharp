using System.Reflection;
using Allure;
using Allure.Model;
using Allure.Sdk.Functions;
using Allure.Net.Sdk.Tests.Infrastructure;

namespace Allure.Net.Sdk.Tests.Functions;

public class ParametersTests
{
    [Test]
    public async Task ShouldCreateParametersUsingAttributesAndSerializer()
    {
        var serializer = new RecordingParameterSerializer(value => $"<{value}>" );
        var method = typeof(ParametersTests).GetMethod(
            nameof(AttributedMethod),
            BindingFlags.Static | BindingFlags.NonPublic
        )!;

        var parameters = Parameters.Create(
            method.GetParameters(),
            ["visible", "hidden", "ignored"],
            serializer
        ).ToList();

        await Assert.That(parameters.Count).IsEqualTo(2);
        await Assert.That(parameters[0].Name).IsEqualTo("renamed");
        await Assert.That(parameters[0].Value).IsEqualTo("<visible>");
        await Assert.That(parameters[0].Mode).IsEqualTo(ParameterMode.Masked);
        await Assert.That(parameters[0].Excluded).IsTrue();
        await Assert.That(parameters[1].Name).IsEqualTo("hidden");
        await Assert.That(parameters[1].Mode).IsNull();
        await Assert.That(serializer.Values.Cast<string>())
            .IsEquivalentTo(["visible", "hidden"]);
    }

    [Test]
    public async Task ShouldTruncateMismatchedParameterSequences()
    {
        var serializer = new RecordingParameterSerializer(value => value?.ToString() ?? "null");

        var parameters = Parameters.Create(
            ["first", "second"],
            [null, null],
            [1],
            serializer
        ).ToList();

        await Assert.That(parameters.Count).IsEqualTo(1);
        await Assert.That(parameters[0].Name).IsEqualTo("first");
    }

    static void AttributedMethod(
        [AllureParameter(Name = "renamed", Mode = ParameterMode.Masked, Excluded = true)] string visible,
        string hidden,
        [AllureParameter(Ignore = true)] string ignored
    ) { }
}
