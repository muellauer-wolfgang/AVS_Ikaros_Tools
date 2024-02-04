using Autofac;
using Syncfusion.XlsIO.FormatParser.FormatTokens;
using Splitbuchungen_Auflösen.Startup;
using Splitbuchungen_Auflösen.Infrastructure.Interfaces;
using Splitbuchungen_Auflösen.Services.Interfaces;
using Splitbuchungen_Auflösen.Models;
using Splitbuchungen_Auflösen.DataServices;
using System.Text;
using System.Diagnostics;
namespace Splitbuchungen_Auflösen
{
  internal class Program
  {


    public static IContainer _container { get; private set; } = new Bootstrapper().Bootstrap();
    public static IConfigProvider _config { get; set; }
    public static StringBuilder _errorMessages { get; set; } = new StringBuilder();
    public static StringBuilder _aktiveAkten { get; set; } = new StringBuilder();
    static void Main(string[] args)
    {
      int counter = 0;
      Dictionary<string, AktBuchungen> buchungsDict = new();

      _config = _container.Resolve<IConfigProvider>();
      Console.WriteLine("Ikaros_Tool Splitbuchungen auflösen");
      Console.WriteLine($"BasePath: {_config.BasePath}");
      IXlsx_Reader xlsxReader = _container.Resolve<IXlsx_Reader>();

      Console.WriteLine($"START LESEN XLSX-Buchungen {_config.Infile_Buchungen}");

      foreach (Einzelbuchung_DTO dto in xlsxReader.Retrieve_Buchungen(
        Path.Combine(_config.BasePath, _config.Infile_Buchungen))) {
        counter++;
        if (counter % 100 == 0) {
          Console.WriteLine($"Anzahl Records gelesen: {counter}");
        }
        if (!buchungsDict.ContainsKey(dto.Aktenzeichen)) {
          buchungsDict.Add(dto.Aktenzeichen, new AktBuchungen(_container.Resolve<IVerzinsungs_Service>()));
        }
        buchungsDict[dto.Aktenzeichen].Add_Einzelbuchung(dto);
        //Console.WriteLine(dto.ToString());
      }

      Console.WriteLine("LESEN XLSX-Buchungen FERTIG");
      Console.WriteLine($"Anzahl Akte in XLSX-File: {buchungsDict.Count}");

      //jetzt lesen des csv-Files

      Console.WriteLine($"START LESEN CSV-SUBITO {_config.Infile_4_Subito}");
      int csvCounter = 0;
      ISubito_Csv_Manager subitoMgr = _container.Resolve<ISubito_Csv_Manager>();
      subitoMgr.ReadFile(Path.Combine(_config.BasePath, _config.Infile_4_Subito));
      foreach (var fieldSet in subitoMgr.EnumerateRecords()) {
        csvCounter++;
        if (csvCounter % 100 == 0) {
          Console.WriteLine($"Processing Line {csvCounter}");
        }
        string aktId = fieldSet[subitoMgr.Find_Column_by_Name("Stamm-ID")];    // 0
        if (!string.IsNullOrEmpty(aktId)) {

          if (!string.IsNullOrEmpty(aktId) && aktId.Equals("2023000326")) {
            Debug.WriteLine("Trigger");

          }



          //Kosten darf ich nur auf Haupt-Akte buchen, das sind Akte, wo im Feld
          //Schuldnernummer 0 steht. Auf den nicht 0 nummern sind die Koste nimmer 0
          if (fieldSet[subitoMgr.Find_Column_by_Name("Schuldnernummer")] !="0") {
            fieldSet[subitoMgr.Find_Column_by_Name("Hauptforderung")] = "0,00";
            fieldSet[subitoMgr.Find_Column_by_Name("Zinsen")] = "0,00";
            fieldSet[subitoMgr.Find_Column_by_Name("Kosten")] = "0,00";
            fieldSet[subitoMgr.Find_Column_by_Name("Kosten 2 Betrag")] = "0,00";
            fieldSet[subitoMgr.Find_Column_by_Name("Zinssatz ab")] = DateTime.Now.ToString("dd.MM.yyyy");
            fieldSet[subitoMgr.Find_Column_by_Name("Kostenzins ab")] = DateTime.Now.ToString("dd.MM.yyyy");
            fieldSet[subitoMgr.Find_Column_by_Name("Verrechnung ab")] = DateTime.Now.ToString("dd.MM.yyyy");
            fieldSet[subitoMgr.Find_Column_by_Name("Valuta")] = DateTime.Now.ToString("dd.MM.yyyy");
            fieldSet[subitoMgr.Find_Column_by_Name("Abweichendes Übergabedatum")] = DateTime.Now.ToString("dd.MM.yyyy");
          } else {
            if (buchungsDict.ContainsKey(aktId)) {
              try {
                AktBuchungen b = buchungsDict[aktId];
                BuchungsSaldo salden = buchungsDict[aktId].SaldiereBuchungen();
                if (salden.Letzte_Zahlung_Am > DateTime.Now.AddDays(-90)) {
                  string infoMessage = $"Aktiver Akt ID:{aktId} letzte Zahlung: {salden.Letzte_Zahlung_Am:yyyy-MM-dd}";
                  Console.WriteLine(infoMessage);
                  _aktiveAkten.AppendLine(infoMessage);
                }
                if (salden != null) {
                  //ich muss noch kontrollieren, ob ein Saldenpost negativ ist
                  //dann schreibe ich das in ein File
                  if (salden.Zinsen < decimal.Zero
                      || salden.Zinsen < decimal.Zero
                      || salden.Kosten_Unverzinslich < decimal.Zero) {
                    string errorMessage = $"NEGATIVE KOSTEN BEI AKT: {aktId} HF:{salden.Zinsen} ZI:{salden.Zinsen} KU:{salden.Kosten_Unverzinslich}";
                    Console.WriteLine(errorMessage);
                    _errorMessages.AppendLine(errorMessage);
                    salden.Zinsen = Math.Abs(salden.Zinsen);
                    salden.Zinsen = Math.Abs(salden.Zinsen);
                    salden.Kosten_Unverzinslich = Math.Abs(salden.Kosten_Unverzinslich);
                  }
                  string hf = salden.Hauptforderung.ToString("#0.00");
                  fieldSet[subitoMgr.Find_Column_by_Name("Hauptforderung")] = hf;
                  string zns = salden.Zinsen.ToString("#0.00");
                  fieldSet[subitoMgr.Find_Column_by_Name("Zinsen")] = zns;
                  string kosten;
                  string kostenAG;
                  if (salden.Kosten_Unverzinslich > salden.Spesen_Auftraggeber) {
                    kosten = (salden.Kosten_Unverzinslich - salden.Spesen_Auftraggeber).ToString("#0.00");
                    kostenAG = salden.Spesen_Auftraggeber.ToString("#0.00");
                  } else {
                    kosten = salden.Kosten_Unverzinslich.ToString("#0.00");
                    kostenAG = "0,00";
                  }
                  fieldSet[subitoMgr.Find_Column_by_Name("Kosten")] = kostenAG;  
                  fieldSet[subitoMgr.Find_Column_by_Name("Kosten 2 Betrag")] = kosten;
                  fieldSet[subitoMgr.Find_Column_by_Name("Zinssatz ab")] = DateTime.Now.ToString("dd.MM.yyyy");
                  fieldSet[subitoMgr.Find_Column_by_Name("Kostenzins ab")] = DateTime.Now.ToString("dd.MM.yyyy");
                  fieldSet[subitoMgr.Find_Column_by_Name("Verrechnung ab")] = DateTime.Now.ToString("dd.MM.yyyy");
                  fieldSet[subitoMgr.Find_Column_by_Name("Valuta")] = DateTime.Now.ToString("dd.MM.yyyy");
                  fieldSet[subitoMgr.Find_Column_by_Name("Abweichendes Übergabedatum")] = DateTime.Now.ToString("dd.MM.yyyy");
                } else {
                  Console.WriteLine($"Fehler beim Saldieren Akt {aktId} csvCount: {csvCounter}");
                }
              } catch (Exception e) {
                Console.WriteLine($"Exception Fehler beim Saldieren Akt {aktId} csvCount: {csvCounter}");
                Console.WriteLine(e.Message);
              }
            } else {
              Console.WriteLine($"Akt {aktId} NICHT GEFUNDEN");
            }
          }
        }
      }
      Console.WriteLine($"Anzahl Akte in CSV-File: {csvCounter}");
      Console.WriteLine("LESEN CSV FERTIG");

      Console.WriteLine($"Schreiben CSV FILE {_config.Outfile_4_Subito}");
      subitoMgr.WriteFile(Path.Combine(_config.BasePath, _config.Outfile_4_Subito));
      Console.WriteLine("Schreiben CSV FERTIG");

      Console.WriteLine("Schreiben Messages");
      File.WriteAllText(
        Path.Combine(_config.BasePath, $"ErrorMessages_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.txt"),
        _errorMessages.ToString());

      File.WriteAllText(
  Path.Combine(_config.BasePath, $"AktiveAkten_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.txt"),
  _aktiveAkten.ToString());

    } //end     static void Main(string[] args)

  } //end   internal class Program

} //end namespace Splitbuchungen_Auflösen

