using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokumente_2_Subito.Models
{
  public class Ikaros_Document_Item_DTO
  {
    public string IkarosAnr { get; set; }
    public string GläubigerName { get; set; }
    public string GläubigerNotizen { get; set; }
    public string SchuldnerName { get; set; }
    public string SchuldnerNotizen { get; set; }
    public string SchuldnerLand { get; set; } 
    public DateTime? Datum { get; set; }
    public string Kürzel { get; set; }
    public string Kurztext { get; set; }
    public decimal? Betrag { get; set; }
    public string Bemerkung { get; set; }
    public string FilePath { get; set; }
    public string FileName { get; set; }
    public string FileType { get; set; }
    public string AktenNotiz { get; set; }

    public Ikaros_Document_Item_DTO() { }

    public Ikaros_Document_Item_DTO(IDataReader rdr)
    {
      this.IkarosAnr = rdr["Aktenzeichen"].To<string>();
      this.GläubigerName = rdr["Gläubiger"].To<string>();
      this.GläubigerNotizen = rdr["GläubigerNotizen"].To<string>();
      this.SchuldnerName = rdr["Schuldner"].To<string>();
      this.SchuldnerNotizen = rdr["SchuldnerNotizen"].To<string>();
      this.SchuldnerLand = rdr["SchuldnerLand"].To<string>();
      this.Datum = rdr["Datum"].To<DateTime?>();
      this.Kürzel = rdr["Kürzel"].To<string>();
      this.Kurztext = rdr["Kurztext"].To<string>() ;
      this.Betrag = rdr["Betrag"].To<decimal?>();
      this.Bemerkung = rdr["Bemerkung"].To<string>();
      this.FilePath = rdr["FilePath"].To<string>();
      this.FileName = rdr["FileName"].To<string>(); 
      this.FileType = rdr["FileType"].To<string>();
      this.AktenNotiz = rdr["AktenNotiz"].To<string>();
    }

  } //end   public class Ikaros_Tupel_DTO

} //end namespace Dokumente_2_Subito.Models

/*
SELECT  
  a.Az AS Aktenzeichen 
  ,k.Name1 AS Gläubiger
  ,k.Notizen AS GläubigerNotizen
  ,k2.Suchbegriff AS Schuldner
  ,k2.Notizen AS SchuldnerNotizen
  ,v.Datum AS Datum 
  ,vv.Kuerzel AS Kürzel
  ,v.Kurztext AS Kurztext
  ,v.Betrag AS Betrag
  ,v.Bemerkung AS Bemerkung
  ,CASE
     WHEN (SELECT d.Dok_ID FROM Akte a JOIN Vorgang v2 ON v.Akte_ID = a.Akte_ID JOIN Dokument d ON d.RefID = v.Vg_ID WHERE d.RefTabelle = 'Vorgang' AND d.Speicherungsart = 'F' AND v.Vg_ID = v2.Vg_ID) IS NOT NULL 
     THEN 'C:\ExterneDokumente\' + RIGHT((SELECT d.Dok_ID FROM Akte a JOIN Vorgang v2 ON v.Akte_ID = a.Akte_ID JOIN Dokument d ON d.RefID = v.Vg_ID WHERE d.RefTabelle = 'Vorgang' AND d.Speicherungsart = 'F' AND v.Vg_ID = v2.Vg_ID), 3) + '\'
     ELSE NULL 
   END AS Filepath
  ,(SELECT d.Dok_ID FROM Akte a JOIN Vorgang v2 ON v.Akte_ID = a.Akte_ID JOIN Dokument d ON d.RefID = v.Vg_ID WHERE d.RefTabelle = 'Vorgang' AND d.Speicherungsart = 'F' AND v.Vg_ID = v2.Vg_ID) AS Filename
  ,(SELECT d.Extension FROM Akte a JOIN Vorgang v2 ON v.Akte_ID = a.Akte_ID JOIN Dokument d ON d.RefID = v.Vg_ID WHERE d.RefTabelle = 'Vorgang' AND d.Speicherungsart = 'F' AND v.Vg_ID = v2.Vg_ID) AS Filetype
  ,a.Notizen AS AktenNotiz
FROM Akte a
  JOIN Vorgang v ON v.Akte_ID = a.Akte_ID
  JOIN VgVorlage vv ON vv.VgVorl_ID = v.VgVorl_ID
  JOIN Kontakt k ON k.Kontakt_ID = a.Mandant_ID
  JOIN Kontakt k2 ON k2.Kontakt_ID = a.Schuldner_ID
ORDER BY a.Az, v.Datum, v.lupdate;

*/



