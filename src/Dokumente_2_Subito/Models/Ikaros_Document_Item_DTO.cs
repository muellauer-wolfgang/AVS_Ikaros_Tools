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
    public int    IkarosAnrIndex { get; set; } 
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
      this.IkarosAnrIndex = rdr["UnterAktNr"].To<int>();
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
      //Anpassen der IkarosAnr an die Nomenklatur mit /AktIndex
      this.IkarosAnr = this.IkarosAnr + $"/{this.IkarosAnrIndex}";
    }

  } //end   public class Ikaros_Tupel_DTO

} //end namespace Dokumente_2_Subito.Models



