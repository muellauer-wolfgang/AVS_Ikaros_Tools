using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abgleich_Ikaros_Export_Subito_Query.Infrastructure.Interfaces;

namespace Abgleich_Ikaros_Export_Subito_Query.Infrastructure
{
  public class ConfigProvider : IConfigProvider
  {
    public string BasePath => @"H:\mdc\Kunden\AVS\Daten_Ikaros\_Ikaros_Migration\Tmp";
    public string Data_File_Ikaros_Export => @"IKAROS-Import-Diverse.csv";
    public string Data_File_Subito_Query => @"SUBITO-Akten-Diverse.xlsx";
    public string Data_File_Migration_Report => @"Migrations_Report.csv";
  }

}


