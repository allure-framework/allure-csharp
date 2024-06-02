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
var attrsPattern = new Regex(@"(?<name>\w+)=[""'](?<value>[^""']+)[""']", RegexOptions.IgnoreCase | RegexOptions.Singleline);

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
    if (match.Success)
    {
        var attrs = match.Groups["attrs"].Value;
        var url = match.Groups["url"].Value;
        var title = match.Groups["title"].Value;
        string src = default;
        string alt = default;
        string height = default;
        string width = default;
        foreach (Match attrMatch in attrsPattern.Matches(attrs))
        {
            var attrName = attrMatch.Groups["name"].Value;
            var attrValue = attrMatch.Groups["value"].Value;
            switch (attrName)
            {
                case "src":
                    src = attrValue;
                    break;
                case "alt":
                    alt = attrValue;
                    break;
                case "height":
                    height = Regex.Replace(attrValue, @"\D", "");
                    break;
                case "width":
                    width = Regex.Replace(attrValue, @"\D", "");
                    break;
            }
        }
        var imgOriginalFileName = new Uri(src).Segments.Last();
        var imgFileNameBuilder = new StringBuilder(
            Path.GetFileNameWithoutExtension(imgOriginalFileName)
        );
        IEnumerable<string> dimensions = [width, height];
        var dimSuffix = string.Join("x", dimensions.Where(v => v is not null));
        if (dimSuffix != "")
        {
            imgFileNameBuilder.Append($"_{dimSuffix}");
        }
        var imgExtension = Path.GetExtension(imgOriginalFileName).ToLower();
        imgFileNameBuilder.Append(
            imgExtension.ToLower() == ".svg" ? ".png" : imgExtension
        );
        var imgFileName = imgFileNameBuilder.ToString();
        var imgFsPath = Path.Combine(cwd, "..", "img", imgFileName);
        if (!File.Exists(imgFsPath))
        {
            Log.LogError($"Nuget README converter: {this.InputReadmePath} - {imgFileName} doesn't exist in img");
        }

        outputWriter.WriteLine(
            "[![{0}]({1} \"{2}\")]({3})",
            alt,
            $"https://raw.githubusercontent.com/allure-framework/allure-csharp/main/img/{imgFileName}",
            title,
            url
        );
    }
    else
    {
        outputWriter.WriteLine(line);
    }
}

this.GeneratedReadmePath = outputFilePath;
