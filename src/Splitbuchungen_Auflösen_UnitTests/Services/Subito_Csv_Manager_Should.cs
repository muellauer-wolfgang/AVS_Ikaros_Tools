using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Splitbuchungen_Auflösen.Services.Interfaces;
using Splitbuchungen_Auflösen.Services;

namespace Splitbuchungen_Auflösen_UnitTests.Services
{
  public class Subito_Csv_Manager_Should
  {
    [Fact]
    public void Read_Csv_File()
    {
      string sutFilename = @"H:\mdc\Kunden\AVS\Software\Ikaros_Tools\daten\Abschlusstest_Tmp\IKAROS-Akten.csv";
      ISubito_Csv_Manager sut_manager = new Subito_Csv_Manager();
      bool readStatus = sut_manager.ReadFile(sutFilename);
      var allPayload = sut_manager.EnumerateRecords().ToArray();

      Assert.True(allPayload.Length > 1);
      Assert.True(readStatus);

    }
  }


} //end namespace Splitbuchungen_Auflösen_UnitTests.Services

