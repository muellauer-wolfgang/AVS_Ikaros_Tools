using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abgleich_Migrationsdaten.Models;


namespace Abgleich_Migrationsdaten.Services.Interfaces
{
  public interface ICsv_Reader
  {
    IEnumerable<Ikaros_Akt_DTO> Retrieve_Buchungen(string fileName);

  }

} //end namespace Splitbuchungen_Auflösen.Services.Interfaces

