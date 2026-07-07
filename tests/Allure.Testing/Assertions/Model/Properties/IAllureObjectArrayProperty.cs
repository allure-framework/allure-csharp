namespace Allure.Testing.Assertions.Model.Properties;

public interface IAllureObjectArrayProperty<TElement, TSelf> : IAllureArrayProperty<TElement, IModelObjectItemFactory<TElement>, TSelf>
    where TElement : IAllureModelObject<TElement>
    where TSelf : IAllureModelObject<TSelf>, IAllureObjectArrayProperty<TElement, TSelf>;
