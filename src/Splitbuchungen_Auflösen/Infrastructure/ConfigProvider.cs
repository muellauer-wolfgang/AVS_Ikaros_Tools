using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Splitbuchungen_Auflösen.Infrastructure.Interfaces;

namespace Splitbuchungen_Auflösen.Infrastructure
{
  public class ConfigProvider : IConfigProvider
  {

    /*
        public string BasePath => @"H:\mdc\Kunden\AVS\Software\Ikaros_Tools\daten\Test_4_Spike";
        public string Infile_Buchungen => @"IKAROS-Export-20160012675.xlsx";
        public string Infile_4_Subito => @"IKAROS-Import-20160012675.csv";
        public string Outfile_4_Subito => @"IKAROS-Import-20160012675_MODIFIED.csv";
    */

    //public string BasePath => @"H:\mdc\Kunden\AVS\Software\Ikaros_Tools\daten\Abschlusstest";
    //public string BasePath => @"H:\mdc\Kunden\AVS\Software\Ikaros_Tools\daten\01_Akt_Azzolina";
    //public string BasePath => @"H:\mdc\Kunden\AVS\Software\Ikaros_Tools\daten\02_Akt_Kneifeld";
    //public string BasePath => @"H:\mdc\Kunden\AVS\Software\Ikaros_Tools\daten\03_Akt_Pisorn";
    //public string BasePath => @"H:\mdc\Kunden\AVS\Software\Ikaros_Tools\daten\04_Akt_Hillers";
    //public string BasePath => @"H:\mdc\Kunden\AVS\Software\Ikaros_Tools\daten\05_Akt_Lehner";
    //public string BasePath => @"H:\mdc\Kunden\AVS\Software\Ikaros_Tools\daten\06_Akt_Gagesch";
    //public string BasePath => @"H:\mdc\Kunden\AVS\Software\Ikaros_Tools\daten\07_Akt_Tozios";

    public string BasePath => @"H:\mdc\Kunden\AVS\Software\Ikaros_Tools\daten\Import_2024_02_04";

    public string Infile_Buchungen => @"IKAROS-Export.xlsx";
    public string Infile_4_Subito => @"IKAROS-Akten.csv";
    public string Outfile_4_Subito => @"IKAROS-Akten_MODIFIED.csv";



  } //end   public class ConfigProvider : IConfigProvider

} //end namespace Splitbuchungen_Auflösen.Infrastructure

