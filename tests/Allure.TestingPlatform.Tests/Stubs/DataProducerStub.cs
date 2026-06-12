using Microsoft.Testing.Platform.Extensions.Messages;

namespace Allure.TestingPlatform.Tests.Stubs;

public class DataProducerStub : IDataProducer
{
    public Type[] DataTypesProduced { get; set; } = [
        typeof(TestNodeUpdateMessage),
        typeof(FileArtifact),
        typeof(SessionFileArtifact),
    ];

    public string Uid { get; set; } = "5222b138-d01c-4ab6-9e17-f2782d62a425";

    public string Version { get; set; } = "1.0.0";

    public string DisplayName { get; set; } = "Data producer stub";

    public string Description { get; set; } = "";

    public Task<bool> IsEnabledAsync() => Task.FromResult(true);

    public static DataProducerStub Instance { get; } = new();
}