using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokumente_2_Subito.Models
{
  public class SubitoAkt
  {
    public string SubitoAnr { get;  set; }
    public Gläubiger AktGläubiger { get; set; } 
    public List<Ikaros_Document_Item_DTO> IkarosItems { get; private set; } = new();

  } //end   public class SubitoAkt

} //end namespace Dokumente_2_Subito.Models

