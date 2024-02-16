using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abgleich_Ikaros_Export_Subito_Query.Models;

namespace Abgleich_Ikaros_Export_Subito_Query.Services.Interfaces
{
  public interface ICsv_Reader_Writer
  {
    void Create_Akte_File(string fileName, IEnumerable<Migrations_Report_DTO> records);
    IEnumerable<Ikaros_Akt_DTO> Retrieve_Akte(string fileName);

  }
} //end namespace Abgleich_Ikaros_Export_Subito_Query.Services.Interfaces


