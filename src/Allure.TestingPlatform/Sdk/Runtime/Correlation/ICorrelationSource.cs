using System.Threading;
using System.Threading.Tasks;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Allure.TestingPlatform.Sdk.Runtime.Correlation;

public interface ICorrelationSource
{
    Task<CorrelationUid?> GetCorrelationAsync(
        IDataProducer dataProducer,
        DataWithSessionUid message,
        CancellationToken cancellationToken
    );
}
