using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abgleich_Migrationsdaten.Infrastructure.Interfaces;

namespace Abgleich_Migrationsdaten.Infrastructure
{
  public class ConfigProvider : IConfigProvider
  {
    public string BasePath => @"H:\mdc\Kunden\AVS\Daten_Ikaros\_Ikaros_Migration";
    public string Report_File_Migration => @"Ikaros_Subito_Migrations_Report_2024_02_12.xlsx";
    public string Data_File_BHI_Akte => @"IKAROS-Import-BHI.csv";
    public string Data_File_Diverse_Akte => @"IKAROS-Import-Diverse.csv";

  } //end   public class ConfigProvider : 

} //end namespace Splitbuchungen_Auflösen.Infrastructure

