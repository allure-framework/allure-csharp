/*
 * The source code of the TransformReadmeForNuget task defined in Directory.Build.targets.
 * The task is used by the GenerateNugetReadmeFiles target. See Directory.Build.targets
 * for some details.
 *
*/

var cwd = Environment.CurrentDirectory;
var inputBaseFileName = Path.GetFileNameWithoutExtension(this.InputReadmePath);
var inputFileExtension = Path.GetExtension(this.InputReadmePath);
var outputFileName = $"_generated_{inputBaseFileName}.nuget{inputFileExtension}";
var inputFilePath = Path.Combine(cwd, this.InputReadmePath);
var outputFilePath = Path.Combine(cwd, this.OutputDirPath, outputFileName);

var imgLinkPattern = new Regex(
    @"\[<img(?<attrs>[^>]*)>\]\((?<url>[^ ]+) ""(?<title>[^""]+)""\)",
    RegexOptions.IgnoreCase | RegexOptions.Singleline
);

using var inputReader = new StreamReader(
    inputFilePath,
    encoding: Encoding.UTF8,
    detectEncodingFromByteOrderMarks: true
);
using var outputWriter = new StreamWriter(
    outputFilePath,
    append: false,
    encoding: Encoding.UTF8
);

for (string line; (line = inputReader.ReadLine()) is not null; )
{
    var match = imgLinkPattern.Match(line);
    if (!imgLinkPattern.IsMatch(line))
    {
        outputWriter.WriteLine(line);
    }
}

this.GeneratedReadmePath = outputFilePath;
