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
    public int Id { get; set; }
    public string IkarosAnr { get; set; }
    public string SubitoAnr { get; set; }
    public string Auftraggeber { get; set; }
    public string Gläubiger { get; set; }
    public string ProcessingState { get; set; }
    public Mapping_Ikaros_Subito() { }
    public Mapping_Ikaros_Subito(IDataReader rdr)
    {
      this.Id = rdr["Id"].To<int>();
      this.IkarosAnr = rdr["IkarosAnr"].To<string>();
      this.SubitoAnr = rdr["SubitoAnr"].To<string>() ;
      this.Auftraggeber = rdr["Auftraggeber"].To<string>();
      this.Gläubiger = rdr["Gläubiger"].To<string>();
      this.ProcessingState = rdr["ProcessingState"].To<string>();
    }

  }  //end  public class Mapping_Ikaros_Subito

} //end namespace Dokumente_2_Subito.Models

