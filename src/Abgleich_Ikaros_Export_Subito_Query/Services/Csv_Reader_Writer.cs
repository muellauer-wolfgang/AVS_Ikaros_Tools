using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using Abgleich_Ikaros_Export_Subito_Query.Infrastructure.Interfaces;
using Abgleich_Ikaros_Export_Subito_Query.Models;
using Abgleich_Ikaros_Export_Subito_Query.Services.Interfaces;

namespace Abgleich_Ikaros_Export_Subito_Query.Services
{
  public class Csv_Reader_Writer : ICsv_Reader_Writer
  {
    public Csv_Reader_Writer()
    {
      EncodingProvider provider = CodePagesEncodingProvider.Instance;
      Encoding.RegisterProvider(provider);
    }

    public void Create_Akte_File(string fileName, IEnumerable<Migrations_Report_DTO> records)
    {
      StringBuilder sb = new();
      sb.AppendLine("IkarosAnr;SubitoAnr;Auftraggeber;Gläubiger;ProcessingState");
      foreach(Migrations_Report_DTO mr in records) {
        sb.Append(mr.IkarosAnr); sb.Append(";");
        sb.Append(mr.SubitoAnr); sb.Append(";");
        sb.Append(mr.Auftraggeber); sb.Append(";");
        sb.AppendLine(";");
      }
      File.WriteAllText(fileName, sb.ToString());
    }

    public IEnumerable<Ikaros_Akt_DTO> Retrieve_Akte(string fileName)
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

} //end namespace Abgleich_Ikaros_Export_Subito_Query.Services


