namespace Allure.TestingPlatform.Internal.Correlation;

sealed record class CorrelationFailure(string Message) : CorrelationResult;
