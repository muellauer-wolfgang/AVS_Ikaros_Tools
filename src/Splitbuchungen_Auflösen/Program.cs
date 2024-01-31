using Autofac;
using Splitbuchungen_Auflösen.Startup;
using Splitbuchungen_Auflösen.Infrastructure.Interfaces;
using Splitbuchungen_Auflösen.Services.Interfaces;
using Splitbuchungen_Auflösen.Models;

namespace Splitbuchungen_Auflösen
{
  internal class Program
  {


    public static IContainer Container { get; private set; } = new Bootstrapper().Bootstrap();
    public static IConfigProvider Config { get; set; }
    static void Main(string[] args)
    {

      DateTime bis = new DateTime(2018, 4, 17);
      DateTime von = new DateTime(2016, 9, 20);
      TimeSpan diffInDays = bis - von;
      decimal zinstage = new decimal( diffInDays.TotalDays) + 1M;
      decimal dcimalZinsen = 4873.35M * 4.38M * zinstage  / (100.0M *  365.0M);

      double zinsen = 4873.35 * 4.38 * (diffInDays.TotalDays + 1)  / (100.0*  365.0) ;
      Console.WriteLine($"Zinsen: {zinsen}");

      Dictionary<string, AktBuchungen> buchungsDict = new();

      Config = Container.Resolve<IConfigProvider>();
      Console.WriteLine("Ikaros_Tool Splitbuchungen auflösen");
      Console.WriteLine($"BasePath: {Config.BasePath}");
      IXlsx_Reader xlsxReader = Container.Resolve<IXlsx_Reader>();

      foreach(Akt_Einzelbuchung_DTO dto in xlsxReader.Retrieve_Buchungen(@"IKAROS-Export-20160012675.xlsx")) {
        if (!buchungsDict.ContainsKey(dto.Aktenzeichen)) {
          buchungsDict.Add(dto.Aktenzeichen, new AktBuchungen() );
        }
        buchungsDict[dto.Aktenzeichen].Add_Einzelbuchung(dto);
        Console.WriteLine(dto.ToString());
      }
      Console.WriteLine("LESEN FERTIG");
      Console.WriteLine($"Anzahl Akte: {buchungsDict.Count}");
      foreach(AktBuchungen ab in buchungsDict.Values) {
        Console.WriteLine($"Saldo: {ab.SaldiereBuchungen()}");
      }


    } //end     static void Main(string[] args)


  } //end   internal class Program

} //end namespace Splitbuchungen_Auflösen

