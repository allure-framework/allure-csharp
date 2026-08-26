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
        await Assert.That(testResult.FullName).IsEqualTo("Foo:Bar.Baz.Qux()");
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
        await Assert.That(testResult.FullName).IsEqualTo("Foo:Bar.Baz.Qux(Param1,Param2)");
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
        await Assert.That(testResult.TitlePath).IsEquivalentTo(
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
        await Assert.That(testResult.TitlePath).IsEquivalentTo(
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
        var parentSuite = await Assert.That(testResult.Labels).HasSingleItem(l => l.Name == "parentSuite");
        var suite = await Assert.That(testResult.Labels).HasSingleItem(l => l.Name == "suite");
        var subSuite = await Assert.That(testResult.Labels).HasSingleItem(l => l.Name == "subSuite");
        await Assert.That(parentSuite.Value).IsEqualTo("Foo");
        await Assert.That(suite.Value).IsEqualTo("Bar");
        await Assert.That(subSuite.Value).IsEqualTo("Baz");
    }

    [Test]
    public async Task ShouldPreferProvidedDefaultSuitesToIdentifierFallbacks()
    {
        var testStart = new TestNodeUpdateMessage(new SessionUid("Bar"), new()
        {
            DisplayName = "Foo",
            Uid = "1",
            Properties = new(InProgressTestNodeStateProperty.CachedInstance)
        });
        var testUpdate = new AllureTestUpdateMessage(new("Bar"), new("1"))
        {
            Properties = [
                new AllureDefaultSuitesProperty("Provided Parent", "Provided Suite", "Provided Sub Suite")
            ],
        };
        var testStop = new TestNodeUpdateMessage(new SessionUid("Bar"), new()
        {
            DisplayName = "Foo",
            Uid = "1",
            Properties = new(
                new PassedTestNodeStateProperty(),
                new TestMethodIdentifierProperty(
                    assemblyFullName: "Fallback Parent",
                    @namespace: "Fallback Suite",
                    typeName: "Fallback Sub Suite",
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
        var parentSuite = await Assert.That(testResult.Labels).HasSingleItem(l => l.Name == "parentSuite");
        var suite = await Assert.That(testResult.Labels).HasSingleItem(l => l.Name == "suite");
        var subSuite = await Assert.That(testResult.Labels).HasSingleItem(l => l.Name == "subSuite");
        await Assert.That(parentSuite.Value).IsEqualTo("Provided Parent");
        await Assert.That(suite.Value).IsEqualTo("Provided Suite");
        await Assert.That(subSuite.Value).IsEqualTo("Provided Sub Suite");
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
                new AllureLabelsProperty([new (){ Name = "parentSuite", Value = "foo" }])
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
        var parentSuite = await Assert.That(testResult.Labels).HasSingleItem(l => l.Name == "parentSuite");
        await Assert.That(parentSuite.Value).IsEqualTo("foo");
        await Assert.That(testResult.Labels).DoesNotContain(
            l => l.Name == "suite"
        ).And.DoesNotContain(
            l => l.Name == "subSuite"
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
                new AllureLabelsProperty([new (){ Name = "suite", Value = "foo" }])
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
        var parentSuite = await Assert.That(testResult.Labels).HasSingleItem(l => l.Name == "suite");
        await Assert.That(parentSuite.Value).IsEqualTo("foo");
        await Assert.That(testResult.Labels).DoesNotContain(
            l => l.Name == "parentSuite"
        ).And.DoesNotContain(
            l => l.Name == "subSuite"
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
                new AllureLabelsProperty([new (){ Name = "subSuite", Value = "foo" }])
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
        var parentSuite = await Assert.That(testResult.Labels).HasSingleItem(l => l.Name == "subSuite");
        await Assert.That(parentSuite.Value).IsEqualTo("foo");
        await Assert.That(testResult.Labels).DoesNotContain(
            l => l.Name == "parentSuite"
        ).And.DoesNotContain(
            l => l.Name == "suite"
        );
    }
}
