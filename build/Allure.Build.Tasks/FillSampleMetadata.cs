using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Allure.Build.Tasks.Functions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Allure.Build.Tasks;

public class FillSampleMetadata : Task
{
    [Required]
    public ITaskItem[] Samples { get; set; }

    [Required]
    public string RootDirectory { get; set; }

    [Required]
    public string ProjectDirectory { get; set; }

    [Required]
    public string SampleSolutionDirectory { get; set; }

    [Required]
    public string SampleSolutionName { get; set; }

    [Required]
    public string RootNamespace { get; set; }

    [Required]
    public string BinDirectory { get; set; }

    [Required]
    public string ObjDirectory { get; set; }

    [Required]
    public string Configuration { get; set; }

    [Required]
    public string TargetFramework { get; set; }

    [Output]
    public ITaskItem[] ResolvedSamples { get; set; }

    IEnumerable<ITaskItem2> Samples2 =>
        this.Samples.Cast<ITaskItem2>();

    public override bool Execute()
    {
        foreach (var sample in this.Samples2)
        {
            this.EnsureSampleMetadata(sample);
        }

        this.ResolvedSamples = this.Samples;
        return true;
    }

    void EnsureSampleMetadata(ITaskItem2 sample)
    {
        var fragments = this.GetSampleFqNameFragmants(sample.EvaluatedIncludeEscaped);

        if (fragments.Length == 0)
        {
            return;
        }

        if (IsMetadataMissing(sample, "SampleName"))
        {
            var name = fragments[^1];
            sample.SetMetadataValueLiteral("SampleName", name);
        }

        if (IsMetadataMissing(sample, "RegistryNamespace"))
        {
            string[] registryNamespaceParts = [this.RootNamespace, ..fragments[..^1]];
            var registryNamespace = string.Join(".", registryNamespaceParts);
            sample.SetMetadataValueLiteral("RegistryNamespace", registryNamespace);
        }

        if (IsMetadataMissing(sample, "ProjectName"))
        {
            string[] projectNameParts = [this.SampleSolutionName, ..fragments];
            var projectName = string.Join(".", projectNameParts);
            sample.SetMetadataValueLiteral("ProjectName", projectName);
        }

        if (IsMetadataMissing(sample, "ProjectFilePath"))
        {
            var projectName = sample.GetMetadataValueEscaped("ProjectName");
            var path = Path.Combine(
                this.SampleSolutionDirectory,
                projectName,
                $"{projectName}.csproj"
            );
            sample.SetMetadataValueLiteral("ProjectFilePath", path);
        }

        if (IsMetadataMissing(sample, "ProjectRelativePath"))
        {
            var path = Path.GetRelativePath(
                this.RootDirectory,
                Path.Combine(
                    this.SampleSolutionDirectory,
                    sample.GetMetadataValueEscaped("ProjectName")
                )
            );
            sample.SetMetadataValueLiteral("ProjectRelativePath", path);
        }

        if (IsMetadataMissing(sample, "ResultsDirectory"))
        {
            this.SetResultsDirectory(sample);
        }

        if (IsMetadataMissing(sample, "ProjectBinPath"))
        {
            var path = Path.Combine(
                this.BinDirectory,
                sample.GetMetadataValueEscaped("ProjectName")
            );
            sample.SetMetadataValueLiteral("ProjectBinPath", path);
        }

        if (IsMetadataMissing(sample, "ProjectObjPath"))
        {
            var path = Path.Combine(
                this.ObjDirectory,
                sample.GetMetadataValueEscaped("ProjectName")
            );
            sample.SetMetadataValueLiteral("ProjectObjPath", path);
        }
    }

    static bool IsMetadataMissing(ITaskItem2 item, string key) =>
        string.IsNullOrEmpty(item.GetMetadataValueEscaped(key));

    void SetResultsDirectory(ITaskItem2 item)
    {
        var configuration = this.Configuration.ToLowerInvariant();
        var targetFramework = this.TargetFramework.ToLowerInvariant();
        var defaultResultsDirectory = Path.Join(
            this.BinDirectory,
            item.GetMetadataValueEscaped("ProjectName"),
            $"{configuration}_{targetFramework}",
            "allure-results"
        );
        item.SetMetadataValueLiteral("ResultsDirectory", defaultResultsDirectory);
    }

    ImmutableArray<string> GetSampleFqNameFragmants(string samplePath)
    {
        var fragments = this.GetPathFragments(samplePath).Reverse().ToList();

        var samplesDirIndex = fragments.IndexOf("Samples", 0, fragments.Count - 1);

        if (samplesDirIndex == -1)
        {
            Logging.LogUnexpectedLayoutWarning(this.Log, samplePath);
            return [];
        }

        return [..fragments[..samplesDirIndex], fragments[samplesDirIndex + 1]];
    }

    IEnumerable<string> GetPathFragments(string samplePath)
    {
        yield return Path.GetFileNameWithoutExtension(samplePath);
        for (var dir = Path.GetDirectoryName(samplePath);
            IsInProjectDir(dir);
            dir = Path.GetDirectoryName(dir))
        {
            yield return Path.GetFileName(dir);
        }

        bool IsInProjectDir(string path)
            => path is not null && !this.ProjectDirectory.StartsWith(path);
    }
}
