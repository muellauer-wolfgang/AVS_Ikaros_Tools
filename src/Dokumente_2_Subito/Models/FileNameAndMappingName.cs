using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokumente_2_Subito.Models
{
  /// <summary>
  /// Diese Methode enthält einen FileNamen und wenn das File
  /// nicht übertragen werden kann, weil die Extension nicht geht
  /// dann steht im Alias der .txt Filename, der dann über eine
  /// Query korrigiert wird
  /// </summary>
  public class FileNameAndMappingName : IComparable<FileNameAndMappingName>
  {
    public string FileName { get; set; }
    public string Alias { get; set; }
    public FileNameAndMappingName() { }
    public FileNameAndMappingName(string fn)
    {
      FileName = fn;
      Alias= null;
    }
    public int CompareTo(FileNameAndMappingName rhs)
    {
      return string.Compare(this.FileName, rhs.FileName, StringComparison.OrdinalIgnoreCase);
    }

    public override string ToString()
    {
      return $"FN:{this.FileName} --> Alias:{this.Alias}";
    }

  } //end   public class FileNameAndMappingName : IComparable<FileNameAndMappingName>


} //end namespace Dokumente_2_Subito.Models

