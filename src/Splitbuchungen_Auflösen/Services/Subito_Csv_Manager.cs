using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Splitbuchungen_Auflösen.Infrastructure.Interfaces;
using Splitbuchungen_Auflösen.Services.Interfaces;

namespace Splitbuchungen_Auflösen.Services
{
  public class Subito_Csv_Manager : ISubito_Csv_Manager
  {
    private int _fieldCount;
    private string[] _header;
    private IDictionary<string, int> _columnMapping = new Dictionary<string, int>();
    private IList<string[]> _content { get; set; } = new List<string[]>();

    public Subito_Csv_Manager()
    {
      EncodingProvider provider = CodePagesEncodingProvider.Instance;
      Encoding.RegisterProvider(provider);
    }

    public int Find_Column_by_Name(string name)
    {
      if (_columnMapping == null) { 
        throw new InvalidDataException("CSV-ColumnMapping dict fehlt"); 
      }
      if (!_columnMapping.ContainsKey(name)) { 
        throw new InvalidDataException($"CSV-ColumnMapping Column {name} not Found"); 
      }
      return _columnMapping[name];
    }

    void Init_Column_Mapping(string[] lines)
    {
      for (int i = 0; i < lines.Length; i++) {
        _columnMapping[lines[i]] = i;
      }
    }

    /// <summary>
    /// Hier lese ich ein csv-File, stopfe die erste Zeile als Header in 
    /// das array für den Header und die restilichen Zeilen als gesplittetes
    /// Arry in eine Liste,
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public bool ReadFile(string filename)
    {
      if (string.IsNullOrEmpty(filename)) { return false; }
      if (!File.Exists(filename)) { return false; };
      string[] lines = File.ReadAllLines(filename, Encoding.GetEncoding(1252));
      if (lines.Length < 2) { return false; }
      _header = lines[0].Split(new char[] { ';' }, StringSplitOptions.None);
      _fieldCount = _header.Length;
      this.Init_Column_Mapping(_header);  
      for (int i = 1; i < lines.Length; i++) {
        string[] payload = lines[i].Split(new char[] { ';' }, StringSplitOptions.None);
        if (payload.Length < _fieldCount) {
          return false;
        } else {
          _content.Add(payload);
        }
      }
      return true;
    }

    public IEnumerable<string[]> EnumerateRecords()
    {
      return _content;
    }

    public bool WriteFile(string filename)
    {
      if (string.IsNullOrEmpty(filename)) { return false; }
      File.WriteAllLines(filename, BuildCsvRecords(), Encoding.GetEncoding(1252));
      return true;
    }

    private IEnumerable<string> BuildCsvRecords()
    {
      //if (_header.Length > 0) { yield break; }
      string h = string.Join(";", _header);
      yield return h;
      foreach (var allFields in _content) {
        string dataLine = string.Join(";", allFields);
        yield return dataLine;
      }
    }

  }

} //end namespace Splitbuchungen_Auflösen.Services

