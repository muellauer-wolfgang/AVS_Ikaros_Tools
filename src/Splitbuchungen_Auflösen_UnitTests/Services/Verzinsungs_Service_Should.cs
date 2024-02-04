using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Splitbuchungen_Auflösen.Models;
using Splitbuchungen_Auflösen.Services.Interfaces;
using Splitbuchungen_Auflösen.Services;
using Splitbuchungen_Auflösen.DataServices.Interfaces;
using Splitbuchungen_Auflösen.DataServices;

namespace Splitbuchungen_Auflösen_UnitTests.Services
{
  public class Verzinsungs_Service_Should
  {

    [Fact]
    public void Calculate_Zinsen_01()
    {
      IVerzinsungs_Service zinsenSvz = new Verzinsungs_Service_Lokal();
      Verzinsung zinsInfo = new Verzinsung {
        Forderungsanteil = 1, Forderungsart = "H", Zinsart = "Basis",
        ZinsenAb = DateTime.Now, Zinssatz = 5M
      };
      List<Einzelbuchung> sutZinsbuchungsliste = zinsenSvz.Calculate_Zinsen(
        4873.25M, 
        new DateTime(2016, 09, 20),
        new DateTime(2018, 04, 17),
        zinsInfo);
      Assert.NotNull(sutZinsbuchungsliste);
      Assert.Equal(1, sutZinsbuchungsliste.Count());
      Assert.True(340.0M < sutZinsbuchungsliste[0].Zinsen, "Intervall passt nicht");
      Assert.True(342.0M > sutZinsbuchungsliste[0].Zinsen, "Intervall passt nicht");
    }

    [Fact]
    public void Calculate_Zinsen_02()
    {
      IVerzinsungs_Service zinsenSvz = new Verzinsungs_Service_Lokal();
      Verzinsung zinsInfo = new Verzinsung {
        Forderungsanteil = 1, Forderungsart = "H", Zinsart = "Basis",
        ZinsenAb = DateTime.Now, Zinssatz = 5M
      };
      List<Einzelbuchung> sutZinsbuchungsliste = zinsenSvz.Calculate_Zinsen(
        4873.25M,
        new DateTime(2018, 04, 18),
        new DateTime(2018, 05, 16),
        zinsInfo);
      Assert.NotNull(sutZinsbuchungsliste);
      Assert.Equal(1, sutZinsbuchungsliste.Count());
      Assert.True(16.0M < sutZinsbuchungsliste[0].Zinsen, "Intervall passt nicht");
      Assert.True(18.0M > sutZinsbuchungsliste[0].Zinsen, "Intervall passt nicht");
    }

    [Fact]
    public void Calculate_Zinsen_03()
    {
      IVerzinsungs_Service zinsenSvz = new Verzinsungs_Service_Lokal();
      Verzinsung zinsInfo = new Verzinsung {
        Forderungsanteil = 1, Forderungsart = "H", Zinsart = "Basis",
        ZinsenAb = DateTime.Now, Zinssatz = 5M
      };
      List<Einzelbuchung> sutZinsbuchungsliste = zinsenSvz.Calculate_Zinsen(
        4873.25M,
        new DateTime(2018, 06, 19),
        new DateTime(2018, 07, 17),
        zinsInfo);
      Assert.NotNull(sutZinsbuchungsliste);
      Assert.Equal(1, sutZinsbuchungsliste.Count());
      Assert.True(16.0M < sutZinsbuchungsliste[0].Zinsen, "Intervall passt nicht");
      Assert.True(19.0M > sutZinsbuchungsliste[0].Zinsen, "Intervall passt nicht");
    }

    [Fact]
    public void Calculate_Zinsen_04()
    {
      IVerzinsungs_Service zinsenSvz = new Verzinsungs_Service_Lokal();
      Verzinsung zinsInfo = new Verzinsung {
        Forderungsanteil = 1, Forderungsart = "H", Zinsart = "Basis",
        ZinsenAb = DateTime.Now, Zinssatz = 5M
      };
      List<Einzelbuchung> sutZinsbuchungsliste = zinsenSvz.Calculate_Zinsen(
        4873.25M,
        new DateTime(2020, 05, 19),
        new DateTime(2020, 12, 21),
        zinsInfo);
      Assert.NotNull(sutZinsbuchungsliste);
      Assert.Equal(1, sutZinsbuchungsliste.Count());
      Assert.True(126.0M < sutZinsbuchungsliste[0].Zinsen, "Intervall passt nicht");
      Assert.True(129.0M > sutZinsbuchungsliste[0].Zinsen, "Intervall passt nicht");
    }


    [Fact]
    public void Calculate_Zinsen_05()
    {
      IVerzinsungs_Service zinsenSvz = new Verzinsungs_Service_Lokal();
      Verzinsung zinsInfo = new Verzinsung {
        Forderungsanteil = 1, Forderungsart = "H", Zinsart = "Basis",
        ZinsenAb = DateTime.Now, Zinssatz = 5M
      };
      List<Einzelbuchung> sutZinsbuchungsliste = zinsenSvz.Calculate_Zinsen(
        4138.48M,
        new DateTime(2022, 12, 21),
        new DateTime(2023, 01, 19),
        zinsInfo);
      decimal s = sutZinsbuchungsliste[0].Zinsen + sutZinsbuchungsliste[1].Zinsen;
      Assert.NotNull(sutZinsbuchungsliste);
      Assert.Equal(2, sutZinsbuchungsliste.Count());
      Assert.True(19.0M < sutZinsbuchungsliste[0].Zinsen + sutZinsbuchungsliste[1].Zinsen, "Intervall passt nicht");
      Assert.True(21.0M > sutZinsbuchungsliste[0].Zinsen + sutZinsbuchungsliste[1].Zinsen, "Intervall passt nicht");
    }



  } //end  public class Verzinsungs_Service_Should

} //end namespace Splitbuchungen_Auflösen_UnitTests.Services

