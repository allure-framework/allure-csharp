using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[assembly: InternalsVisibleTo("Allure.Net.Commons.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Allure.Net.Commons.Writer
{
  internal class FileSystemResultsWriter : IAllureResultsWriter
  {
    private readonly AllureConfiguration configuration;
    //private Logger logger = LogManager.GetCurrentClassLogger();

    private static readonly object BytesWriterLock = new object();
    private readonly string outputDirectory;
    private readonly JsonSerializer serializer = new JsonSerializer();

    internal FileSystemResultsWriter(AllureConfiguration configuration)
    {
      this.configuration = configuration;
      outputDirectory = GetResultsDirectory(configuration.Directory);

      serializer.NullValueHandling = NullValueHandling.Ignore;
      serializer.Formatting = Formatting.Indented;
      serializer.Converters.Add(new StringEnumConverter());
    }

    public void Write(TestResult testResult)
    {
      LinkHelper.UpdateLinks(testResult.links, configuration.Links);
      Write(testResult, AllureConstants.TEST_RESULT_FILE_SUFFIX);
    }

    public void Write(TestResultContainer testResult)
    {
      LinkHelper.UpdateLinks(testResult.links, configuration.Links);
      Write(testResult, AllureConstants.TEST_RESULT_CONTAINER_FILE_SUFFIX);
    }

    public void Write(string source, byte[] content)
    {
      using var task = new WriteTask { Filepath = Path.Combine(outputDirectory, source), Content = content };
      ThreadPool.QueueUserWorkItem(WriteBinary, task);
      task.WaitOne();
    }

    public void CleanUp()
    {
      using (var mutex = new Mutex(false, "729dc988-0e9c-49d0-9e50-17e0df3cd82b"))
      {
        mutex.WaitOne();
        var directory = new DirectoryInfo(outputDirectory);
        foreach (var file in directory.GetFiles()) file.Delete();
        foreach (var dir in directory.GetDirectories()) dir.Delete(true);

        mutex.ReleaseMutex();
      }
    }

    public override string ToString()
    {
      return outputDirectory;
    }

    protected string Write(object allureObject, string fileSuffix)
    {
      var filePath = Path.Combine(outputDirectory, $"{Guid.NewGuid().ToString("N")}{fileSuffix}");
      using (var fileStream = File.CreateText(filePath))
      {
        serializer.Serialize(fileStream, allureObject);
      }

      return filePath;
    }

    protected void WriteBinary(object writeTask)
    {
      if (writeTask is WriteTask task)
      {
        lock (BytesWriterLock)
        {
          using (var writer = new BinaryWriter(File.Open(task.Filepath, FileMode.Append, FileAccess.Write)))
          {
            writer.Write(task.Content);
            task.Set();
          }
        }
      }
      else
      {
        throw new ArgumentException("Argument cannot be casted from WriteTask class", nameof(writeTask));
      }
    }

    internal virtual bool HasDirectoryAccess(string directory)
    {
      var tempFile = Path.Combine(directory, Guid.NewGuid().ToString());
      try
      {
        File.WriteAllText(tempFile, string.Empty);
        File.Delete(tempFile);
        return true;
      }
      catch (UnauthorizedAccessException)
      {
        return false;
      }
    }

    private string GetResultsDirectory(string outputDirectory)
    {
      var parentDir = new DirectoryInfo(outputDirectory).Parent.FullName;
      outputDirectory = HasDirectoryAccess(parentDir)
          ? outputDirectory
          : Path.Combine(
              Path.GetTempPath(), AllureConstants.DEFAULT_RESULTS_FOLDER);

      Directory.CreateDirectory(outputDirectory);

      return new DirectoryInfo(outputDirectory).FullName;
    }

    private class WriteTask : EventWaitHandle
    {
      internal WriteTask() : base(false, EventResetMode.ManualReset) { }
      public string Filepath;
      public byte[] Content;
    }
  }
}