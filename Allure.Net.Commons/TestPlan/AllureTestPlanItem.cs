#nullable enable

namespace Allure.Net.Commons.TestPlan;

public record class AllureTestPlanItem
{
    public string? Id { get; set; }

    public string? Selector { get; set; }
}
