using System.Collections.Generic;
using Allure.TestingPlatform.Sdk.Correlation;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Allure.TestingPlatform.Internal.Correlation;

sealed record class CorrelationSuccess(
    CorrelationUid CorrelationUid,
    IEnumerable<IData> MessagesToProcess
) :
    CorrelationResult;
