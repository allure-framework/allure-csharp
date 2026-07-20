using System;

namespace Allure.Abstractions;

public abstract class AllureOperationAttribute(string? name) :
    Attribute,
    IAllureNameSource
{
    public string? Name => name;
}
