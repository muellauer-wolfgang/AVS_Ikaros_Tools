using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Autofac;

using Dokumente_2_Subito.Infrastructure.Interfaces;

namespace Dokumente_2_Subito.Models
{
  public class TransferContainer
  {
    private IConfigProvider _cfg;
    private static int _directoryCurrentIndex { get; set; } = 0;
    private static int _fileFoundCount = 0;
    private static int _fileNotFoundCount = 0;
    private static Dictionary<string, int> _fileTypeHistogramm = new Dictionary<string, int>();
    private static HashSet<string> _acceptedExtensions = 
      new HashSet<string>() {"doc", "docx", "pdf", "txt", "xlsx" };
    private static List<string> _fileRenameSqlStatements = new List<string>();
    private static List<string> _fileChangeDataSqlStatements = new List<string>();
    private static Regex _rxIkarosDocSubDir = new Regex(@"^\D*(?<SUBDIR>\d+)\D*$");

    public string IkarosAnr { get; set; }
    public string Gläubiger { get; set; }
    public List<Ikaros_Document_Item_DTO> IkarosItemList { get; private set; } = new();
    public List<Notiz> Notizen { get; private set; } = new();
    public SortedSet<string> FileNameLongList { get; private set; } = new();

    public TransferContainer(string ikarosAnr, IConfigProvider cfg)
    {
      IkarosAnr = ikarosAnr;
      _cfg = cfg;
    }

    public override string ToString()
    {
      return $"IAnr:{IkarosAnr} ItemCnt:{IkarosItemList.Count}";
    }

    /// <summary>
    /// Diese Methode sammelt alle Daten für den Import als Dokumente
    /// </summary>
    public void PrepareIkarosItemsForMigration(string subitoAnr)
    {
      HashSet<string> aktNotizenSchonVorhanden = new();
      HashSet<string> gläubigerNotizenSchonVorhanden = new();
      HashSet<string> schuldnerNotizenSchonVorhanden = new(); 
      HashSet<string> bemerkungNotizenSchonVorhanden = new();

      foreach (Ikaros_Document_Item_DTO idto in IkarosItemList) { 
        if (idto.Kurztext.StartsWith("Zahlungsaufforderung")) {
          Debug.WriteLine("Trigger");
        }
        //Bearbeiten Aktennotizen

        if (!string.IsNullOrEmpty(idto.AktenNotiz)
          && (!aktNotizenSchonVorhanden.Contains(idto.AktenNotiz))) {
          aktNotizenSchonVorhanden.Add(idto.AktenNotiz);
          Notiz n = new Notiz {
            Datum = idto.Datum, Art = NotizArt.Akt, Kürzel = idto.Kürzel, Kurztext = idto.Kurztext,
            Text = idto.AktenNotiz, FileName = ExtractFileName(idto)
          };
          Notizen.Add(n);
        }

        //Extrahieren von Gläubiger Notizen
        if (!string.IsNullOrEmpty(idto.GläubigerNotizen)
          && (!gläubigerNotizenSchonVorhanden.Contains(idto.GläubigerNotizen))) {
          gläubigerNotizenSchonVorhanden.Add(idto.GläubigerNotizen);
          Notiz n = new Notiz {
            Datum = idto.Datum, Art = NotizArt.Gläubiger, Kürzel = idto.Kürzel, Kurztext = idto.Kurztext,
            Text = idto.GläubigerNotizen, FileName = ExtractFileName(idto)
          };
          Notizen.Add(n);
        }

        //Extrahieren von SchuldnerNotizen
        if (!string.IsNullOrEmpty(idto.SchuldnerNotizen) &&
          (!schuldnerNotizenSchonVorhanden.Contains(idto.SchuldnerNotizen))) {
          schuldnerNotizenSchonVorhanden.Add(idto.SchuldnerNotizen);
          Notiz n = new Notiz {
            Datum = idto.Datum, Art = NotizArt.Schuldner, Kürzel = idto.Kürzel, Kurztext = idto.Kurztext,
            Text = idto.SchuldnerNotizen, FileName = ExtractFileName(idto)
          };
          Notizen.Add(n);
        }

        //Extrahieren von Bemerkungen
        if (!string.IsNullOrEmpty(idto.Bemerkung)
          && (!bemerkungNotizenSchonVorhanden.Contains(idto.Bemerkung))) {
          bemerkungNotizenSchonVorhanden.Add(idto.Bemerkung);
          Notiz n = new Notiz {
            Datum = idto.Datum, Art = NotizArt.Bemerkung, Kürzel = idto.Kürzel, Kurztext = idto.Kurztext,
            Text = idto.Bemerkung, FileName = ExtractFileName(idto)
          };
          Notizen.Add(n);
        }
        //Extrahieren von Files
        if (!string.IsNullOrEmpty(idto.FilePath)
          && _rxIkarosDocSubDir.IsMatch(idto.FilePath)
          && !string.IsNullOrEmpty(idto.FileName)
          && !string.IsNullOrEmpty(idto.FileType)) {
          idto.FileType = idto.FileType.ToLower();
          Match match = _rxIkarosDocSubDir.Match(idto.FilePath);
          string importFileName = Path.Combine(
            _cfg.IkarosDocumentPath,
            match.Groups["SUBDIR"].Value,
            (idto.FileName + idto.FileType));
          if (!File.Exists(importFileName)) {
            _fileNotFoundCount++;
            Console.WriteLine($"File {importFileName} NOT FOUND!!!");
          } else {
            if (!FileNameLongList.Contains(importFileName)) {
              FileNameLongList.Add(importFileName);
              _fileFoundCount++;
              if (!_fileTypeHistogramm.ContainsKey(idto.FileType)) {
                _fileTypeHistogramm[idto.FileType] = 0;
              }
              _fileTypeHistogramm[idto.FileType]++;
              FileInfo fi = new FileInfo(importFileName);
              Notiz n = new Notiz {
                Datum = idto.Datum, Art = NotizArt.Datei, Kürzel = idto.Kürzel, Kurztext = idto.Kurztext,
                Text = fi.Name, FileName = ExtractFileName(idto)
              };
              Notizen.Add(n);

            }
          }
        }

      }
    }

