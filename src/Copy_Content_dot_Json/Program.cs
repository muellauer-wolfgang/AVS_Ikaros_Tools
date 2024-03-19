using System.IO;
using Copy_Content_dot_Json.Infrastructure.Interfaces;
using Copy_Content_dot_Json.Infrastructure;

namespace Copy_Content_dot_Json
{
  internal class Program
  {
    public static IConfigProvider _config = new ConfigProvider();

    static void Main(string[] args)
    {
      Console.WriteLine("Copy_Content_dot_Json -- Kopieren der zugehörigen content.json Files");
      var allFatDirectories = Directory.GetDirectories(_config.ExportPath);
      foreach (string fatDir in allFatDirectories) {
        string sourceFile = Path.Combine(fatDir.Replace(_config.ExportPath, _config.BasePath), "content.json");
        File.Copy(sourceFile, Path.Combine(fatDir, "content.json"));
        Console.WriteLine($"Dest Dir: {fatDir}");
      }

    } //end     static void Main(string[] args)

  } //end   internal class Program

} //end namespace Copy_Content_dot_Json






