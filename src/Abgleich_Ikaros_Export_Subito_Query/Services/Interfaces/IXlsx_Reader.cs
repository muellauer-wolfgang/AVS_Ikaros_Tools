using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abgleich_Ikaros_Export_Subito_Query.Models;

namespace Abgleich_Ikaros_Export_Subito_Query.Services.Interfaces
{
  public interface IXlsx_Reader
  {
    IEnumerable<Subito_Akt_DTO> Retrieve_Akte(string fileName);
  }

} //end namespace Abgleich_Ikaros_Export_Subito_Query.Services.Interfaces

