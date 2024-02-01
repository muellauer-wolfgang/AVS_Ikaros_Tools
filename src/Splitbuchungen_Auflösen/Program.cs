using Autofac;
using Syncfusion.XlsIO.FormatParser.FormatTokens;
using Splitbuchungen_Auflösen.Startup;
using Splitbuchungen_Auflösen.Infrastructure.Interfaces;
using Splitbuchungen_Auflösen.Services.Interfaces;
using Splitbuchungen_Auflösen.Models;
using Splitbuchungen_Auflösen.DataServices;
namespace Splitbuchungen_Auflösen
{
  internal class Program
  {


    public static IContainer Container { get; private set; } = new Bootstrapper().Bootstrap();
    public static IConfigProvider Config { get; set; }
    static void Main(string[] args)
    {
      int counter = 0;
      Dictionary<string, AktBuchungen> buchungsDict = new();

      Config = Container.Resolve<IConfigProvider>();
      Console.WriteLine("Ikaros_Tool Splitbuchungen auflösen");
      Console.WriteLine($"BasePath: {Config.BasePath}");
      IXlsx_Reader xlsxReader = Container.Resolve<IXlsx_Reader>();

      Console.WriteLine("START LESEN XLSX");

      foreach (Akt_Einzelbuchung_DTO dto in xlsxReader.Retrieve_Buchungen(@"IKAROS-Export.xlsx")) {
        counter++;
        if (counter % 100 == 0) {
          Console.WriteLine($"Anzahl Records gelesen: {counter}");
        }
        if (!buchungsDict.ContainsKey(dto.Aktenzeichen)) {
          buchungsDict.Add(dto.Aktenzeichen, new AktBuchungen());
        }
        buchungsDict[dto.Aktenzeichen].Add_Einzelbuchung(dto);
        //Console.WriteLine(dto.ToString());
      }

      Console.WriteLine("LESEN XLSX FERTIG");
      Console.WriteLine($"Anzahl Akte in XLSX-File: {buchungsDict.Count}");

      //jetzt lesen des csv-Files

      Console.WriteLine("START LESEN CSV");
      int csvCounter = 0;
      ISubito_Csv_Manager subitoMgr = Container.Resolve<ISubito_Csv_Manager>();
      subitoMgr.ReadFile(Path.Combine(Config.BasePath, "IKAROS-Akten.csv"));
      foreach (var fieldSet in subitoMgr.EnumerateRecords()) {
        csvCounter++;
        string aktId = fieldSet[0];
        if (!string.IsNullOrEmpty(aktId)) {
          if (buchungsDict.ContainsKey(aktId)) {
            try {
              BuchungsSaldo salden = buchungsDict[aktId].SaldiereBuchungen();
              if (salden != null) {
                string hf = salden.Kosten_Hauptforderung.ToString("#0.00");
                fieldSet[43] = hf;
                string zns = salden.Kosten_Zinsen.ToString("#0.00");
                fieldSet[47] = zns;
                string kosten = salden.Kosten_Unverzinst.ToString("#0.00");
                fieldSet[50] = kosten;
              } else {
                Console.WriteLine($"Fehler beim Saldieren Akt {aktId} csvCount: {csvCounter}");

              }

            } catch (Exception e) {
              Console.WriteLine($"Exception Fehler beim Saldieren Akt {aktId} csvCount: {csvCounter}");
            }

          } else {
            Console.WriteLine($"Akt {aktId} NICHT GEFUNDEN");
          }
        }
      }
      Console.WriteLine($"Anzahl Akte in CSV-File: {csvCounter}");
      Console.WriteLine("LESEN CSV FERTIG");
      subitoMgr.WriteFile(Path.Combine(Config.BasePath, @"New_export.csv"));
      Console.WriteLine("Schreiben CSV FERTIG");


    } //end     static void Main(string[] args)


  } //end   internal class Program

} //end namespace Splitbuchungen_Auflösen

