using System;
using Allure.Net.Commons;

namespace Allure.TestingPlatform.Properties;

public sealed class AllureDescriptionProperty<TObject>(string description) : IAllureProperty<TObject>
    where TObject : ExecutableItem
{
    public string Description { get; } = description;

    public bool Append { get; init; } = false;

    public void Apply(IAllureInfrastructure _, TObject obj)
    {
        Console.WriteLine($"{this.Append}: {obj.description}");
        if (this.Append && obj.description is { Length: > 0 })
        {
            obj.description += $"\n\n{this.Description}";
        }
        else
        {
            obj.description = this.Description;
        }
    }
}