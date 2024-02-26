using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokumente_2_Subito.Models
{
  public class Mapping_Ikaros_Subito
  {
    public string IkarosAnr { get; set; }
    public string SubitoAnr { get; set; }
    public DateTime DatumErfassung { get; set; }
    public string Gläubiger { get; set; }
    public string Schuldner { get; set; }
    public Mapping_Ikaros_Subito() { }
    public Mapping_Ikaros_Subito(IDataReader rdr)
    {
      this.IkarosAnr = rdr["IkarosAnr"].To<string>();
      this.SubitoAnr = rdr["SubitoAnr"].To<string>() ;
      this.DatumErfassung = rdr["DatumErfassung"].To<DateTime>();
      this.Gläubiger = rdr["Glaeubiger"].To<string>();
      this.Schuldner = rdr["Schuldner"].To<string>();
    }

  }  //end  public class Mapping_Ikaros_Subito

} //end namespace Dokumente_2_Subito.Models

