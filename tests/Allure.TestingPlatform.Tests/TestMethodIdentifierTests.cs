using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.TestHost;
using Allure.TestingPlatform.Tests.Stubs;
using Allure.TestingPlatform.Sdk.Messages;
using Allure.TestingPlatform.Sdk.Properties;

namespace Allure.TestingPlatform.Tests;

public class TestMethodIdentifierTests : DataConsumerTestsBase
{
    [Test]
    public async Task ShouldFillFullNameFromMethodId()
    {
        var testNode = new TestNode
        {
            DisplayName = "Foo",
            Uid = "1",
            Properties = new(
                new PassedTestNodeStateProperty(),
                new TestMethodIdentifierProperty(
                    assemblyFullName: "Foo",
                    @namespace: "Bar",
                    typeName: "Baz",
                    methodName: "Qux",
                    methodArity: 0,
                    parameterTypeFullNames: [],
                    returnTypeFullName: "Qut"
                )
            )
        };

        var message = new TestNodeUpdateMessage(new SessionUid("Bar"), testNode);

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, message, CancellationToken.None);

        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        await Assert.That(testResult.fullName).IsEqualTo("Foo:Bar.Baz.Qux()");
    }

    [Test]
    public async Task ShouldUseParameterTypes()
    {
        var testNode = new TestNode
        {
            DisplayName = "Foo",
            Uid = "1",
            Properties = new(
                new PassedTestNodeStateProperty(),
                new TestMethodIdentifierProperty(
                    assemblyFullName: "Foo",
                    @namespace: "Bar",
                    typeName: "Baz",
                    methodName: "Qux",
                    methodArity: 2,
                    parameterTypeFullNames: ["Param1", "Param2"],
                    returnTypeFullName: "Qut"
                )
            )
        };
        var message = new TestNodeUpdateMessage(new SessionUid("Bar"), testNode);

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, message, CancellationToken.None);

        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        await Assert.That(testResult.fullName).IsEqualTo("Foo:Bar.Baz.Qux(Param1,Param2)");
    }

    [Test]
    public async Task ShouldSetTitlePath()
    {
        var testNode = new TestNode
        {
            DisplayName = "Foo",
            Uid = "1",
            Properties = new(
                new PassedTestNodeStateProperty(),
                new TestMethodIdentifierProperty(
                    assemblyFullName: "Foo",
                    @namespace: "Bar",
                    typeName: "Baz",
                    methodName: "Qux",
                    methodArity: 2,
                    parameterTypeFullNames: [],
                    returnTypeFullName: "Qut"
                )
            )
        };
        var message = new TestNodeUpdateMessage(new SessionUid("Bar"), testNode);

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, message, CancellationToken.None);

        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        await Assert.That(testResult.titlePath).IsEquivalentTo(
            ["Foo", "Bar", "Baz"],
            TUnit.Assertions.Enums.CollectionOrdering.Matching);
    }

    [Test]
    public async Task ShouldIncludeMethodNameForParameterizedTests()
    {
        var testNode = new TestNode
        {
            DisplayName = "Foo",
            Uid = "1",
            Properties = new(
                new PassedTestNodeStateProperty(),
                new TestMethodIdentifierProperty(
                    assemblyFullName: "Foo",
                    @namespace: "Bar",
                    typeName: "Baz",
                    methodName: "Qux",
                    methodArity: 2,
                    parameterTypeFullNames: ["Param1", "Param2"],
                    returnTypeFullName: "Qut"
                )
            )
        };
        var message = new TestNodeUpdateMessage(new SessionUid("Bar"), testNode);

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, message, CancellationToken.None);

        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        await Assert.That(testResult.titlePath).IsEquivalentTo(
            ["Foo", "Bar", "Baz", "Qux(Param1,Param2)"],
            TUnit.Assertions.Enums.CollectionOrdering.Matching);
    }

    [Test]
    public async Task ShouldSetDefaultSuiteLabels()
    {
        var testNode = new TestNode
        {
            DisplayName = "Foo",
            Uid = "1",
            Properties = new(
                new PassedTestNodeStateProperty(),
                new TestMethodIdentifierProperty(
                    assemblyFullName: "Foo",
                    @namespace: "Bar",
                    typeName: "Baz",
                    methodName: "Qux",
                    methodArity: 0,
                    parameterTypeFullNames: [],
                    returnTypeFullName: "Qut"
                )
            )
        };
        var message = new TestNodeUpdateMessage(new SessionUid("Bar"), testNode);

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, message, CancellationToken.None);

        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        var parentSuite = await Assert.That(testResult.labels).HasSingleItem(l => l.name == "parentSuite");
        var suite = await Assert.That(testResult.labels).HasSingleItem(l => l.name == "suite");
        var subSuite = await Assert.That(testResult.labels).HasSingleItem(l => l.name == "subSuite");
        await Assert.That(parentSuite.value).IsEqualTo("Foo");
        await Assert.That(suite.value).IsEqualTo("Bar");
        await Assert.That(subSuite.value).IsEqualTo("Baz");
    }

    [Test]
    public async Task ShouldNotSetDefaultSuiteLabelsIfParentSuiteAlreadyProvided()
    {
        var testNode = new TestNode
        {
            DisplayName = "Foo",
            Uid = "1",
            Properties = new(
                new PassedTestNodeStateProperty(),
                new TestMethodIdentifierProperty(
                    assemblyFullName: "Foo",
                    @namespace: "Bar",
                    typeName: "Baz",
                    methodName: "Qux",
                    methodArity: 0,
                    parameterTypeFullNames: [],
                    returnTypeFullName: "Qut"
                )
            )
        };
        var testStart = new TestNodeUpdateMessage(new SessionUid("Bar"), new ()
        {
            DisplayName = "Foo",
            Uid = "1",
            Properties = new(
                InProgressTestNodeStateProperty.CachedInstance
            )
        });
        var testUpdate = new AllureTestUpdateMessage(new("Bar"), new("1"))
        {
            Properties = [
                new AllureLabelsProperty([new (){ name = "parentSuite", value = "foo" }])
            ],
        };
        var testStop = new TestNodeUpdateMessage(new SessionUid("Bar"), new ()
        {
            DisplayName = "Foo",
            Uid = "1",
            Properties = new(
                new PassedTestNodeStateProperty(),
                new TestMethodIdentifierProperty(
                    assemblyFullName: "Foo",
                    @namespace: "Bar",
                    typeName: "Baz",
                    methodName: "Qux",
                    methodArity: 0,
                    parameterTypeFullNames: [],
                    returnTypeFullName: "Qut"
                )
            )
        });

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, testStart, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, testUpdate, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, testStop, CancellationToken.None);

        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        var parentSuite = await Assert.That(testResult.labels).HasSingleItem(l => l.name == "parentSuite");
        await Assert.That(parentSuite.value).IsEqualTo("foo");
        await Assert.That(testResult.labels).DoesNotContain(
            l => l.name == "suite"
        ).And.DoesNotContain(
            l => l.name == "subSuite"
        );
    }

    [Test]
    public async Task ShouldNotSetDefaultSuiteLabelsIfSuiteAlreadyProvided()
    {
        var testNode = new TestNode
        {
            DisplayName = "Foo",
            Uid = "1",
            Properties = new(
                new PassedTestNodeStateProperty(),
                new TestMethodIdentifierProperty(
                    assemblyFullName: "Foo",
                    @namespace: "Bar",
                    typeName: "Baz",
                    methodName: "Qux",
                    methodArity: 0,
                    parameterTypeFullNames: [],
                    returnTypeFullName: "Qut"
                )
            )
        };
        var testStart = new TestNodeUpdateMessage(new SessionUid("Bar"), new ()
        {
            DisplayName = "Foo",
            Uid = "1",
            Properties = new(
                InProgressTestNodeStateProperty.CachedInstance
            )
        });
        var testUpdate = new AllureTestUpdateMessage(new("Bar"), new("1"))
        {
            Properties = [
                new AllureLabelsProperty([new (){ name = "suite", value = "foo" }])
            ],
        };
        var testStop = new TestNodeUpdateMessage(new SessionUid("Bar"), new ()
        {
            DisplayName = "Foo",
            Uid = "1",
            Properties = new(
                new PassedTestNodeStateProperty(),
                new TestMethodIdentifierProperty(
                    assemblyFullName: "Foo",
                    @namespace: "Bar",
                    typeName: "Baz",
                    methodName: "Qux",
                    methodArity: 0,
                    parameterTypeFullNames: [],
                    returnTypeFullName: "Qut"
                )
            )
        });

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, testStart, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, testUpdate, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, testStop, CancellationToken.None);

        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        var parentSuite = await Assert.That(testResult.labels).HasSingleItem(l => l.name == "suite");
        await Assert.That(parentSuite.value).IsEqualTo("foo");
        await Assert.That(testResult.labels).DoesNotContain(
            l => l.name == "parentSuite"
        ).And.DoesNotContain(
            l => l.name == "subSuite"
        );
    }

    [Test]
    public async Task ShouldNotSetDefaultSuiteLabelsIfSubSuiteAlreadyProvided()
    {
        var testNode = new TestNode
        {
            DisplayName = "Foo",
            Uid = "1",
            Properties = new(
                new PassedTestNodeStateProperty(),
                new TestMethodIdentifierProperty(
                    assemblyFullName: "Foo",
                    @namespace: "Bar",
                    typeName: "Baz",
                    methodName: "Qux",
                    methodArity: 0,
                    parameterTypeFullNames: [],
                    returnTypeFullName: "Qut"
                )
            )
        };
        var testStart = new TestNodeUpdateMessage(new SessionUid("Bar"), new ()
        {
            DisplayName = "Foo",
            Uid = "1",
            Properties = new(
                InProgressTestNodeStateProperty.CachedInstance
            )
        });
        var testUpdate = new AllureTestUpdateMessage(new("Bar"), new("1"))
        {
            Properties = [
                new AllureLabelsProperty([new (){ name = "subSuite", value = "foo" }])
            ],
        };
        var testStop = new TestNodeUpdateMessage(new SessionUid("Bar"), new ()
        {
            DisplayName = "Foo",
            Uid = "1",
            Properties = new(
                new PassedTestNodeStateProperty(),
                new TestMethodIdentifierProperty(
                    assemblyFullName: "Foo",
                    @namespace: "Bar",
                    typeName: "Baz",
                    methodName: "Qux",
                    methodArity: 0,
                    parameterTypeFullNames: [],
                    returnTypeFullName: "Qut"
                )
            )
        });

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, testStart, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, testUpdate, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, testStop, CancellationToken.None);

        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        var parentSuite = await Assert.That(testResult.labels).HasSingleItem(l => l.name == "subSuite");
        await Assert.That(parentSuite.value).IsEqualTo("foo");
        await Assert.That(testResult.labels).DoesNotContain(
            l => l.name == "parentSuite"
        ).And.DoesNotContain(
            l => l.name == "suite"
        );
    }
}
