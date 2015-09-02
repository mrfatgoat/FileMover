using Microsoft.Framework.Configuration;
using Newtonsoft.Json;
using System.IO;

namespace FileMover.Config
{
  public static class FileMoverConfigurationBuilderExtension
  {
    /// <summary>
    /// This IConfigurationBuilder extension is the only 
    /// way to create a FileMoverConfiguration instance.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="destFolder"></param>
    /// <returns>An initialized configuration object or <i>null</i> if no configuration was found in destFolder</returns>
    public static IFileMoverConfiguration BuildFileMoverConfiguration(this IConfigurationBuilder builder, string destFolder)
    {
      // Make sure the destination folder exists
      if (!Directory.Exists(destFolder))
        throw new DirectoryNotFoundException($"{destFolder} does not exist");

      string configFileName = "filemover.json";
      string pathToConfig = Path.Combine(destFolder, configFileName);

      // Check if a config file exists
      if (!File.Exists(pathToConfig))
        return null;
      
      builder.AddJsonFile(pathToConfig);
      return new FileMoverConfiguration(builder.Build(), destFolder);
    }


    /// <summary>
    /// Write the default configuration file to the specified path
    /// </summary>
    /// <param name="configFullPath"></param>
    public static void WriteDefaultConfiguration(string configFullPath)
    {
      // TODO: This should really be an embedded resource and not
      //       hard-coded into the program...
      var defaultConfig = new
      {
        Folders = new
        {
          Flash = "Flash",
          Pictures = "Pictures",
          SVG = "Pictures/svg",
          Video = "Video"
        },
        Destinations = new
        {
          jpg = "Folders:Pictures",
          jpeg = "Folders:Pictures",
          png = "Folders:Pictures",
          gif = "Folders:Pictures",
          swf = "Folders:Flash",
          svg = "Folders:SVG",
          avi = "Folders:Video",
          mp4 = "Folders:Video",
          flv = "Folders:Video",
          mov = "Folders:Video",
          mpeg = "Folders:Video",
          webm = "Folders:Video"
        }
      };

      string jsonConfig = JsonConvert.SerializeObject(defaultConfig, Formatting.Indented);
      File.WriteAllText(configFullPath, jsonConfig);      
    }


    /// <summary>
    /// Concrete configuration class. Private so the constructor is hidden.
    /// </summary>
    private class FileMoverConfiguration : IFileMoverConfiguration
    {
      private readonly IConfiguration config;
      private readonly string baseFolder;


      public FileMoverConfiguration(IConfiguration config, string baseFolder)
      {
        this.config = config;
        this.baseFolder = baseFolder;
      }


      public string BaseFolder
      {
        get { return this.baseFolder; }
      }

      
      public string GetDestination(string fileExtension)
      {
        string destPointer = this.config.Get($"Destinations:{fileExtension.Replace(".", "").ToLower()}");
        if (destPointer == null)
          return null;

        return this.GenerateFullPath(this.config.Get(destPointer));
      }


      private string GenerateFullPath(params string[] pathParts)
      {
        return Path.Combine(this.baseFolder, Path.Combine(pathParts));
      }
    }
  }
}