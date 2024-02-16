using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abgleich_Migrationsdaten.Infrastructure.Interfaces
{
  public interface IConfigProvider
  {
    string BasePath { get; }
    string Report_File_Migration {  get; }
    string Data_File_BHI_Akte { get; } 
    string Data_File_Diverse_Akte { get; }

  }

} //end namespace Splitbuchungen_Auflösen.Infrastructure.Interfaces

