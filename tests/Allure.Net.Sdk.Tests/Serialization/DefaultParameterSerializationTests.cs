using Allure.Abstractions;
using Allure.Net.Sdk.Tests.Infrastructure;

namespace Allure.Net.Sdk.Tests.Serialization;

public class DefaultParameterSerializationTests
{
    [Test]
    public async Task ShouldSerializeNullAsNullLiteral()
    {
        var serializer = CreateSerializer();

        await Assert.That(serializer.Serialize(null)).IsEqualTo("null");
    }

    [Test]
    [Arguments("text", "\"text\"")]
    [Arguments("a \"quote\"", "\"a \\\"quote\\\"\"")]
    [Arguments("line\nbreak", "\"line\\nbreak\"")]
    public async Task ShouldSerializeStringsAsJsonStrings(
        string value,
        string expected
    )
    {
        var serializer = CreateSerializer();

        await Assert.That(serializer.Serialize(value)).IsEqualTo(expected);
    }

    [Test]
    [Arguments(17, "17")]
    [Arguments(-23, "-23")]
    public async Task ShouldSerializeIntegersAsJsonNumbers(
        int value,
        string expected
    )
    {
        var serializer = CreateSerializer();

        await Assert.That(serializer.Serialize(value)).IsEqualTo(expected);
    }

    [Test]
    [Arguments(true, "true")]
    [Arguments(false, "false")]
    public async Task ShouldSerializeBooleansAsJsonBooleans(
        bool value,
        string expected
    )
    {
        var serializer = CreateSerializer();

        await Assert.That(serializer.Serialize(value)).IsEqualTo(expected);
    }

    [Test]
    public async Task ShouldSerializeEnumsAsJsonStrings()
    {
        var serializer = CreateSerializer();

        await Assert.That(serializer.Serialize(SampleEnum.Second))
            .IsEqualTo("\"Second\"");
    }

    [Test]
    public async Task ShouldSerializeCollectionsAsJson()
    {
        var serializer = CreateSerializer();
        var value = new Dictionary<string, int[]>
        {
            ["first"] = [1, 2],
            ["second"] = [3],
        };

        await Assert.That(serializer.Serialize(value))
            .IsEqualTo("""{"first":[1,2],"second":[3]}""");
    }

    [Test]
    public async Task ShouldOmitNullObjectProperties()
    {
        var serializer = CreateSerializer();
        var value = new SampleObject
        {
            PascalCaseName = "value",
            OptionalValue = null,
        };

        await Assert.That(serializer.Serialize(value))
            .IsEqualTo("""{"PascalCaseName":"value"}""");
    }

    [Test]
    public async Task ShouldIgnoreReferenceCycles()
    {
        var serializer = CreateSerializer();
        var value = new CyclicObject { Name = "root" };
        value.Next = value;

        await Assert.That(serializer.Serialize(value))
            .IsEqualTo("""{"Name":"root"}""");
    }

    [Test]
    public async Task ShouldIgnoreDelegateProperties()
    {
        var serializer = CreateSerializer();
        var value = new ObjectWithDelegate
        {
            Name = "value",
            Callback = () => { },
        };

        await Assert.That(serializer.Serialize(value))
            .IsEqualTo("""{"Name":"value"}""");
    }

    [Test]
    public async Task ShouldSerializeJsonObjectsThatOverrideToStringAsStrings()
    {
        var serializer = CreateSerializer();
        var value = new ObjectWithCustomToString { Value = 17 };

        await Assert.That(serializer.Serialize(value))
            .IsEqualTo("\"custom: 17\"");
    }

    [Test]
    public async Task ShouldUseInheritedToStringOverrideForJsonObjects()
    {
        var serializer = CreateSerializer();
        var value = new ObjectInheritingCustomToString();

        await Assert.That(serializer.Serialize(value))
            .IsEqualTo("\"inherited\"");
    }

    [Test]
    public async Task ShouldSerializeCollectionsThatOverrideToStringAsJsonCollections()
    {
        var serializer = CreateSerializer();
        var value = new CollectionWithCustomToString { 1, 2 };

        await Assert.That(serializer.Serialize(value)).IsEqualTo("[1,2]");
    }

    [Test]
    public async Task ShouldUseToStringWhenJsonSerializationThrows()
    {
        var serializer = CreateSerializer();
        var value = new ThrowingObject();

        await Assert.That(serializer.Serialize(value))
            .IsEqualTo(typeof(ThrowingObject).FullName);
    }

    static IAllureParameterSerializer CreateSerializer() =>
        RuntimeTestEnvironment.Create().Runtime.ParameterSerializer;

    enum SampleEnum
    {
        First,
        Second,
    }

    sealed class SampleObject
    {
        public required string PascalCaseName { get; init; }

        public string? OptionalValue { get; init; }
    }

    sealed class CyclicObject
    {
        public required string Name { get; init; }

        public CyclicObject? Next { get; set; }
    }

    sealed class ObjectWithDelegate
    {
        public required string Name { get; init; }

        public required Action Callback { get; init; }
    }

    sealed class ObjectWithCustomToString
    {
        public int Value { get; init; }

        public override string ToString() => $"custom: {this.Value}";
    }

    abstract class ObjectWithInheritedToString
    {
        public override string ToString() => "inherited";
    }

    sealed class ObjectInheritingCustomToString : ObjectWithInheritedToString;

    sealed class CollectionWithCustomToString : List<int>
    {
        public override string ToString() => "collection";
    }

    sealed class ThrowingObject
    {
        public string ThrowingProperty => throw new InvalidOperationException();
    }
}
