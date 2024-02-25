using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dokumente_2_Subito.Models;

namespace Dokumente_2_Subito.DataServices.Interfaces
{
  public interface IIkarosDataService
  {
    IEnumerable<Ikaros_Document_Item_DTO> RetrieveAll();
  }

}
