namespace FileMover.Config
{
  /// <summary>
  /// Interface for a FileMover configuration
  /// </summary>
  public interface IFileMoverConfiguration
  {
    string BaseFolder { get; }
    string GetDestination(string fileExtension);
  }
}
