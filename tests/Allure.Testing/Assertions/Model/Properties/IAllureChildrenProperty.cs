using System;

namespace Allure.Testing.Assertions.Model.Properties;

[GenerateAllureAssertions(ItemMethodName = "Child")]
public interface IAllureChildrenProperty<TSelf> : IAllureArrayProperty<Guid, IUuidItemFactory, TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllureChildrenProperty<TSelf>;
