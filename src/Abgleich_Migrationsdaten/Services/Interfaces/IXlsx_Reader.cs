using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abgleich_Migrationsdaten.Models;

namespace Abgleich_Migrationsdaten.Services.Interfaces
{
  public interface IXlsx_Reader
  {
    IEnumerable<Migrations_Report_DTO> Retrieve_Buchungen(string fileName, string worksheetName);    

  } //end   public interface IXlsx_Reader

} //end namespace Splitbuchungen_Auflösen.Services.Interfaces

