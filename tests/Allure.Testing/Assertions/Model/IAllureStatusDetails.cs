using Allure.Testing.Assertions.Model.Properties;

namespace Allure.Testing.Assertions.Model;

public interface IAllureStatusDetails<TSelf> :
    IAllureModelObject<TSelf>,
    IAllureFlakyProperty<TSelf>,
    IAllureKnownProperty<TSelf>,
    IAllureMessageProperty<TSelf>,
    IAllureMutedProperty<TSelf>,
    IAllureTraceProperty<TSelf>

    where TSelf : IAllureStatusDetails<TSelf>, IAllureModelObject<TSelf>;
