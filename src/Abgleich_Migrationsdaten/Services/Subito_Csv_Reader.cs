using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using Abgleich_Migrationsdaten.Infrastructure.Interfaces;
using Abgleich_Migrationsdaten.Models;
using Abgleich_Migrationsdaten.Services.Interfaces;

namespace Abgleich_Migrationsdaten.Services
{
  public class Subito_Csv_Reader : ICsv_Reader
  {

    public Subito_Csv_Reader()
    {
      EncodingProvider provider = CodePagesEncodingProvider.Instance;
      Encoding.RegisterProvider(provider);
    }


    public IEnumerable<Ikaros_Akt_DTO> Retrieve_Buchungen(string fileName)
    {
      if (string.IsNullOrEmpty(fileName)) { yield break; }
      if (!File.Exists(fileName)) { yield break; };
      string[] lines = File.ReadAllLines(fileName, Encoding.GetEncoding(1252));
      if (lines.Length < 2) { yield break; }
      for (int i = 1; i < lines.Length; i++) {
        string[] payload = lines[i].Split(new char[] { ';' }, StringSplitOptions.None);
        Ikaros_Akt_DTO dto = new();
        dto.IkarosAnr = payload[0];
        dto.Auftraggeber = payload[1];
        yield return dto;
      }
    }
  }

} //end namespace Splitbuchungen_Auflösen.Services

