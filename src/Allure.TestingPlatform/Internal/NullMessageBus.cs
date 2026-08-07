using System.Threading.Tasks;
using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.Messages;

namespace Allure.TestingPlatform.Internal;

class NullMessageBus : IMessageBus
{
    public Task PublishAsync(IDataProducer dataProducer, IData data) =>
        Task.CompletedTask;

    public static NullMessageBus Instance { get; } = new();
}
