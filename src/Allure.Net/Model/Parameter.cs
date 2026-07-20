namespace Allure.Model;

public sealed class Parameter
{
    required public string Name { get; set; }

    required public string Value { get; set; }

    public ParameterMode? Mode { get; set; }

    public bool Excluded { get; set; }
}