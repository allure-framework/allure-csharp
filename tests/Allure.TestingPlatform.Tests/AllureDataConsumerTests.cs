using System.Threading.Tasks;
using Allure.TestingPlatform;
using Microsoft.Testing.Platform.Extensions.Messages;
using NUnit.Framework;

namespace Allure.TestingPlatform.Tests;

[TestFixture]
public class AllureDataConsumerTests
{
    [Test]
    public void DataTypesConsumed_includes_TestNodeUpdateMessage()
    {
        var consumer = new AllureDataConsumer();

        Assert.That(consumer.DataTypesConsumed, Has.Member(typeof(TestNodeUpdateMessage)));
    }

    [Test]
    public async Task IsEnabledAsync_returns_true()
    {
        var consumer = new AllureDataConsumer();

        Assert.That(await consumer.IsEnabledAsync(), Is.True);
    }

    [Test]
    public void Extension_metadata_is_populated()
    {
        var consumer = new AllureDataConsumer();

        Assert.Multiple(() =>
        {
            Assert.That(consumer.Uid, Is.EqualTo("Allure.TestingPlatform"));
            Assert.That(consumer.DisplayName, Is.Not.Empty);
            Assert.That(consumer.Description, Is.Not.Empty);
            Assert.That(consumer.Version, Is.Not.Empty);
        });
    }
}
