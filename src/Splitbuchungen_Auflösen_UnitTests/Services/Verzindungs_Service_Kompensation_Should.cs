using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Splitbuchungen_Auflösen.Models;
using Splitbuchungen_Auflösen.Services.Interfaces;
using Splitbuchungen_Auflösen.Services;

namespace Splitbuchungen_Auflösen_UnitTests.Services
{
  public class Verzindungs_Service_Kompensation_Should
  {

    [Fact]
    public void Calculate_Kompensation_01()
    {
      //Arrange
      List<(DateTime von, DateTime bis, int ergebnisSoll)> sut_testdaten = new List<(DateTime von, DateTime bis, int ergebnisSoll)> {
        new (new DateTime(2016, 09, 19), new DateTime(2018, 04, 17), -24),
        new (new DateTime(2018, 04, 18), new DateTime(2018, 05, 16), 0),
        new (new DateTime(2018, 05, 17), new DateTime(2018, 06, 18), -1),
        new (new DateTime(2018, 06, 19), new DateTime(2018, 07, 17), 0),
        new (new DateTime(2018, 07, 18), new DateTime(2018, 08, 17), -1),
        new (new DateTime(2018, 08, 18), new DateTime(2018, 09, 18), -1),
        new (new DateTime(2018, 09, 19), new DateTime(2018, 10, 16), 0),
        new (new DateTime(2018, 10, 17), new DateTime(2018, 11, 16), -1),
        new (new DateTime(2018, 11, 17), new DateTime(2018, 12, 18), 0),
        new (new DateTime(2018, 12, 19), new DateTime(2019, 01, 16), -1),
        new (new DateTime(2019, 01, 17), new DateTime(2019, 02, 18), -1)
      };
      int sut_KompentsationsTage;

      Type verzinserType = typeof(Verzinsungs_Service);
      object verzinserInstance = Activator.CreateInstance(verzinserType);
      MethodInfo verzinserMethod = verzinserType.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
        .Where(x => x.Name == "Calculate_Kompensationstage" && x.IsPrivate)
        .First();
      for (int i = 0; i < sut_testdaten.Count; i++) {
        sut_KompentsationsTage = (int)verzinserMethod.Invoke(
          verzinserInstance,
          new object[] { sut_testdaten[i].von, sut_testdaten[i].bis });
        Assert.True( sut_testdaten[i].ergebnisSoll == sut_KompentsationsTage, $"Fehler bei sut_testdaten Index{i}");
      }
    }

  } //end   public class Verbindungs_Service_Compensation_Should

} //end namespace Splitbuchungen_Auflösen_UnitTests.Services


