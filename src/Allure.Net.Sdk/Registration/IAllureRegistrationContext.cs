using System;
using Allure.Abstractions;

namespace Allure.Sdk.Registration;

public interface IAllureRegistrationContext
{
    void UseParameterSerializer(Func<IAllureParameterSerializer> serializerFactory);
}
