using System.Threading;
using System.Threading.Tasks;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Allure.TestingPlatform.Sdk.Runtime.Correlation;

public class SessionUidCorrelation : ICorrelationSource
{
    public Task<CorrelationUid?> GetCorrelationAsync(
        IDataProducer dataProducer,
        DataWithSessionUid message,
        CancellationToken cancellationToken
    ) =>
        Task.FromResult<CorrelationUid?>(
            new CorrelationUid(message.SessionUid.Value)
        );
}
