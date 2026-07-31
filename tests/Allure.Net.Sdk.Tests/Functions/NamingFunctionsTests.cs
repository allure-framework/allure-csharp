using System.Reflection;
using Allure.Sdk.Functions;

namespace Allure.Net.Sdk.Tests.Functions;

public class NamingFunctionsTests
{
    [Test]
    public async Task ShouldSerializeTypesAndGenericMethodSignatures()
    {
        var method = typeof(Sample<int>).GetMethod(nameof(Sample<int>.GenericMethod))!
            .MakeGenericMethod(typeof(string));

        await Assert.That(ReflectionNames.ForType(typeof(string))).IsEqualTo("System.String");
        await Assert.That(ReflectionNames.ForType(typeof(Sample<int>)))
            .Contains("Allure.Net.Sdk.Tests:Allure.Net.Sdk.Tests.Functions.NamingFunctionsTests+Sample`1[System.Int32]");
        await Assert.That(ReflectionNames.ForMethodSignature(method))
            .Contains("GenericMethod[TMethod](TMethod)");
    }

    [Test]
    public async Task ShouldCreateTitlePathsForTypesAndParameterizedMethods()
    {
        var typePath = Titles.PathFor(typeof(NamedSample));
        var namedMethod = typeof(NamedSample).GetMethod(nameof(NamedSample.Named), BindingFlags.Instance | BindingFlags.Public)!;
        var parameterlessMethod = typeof(NamedSample).GetMethod(nameof(NamedSample.Parameterless), BindingFlags.Instance | BindingFlags.Public)!;

        var namedMethodPath = Titles.PathFor(namedMethod);
        var parameterlessPath = Titles.PathFor(parameterlessMethod);

        await Assert.That(typePath[^1]).IsEqualTo("Named type");
        await Assert.That(namedMethodPath[^1]).IsEqualTo("Named method");
        await Assert.That(parameterlessPath.Count).IsEqualTo(typePath.Count);
    }

    class Sample<T>
    {
        public void GenericMethod<TMethod>(TMethod value) { }
    }

    [AllureName("Named type")]
    class NamedSample
    {
        [AllureName("Named method")]
        public void Named(int value) { }

        public void Parameterless() { }
    }
}
