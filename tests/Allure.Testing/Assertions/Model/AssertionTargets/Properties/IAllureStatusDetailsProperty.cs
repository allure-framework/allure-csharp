using System;
using System.Text.Json;

namespace Allure.Testing.Assertions.Model.AssertionTargets.Properties;

public interface IAllureStatusDetailsProperty : IAllureObjectProperty<AllureStatusDetails, IAllureStatusDetailsProperty>
{
    static string IAllureProperty<AllureStatusDetails, IAllureStatusDetailsProperty>.PropertyName { get; }
        = "statusDetails";

    static Func<JsonElement, AllureStatusDetails> IAllureObjectProperty<AllureStatusDetails, IAllureStatusDetailsProperty>.Factory { get; }
        = json => new(json);
}
