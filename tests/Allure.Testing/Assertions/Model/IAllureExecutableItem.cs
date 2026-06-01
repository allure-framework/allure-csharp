using Allure.Testing.Assertions.Model.Properties;

namespace Allure.Testing.Assertions.Model;

public interface IAllureExecutableItem<TSelf> :
    IAllureModelObject<TSelf>,
    IAllureNameProperty<TSelf>,
    IAllureStartProperty<TSelf>,
    IAllureStopProperty<TSelf>,
    IAllureStepsProperty<TSelf>,
    IAllureStatusProperty<TSelf>,
    IAllureStatusDetailsProperty<TSelf>

    where TSelf : IAllureExecutableItem<TSelf>, IAllureModelObject<TSelf>
{

}
