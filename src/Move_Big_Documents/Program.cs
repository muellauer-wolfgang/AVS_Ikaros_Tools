using System.IO;
using Move_Big_Documents.Infrastructure.Interfaces;
using Move_Big_Documents.Infrastructure;

namespace Move_Big_Documents
{
  internal class Program
  {
    public static IConfigProvider _config = new ConfigProvider();

    static void Main(string[] args)
    {
      Console.WriteLine("Move_Big_Documents -- Verschieben von zu grossen Dokumenten");
      Queue<string> directories = new Queue<string>();
      directories.Enqueue(_config.BasePath);
      while (directories.Count > 0) {
        string currentDirectory = directories.Dequeue();
        foreach(string sd in Directory.GetDirectories(currentDirectory)) { 
          directories.Enqueue(sd);
        }
        //mich interessieren nur die Files in den attachments Directories
        if (currentDirectory.EndsWith("attachments", StringComparison.CurrentCultureIgnoreCase)) {
          string[] files = Directory.GetFiles(currentDirectory);
          foreach (var f in files) {
            if (f.Contains("Migrierte_Dokumente",StringComparison.CurrentCultureIgnoreCase) 
              || f.Contains("Forderungsaufstellung", StringComparison.CurrentCultureIgnoreCase)) {
              continue;
            } else {
              FileInfo finfo = new FileInfo(f); 
              if (finfo.Length > _config.MaxFileSize) {
                Console.WriteLine($"File zu gross: {finfo.FullName}");
                string srcPath = currentDirectory;
                string destPath = currentDirectory.Replace(_config.BasePath, _config.ExportPath);
                if (!Directory.Exists(destPath)) {
                  Directory.CreateDirectory(destPath);
                }
                string destFileNameLong = Path.Combine(destPath, finfo.Name);
                finfo.MoveTo(destFileNameLong);

                //todo: File moven und so für später aufheben
              }
            }

          }
        }
      }

    } //end     static void Main(string[] args)

  } //end   internal class Program

} //end namespace Move_Big_Documents

