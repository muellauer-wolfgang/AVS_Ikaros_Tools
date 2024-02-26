using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokumente_2_Subito.Models
{
  public class TransferContainer
  {
    public string IkarosAnr { get; set; }
    public string Gläubiger { get; set; } 
    public List<Ikaros_Document_Item_DTO> IkarosItems { get; private set; } = new();

    public override string ToString()
    {
      return $"IAnr:{IkarosAnr} ItemCnt:{IkarosItems.Count}";
    }

  } //end   public class SubitoAkt

} //end namespace Dokumente_2_Subito.Models