    /// <summary>
    /// Diese Methode erstellt jetzt alle Directories und Files für die 
    /// Migration.     /// </summary>
    public void MigrateIkarosItems(string subitoAnr)
    {
      string exportBaseDirectory = _cfg.ExportPath;
      string tempDirName = $"{(_directoryCurrentIndex++):0000000000}";  
      string exportDirectory = Path.Combine(exportBaseDirectory, tempDirName);
      if (!Directory.Exists(exportBaseDirectory)) {
        Directory.CreateDirectory(exportBaseDirectory);
      }
      if (!Directory.Exists(exportDirectory)) {
        Directory.CreateDirectory(exportDirectory);
      }
      //jetzt muss ich das content.json File erstellen
      StringBuilder sbJsonFile = new();
      sbJsonFile.AppendLine("{");
      sbJsonFile.AppendLine($"\"subject\": \"{subitoAnr}\",");
      sbJsonFile.AppendLine($"\"body\": \"Import aller Dokumente aus IKAROS\",");
      sbJsonFile.AppendLine($"\"from\": \"Ikaros_Import\",");
      sbJsonFile.AppendLine($"\"recievedTime\": \"{DateTime.Now:yyyy-MM-dd HH:mm:ss}\"");
      sbJsonFile.AppendLine("}");
      File.WriteAllText(Path.Combine(exportDirectory, "content.json"), sbJsonFile.ToString());


      //am Schluss die Files
      string attachmentDir = Path.Combine(exportBaseDirectory, tempDirName, "attachments");
      if (!Directory.Exists(attachmentDir)) {
        Directory.CreateDirectory(attachmentDir);
      }

      this.Write_Notes_to_File(attachmentDir, subitoAnr);

      if (!Directory.Exists(attachmentDir)) { Directory.CreateDirectory(attachmentDir); }
      foreach (string srcFn in this.FileNameLongList) {
        FileInfo fi = new FileInfo(srcFn);
        File.Copy(srcFn, Path.Combine(attachmentDir, fi.Name), true);
      }
    }

