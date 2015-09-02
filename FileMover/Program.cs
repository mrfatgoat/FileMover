using FileMover.Config;
using Microsoft.Framework.Configuration;
using System;
using System.IO;

namespace FileMover
{
  class Program
  {
    static void Main(string[] args)
    {
      // Validate command line arguments
      if (args.Length != 2)
      {
        PrintUsage();
        return;
      }
      
      string source = args[0];
      string destination = args[1];

      if(source.Equals("/init", StringComparison.OrdinalIgnoreCase))
      {
        InitConfig(destination);
        Console.WriteLine("Configuration file created");
        return;
      }

      // Load the configuration file
      IFileMoverConfiguration config = new ConfigurationBuilder()
        .BuildFileMoverConfiguration(destination);

      // Determine if the source is a file or a folder
      bool sourceIsFile = false;
      
      if (!Directory.Exists(source))
      {
        if (!File.Exists(source))
        {
          Console.WriteLine($@"Source file or folder does not exist: ""{source}""");
          return;
        }
        else
          sourceIsFile = true;
      }

      // Move each file
      if (sourceIsFile)
      {
        MoveFile(config, source, config.BaseFolder);
      }
      else
      {
        // The source is a folder, move all the files in that folder
        string[] sourceFiles = Directory.GetFiles(source, "*", SearchOption.TopDirectoryOnly);

        foreach (string file in sourceFiles)
          MoveFile(config, file, config.BaseFolder);
      }
    }


    private static void MoveFile(IFileMoverConfiguration config, string sourceFilePath, string destFolder)
    {
      // Isolate the source file name
      string sourceFileName = Path.GetFileName(sourceFilePath);

      // Create the full destination path
      string fileExtension = Path.GetExtension(sourceFileName);
      string finalDest = config.GetDestination(fileExtension);

      if (finalDest == null)
      {
        Console.WriteLine($@"No destination configured for ""{fileExtension}"" files");
        return;
      }
      
      // Create file's the final destination path
      string destFilePath = Path.Combine(finalDest, sourceFileName);
      
      // Check if the destination file exists
      if (File.Exists(destFilePath))
      {
        Console.WriteLine($@"{sourceFileName} already exists in {destFolder}");
        return;
      }
      
      try
      {
        // Create the destination directory if it doesn't exist
        if (!Directory.Exists(finalDest))
          Directory.CreateDirectory(finalDest);

        // Move the file
        Console.WriteLine($"{sourceFileName}...)");
        File.Move(sourceFilePath, destFilePath);
      }
      catch (Exception ex)
      {
        // Some exception occurred, not sure why, don't really care.
        // Leave the file where it is.
        Console.WriteLine($"\tException moving { sourceFilePath }: { ex.Message }");
      }
    }


    /// <summary>
    /// Create a new config file at the specified location
    /// </summary>
    /// <param name="destination"></param>
    public static bool InitConfig(string destination)
    {
      if (!Directory.Exists(destination))
        return false;

      // Create the config file
      string configFullPath = Path.Combine(destination, "filemover.json");
      FileMoverConfigurationBuilderExtension.WriteDefaultConfiguration(configFullPath);

      return true;
    }


    /// <summary>
    /// Print command line usage info
    /// </summary>
    private static void PrintUsage()
    {
      Console.WriteLine();
      Console.WriteLine("FileMover usage:");
      Console.WriteLine();
      Console.WriteLine("To create a new configuration file:");
      Console.WriteLine("    FileMover.exe /init destinationFolder");
      Console.WriteLine();
      Console.WriteLine("To move files from source to destination:");
      Console.WriteLine("    FileMover.exe sourceFileOrFolder destinationFolder");
      Console.WriteLine();
    }
  }
}