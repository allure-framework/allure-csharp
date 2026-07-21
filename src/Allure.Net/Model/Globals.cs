using System.Collections.Generic;

namespace Allure.Model;

public sealed class Globals
{
    public List<GlobalAttachment> Attachments { get; init; } = [];

    public List<GlobalError> Errors { get; init; } = [];
}
