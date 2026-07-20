using System.Collections.Generic;

namespace Allure.Model;

public sealed class Globals
{
    public List<GlobalAttachment> Attachments { get; init; } = [];

    public List<GlobalAttachment> Errors { get; init; } = [];
}
