using System.Collections.Generic;

namespace Allure.Build.Tasks.DataTypes;

public record MsBuildImportFiles(
    IEnumerable<string> PropsFiles,
    IEnumerable<string> TargetsFiles
);
