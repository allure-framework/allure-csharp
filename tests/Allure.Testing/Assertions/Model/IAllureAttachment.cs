using Allure.Testing.Assertions.Model.Properties;

namespace Allure.Testing.Assertions.Model;

public interface IAllureAttachment<TSelf> :
    IAllureModelObject<TSelf>,
    IAllureNameProperty<TSelf>,
    IAllureAttachmentSourceProperty<TSelf>,
    IAllureLinkTypeProperty<TSelf>

    where TSelf : IAllureAttachment<TSelf>, IAllureModelObject<TSelf>;