    private string ExtractFileName(Ikaros_Document_Item_DTO idto)
    {
      if (!string.IsNullOrEmpty(idto.FilePath)
        && _rxIkarosDocSubDir.IsMatch(idto.FilePath)
        && !string.IsNullOrEmpty(idto.FileName)
        && !string.IsNullOrEmpty(idto.FileType)) {
        idto.FileType = idto.FileType.ToLower();
        Match match = _rxIkarosDocSubDir.Match(idto.FilePath);
        string importFileName = Path.Combine(
          _cfg.IkarosDocumentPath,
          match.Groups["SUBDIR"].Value,
          (idto.FileName + idto.FileType));
        if (!File.Exists(importFileName)) {
          _fileNotFoundCount++;
          return string.Empty;
        } else {
          if (!FileNameLongList.Contains(importFileName)) {
            FileNameLongList.Add(importFileName);
            _fileFoundCount++;
            if (!_fileTypeHistogramm.ContainsKey(idto.FileType)) {
              _fileTypeHistogramm[idto.FileType] = 0;
            }
            _fileTypeHistogramm[idto.FileType]++;
          }
          FileInfo fi = new FileInfo(importFileName);
          return fi.Name;
        }
      } else {
        return string.Empty;
      }
    }

    private void Write_Notes_to_File(string attachmentPath, string subitoAnr)
    {
      string htmlTemplate = File.ReadAllText("Template_Notizen.html");
      string varTitel = $"{this.IkarosAnr}";
      string varÜberschirft = $"Notizen Dokumente IkarosAnr: {this.IkarosAnr} --> SubitoAnr {subitoAnr}";


      string varÜsNotizenAkt = $"Aktnotizen [{Notizen.Count()}]";
      StringBuilder varNotizenAkt = new();
      foreach(Notiz n in Notizen) { varNotizenAkt.AppendLine(n.AsHtmlTableRow());  }

      htmlTemplate = htmlTemplate.Replace("<!--Titel-->", varTitel);
      htmlTemplate = htmlTemplate.Replace("<!--Überschrift-->", varÜberschirft);
      htmlTemplate = htmlTemplate.Replace("<!--ÜsNotizenAkt-->", varÜsNotizenAkt);

      htmlTemplate = htmlTemplate.Replace("<!--NotizenAkt-->", varNotizenAkt.ToString());

      string ikNr = this.IkarosAnr.Replace('/', '_');
      File.WriteAllText(Path.Combine(attachmentPath, $"Migrierte_Dokumente_{ikNr}.html"), htmlTemplate);
    }

    public int CalcMaxLenGläubigerNotizen()
    {
      int maxLen = 0;
      foreach (Ikaros_Document_Item_DTO idto in IkarosItemList) {
        if (!string.IsNullOrEmpty(idto.GläubigerNotizen)) {
          int l = idto.GläubigerNotizen.Length;
          maxLen = Math.Max(maxLen, l);
        }
      }
      return maxLen;
    }

    public int CalcMaxLenSchuldnerNotzen()
    {
      int maxLen = 0;
      foreach (Ikaros_Document_Item_DTO idto in IkarosItemList) {
        if (!string.IsNullOrEmpty(idto.SchuldnerNotizen)) {
          int l = idto.SchuldnerNotizen.Length;
          maxLen = Math.Max(maxLen, l);
        }
      }
      return maxLen;
    }

    public int CalcMaxLenBemerkung()
    {
      int maxLen = 0;
      foreach (Ikaros_Document_Item_DTO idto in IkarosItemList) {
        if (!string.IsNullOrEmpty(idto.Bemerkung)) {
          int l = idto.Bemerkung.Length;
          maxLen = Math.Max(maxLen, l);
        }
      }
      return maxLen;
    }

    public int CalcMaxLenAktenNotiz()
    {
      int maxLen = 0;
      foreach (Ikaros_Document_Item_DTO idto in IkarosItemList) {
        if (!string.IsNullOrEmpty(idto.AktenNotiz)) {
          int l = idto.AktenNotiz.Length;
          maxLen = Math.Max(maxLen, l);
        }
      }
      return maxLen;
    }

    public static Dictionary<string, int> GetFileTypeHistogramm() => _fileTypeHistogramm;
    public static int GetFileFoundCount() => _fileFoundCount;
    public static int GetFileNotFoundCount() => _fileNotFoundCount;

  } //end   public class SubitoAkt

} //end namespace Dokumente_2_Subito.Models

