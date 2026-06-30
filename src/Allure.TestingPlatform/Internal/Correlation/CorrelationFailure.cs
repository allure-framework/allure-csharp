using System.Collections.Generic;
using Allure.TestingPlatform.Sdk;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Allure.TestingPlatform.Internal.Correlation;

sealed record class CorrelationFailure(string Message) : CorrelationResult;
