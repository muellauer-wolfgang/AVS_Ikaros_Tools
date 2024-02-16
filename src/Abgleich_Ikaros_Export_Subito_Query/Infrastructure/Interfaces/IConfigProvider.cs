using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abgleich_Ikaros_Export_Subito_Query.Infrastructure.Interfaces
{
  public interface IConfigProvider
  {
    string BasePath { get; }
    string Data_File_Ikaros_Export { get; }
    string Data_File_Subito_Query { get; }
    string Data_File_Migration_Report { get; }

  }

}
