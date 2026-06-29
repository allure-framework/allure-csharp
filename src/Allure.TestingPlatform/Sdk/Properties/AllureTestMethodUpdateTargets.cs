using System;

namespace Allure.TestingPlatform.Sdk.Properties;

[Flags]
public enum AllureTestMethodUpdateTargets
{
    FullName = 0x01 << 0,
    TitlePath = 0x01 << 1,
    Labels = 0x01 << 2,
    Parameters = 0x01 << 3,
    ApiAttributes = 0x01 << 4,

    All = FullName | TitlePath | Labels | Parameters | ApiAttributes,
}
