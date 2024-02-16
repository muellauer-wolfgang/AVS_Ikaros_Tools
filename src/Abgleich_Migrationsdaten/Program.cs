using System.Diagnostics;
using System.Text;
using Autofac;
using Syncfusion.XlsIO.FormatParser.FormatTokens;
using Abgleich_Migrationsdaten.Startup;
using Abgleich_Migrationsdaten.Infrastructure.Interfaces;
using Abgleich_Migrationsdaten.Services.Interfaces;
using Abgleich_Migrationsdaten.Models;

namespace Abgleich_Migrationsdaten
{
  internal class Program
  {
    public static IContainer _container { get; private set; } = new Bootstrapper().Bootstrap();
    public static IConfigProvider _config { get; set; }
    public static List<Migrations_Report_DTO> _report_Bhi { get; set; } = new();
    public static List<Migrations_Report_DTO> _report_Diverse { get; set; } = new();
    public static List<Ikaros_Akt_DTO> _akte_Bhi { get; set; } = new();
    public static List<Ikaros_Akt_DTO> _akte_Diverse { get; set; } = new(); 

    static void Main(string[] args)
    {
      _config = _container.Resolve<IConfigProvider>();
      Console.WriteLine("Ikaros_Tool Abgleich_Migrationsdaten");
      Console.WriteLine($"BasePath: {_config.BasePath}");

      IXlsx_Reader xlsxReader = _container.Resolve<IXlsx_Reader>();
      ICsv_Reader csvxReader = _container. Resolve<ICsv_Reader>();

      //Console.WriteLine(
      //  $"START lesen Report Migration BHI " + 
      //  Environment.NewLine +
      //  $"File: {_config.Report_File_Migration}");
      foreach (Migrations_Report_DTO dto in xlsxReader.Retrieve_Buchungen(
        Path.Combine(_config.BasePath, _config.Report_File_Migration), 
        "BHI")) {
        _report_Bhi.Add(dto);
      }
      Console.WriteLine($"Anzahl Records Migration BHI gelesen: {_report_Bhi.Count}");

      //Console.WriteLine(
      //  $"START lesen Report Migration Diverse " +
      //  Environment.NewLine +
      //  $"{_config.Report_File_Migration}");
      
      foreach (Migrations_Report_DTO dto in xlsxReader.Retrieve_Buchungen(
        Path.Combine(_config.BasePath, _config.Report_File_Migration),
        "Diverse")) {
        _report_Diverse.Add(dto);
      }
      Console.WriteLine($"Anzahl Records Migration Diverse gelesen: {_report_Diverse.Count}");


      //Console.WriteLine($"START lesen Akte_BHI {_config.Data_File_BHI_Akte}");
      foreach (Ikaros_Akt_DTO dto in csvxReader.Retrieve_Buchungen(
        Path.Combine(_config.BasePath, _config.Data_File_BHI_Akte))) {
        _akte_Bhi.Add(dto);
      }
      Console.WriteLine($"Anzahl Records Akte BHI gelesen: {_akte_Bhi.Count}");


      //Console.WriteLine($"START lesen Akte_Diverse {_config.Data_File_Diverse_Akte}");
      foreach (Ikaros_Akt_DTO dto in csvxReader.Retrieve_Buchungen(
        Path.Combine(_config.BasePath, _config.Data_File_Diverse_Akte))) {
        _akte_Diverse.Add(dto);
      }
      Console.WriteLine($"Anzahl Records Akte Diverse gelesen: {_akte_Diverse.Count}");

      //ich schaue jetzt, ob alle Ikaros-ANr aus BHI Import File in Migrations-Report sind
      HashSet<string> anrBhi_Import = new HashSet<string>();
      foreach(var r in _akte_Bhi) { anrBhi_Import.Add(r.IkarosAnr); }
      HashSet<string> anrBhi_Report = new HashSet<string>();
      foreach(var r in _report_Bhi) { anrBhi_Report.Add(r.IkarosAnr); }

      List<string> akteBhiNichtinReportFile = anrBhi_Import.Except(anrBhi_Report).ToList();
      Console.WriteLine($"Es sind {akteBhiNichtinReportFile.Count} Akte BHI in ImportFile und nicht in Report File");
      Console.WriteLine("Liste mit Differenzen:");
      foreach(string a in akteBhiNichtinReportFile) { Console.WriteLine($"  {a}"); }
      List<string> akteBhiNichtInImportFile = anrBhi_Report.Except(anrBhi_Import).ToList(); 
      Console.WriteLine($"Es sind {akteBhiNichtInImportFile.Count} Akte BHI in ReportFile und nicht in Import File");


      //ich schaue jetzt, ob alle Ikaros-ANr aus Diverse Import File in Migrations-Report sind
      HashSet<string> anrDiverse_Import = new HashSet<string>();
      foreach (var r in _akte_Diverse) { anrDiverse_Import.Add(r.IkarosAnr); }
      HashSet<string> anrDiverse_Report = new HashSet<string>();
      foreach (var r in _report_Diverse) { anrDiverse_Report.Add(r.IkarosAnr); }

      List<string> akteDiverseNichtinReportFile = anrDiverse_Import.Except(anrDiverse_Report).ToList();
      Console.WriteLine($"Es sind {akteDiverseNichtinReportFile.Count} Akte Diverse in ImportFile und nicht in Report File");
      Console.WriteLine("Liste mit Differenzen:");
      foreach (string a in akteDiverseNichtinReportFile) { Console.WriteLine($"  {a}"); }
      List<string> akteDiverseNichtInImportFile = anrDiverse_Report.Except(anrDiverse_Import).ToList();
      Console.WriteLine($"Es sind {akteDiverseNichtInImportFile.Count} Akte Diverse in ReportFile und nicht in Import File");






      /*
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
                  fieldSet[subitoMgr.Find_Column_by_Name("Zinssatz ab")] =    new DateTime(2024, 01, 31).ToString("dd.MM.yyyy");
                  fieldSet[subitoMgr.Find_Column_by_Name("Kostenzins ab")] =  new DateTime(2024, 01, 31).ToString("dd.MM.yyyy");
                  fieldSet[subitoMgr.Find_Column_by_Name("Verrechnung ab")] = new DateTime(2024, 01, 31).ToString("dd.MM.yyyy");
                  fieldSet[subitoMgr.Find_Column_by_Name("Valuta")] =         new DateTime(2024, 01, 31).ToString("dd.MM.yyyy");
                  fieldSet[subitoMgr.Find_Column_by_Name("Abweichendes Übergabedatum")] = new DateTime(2024, 01, 31).ToString("dd.MM.yyyy");
                } else {
                  if (buchungsDict.ContainsKey(aktId)) {
                    try {
                      if (aktId.Equals("20160012037")) {
                        Trace.Write("Trigger");
                      }
                      AktBuchungen b = buchungsDict[aktId];
                      if (string.IsNullOrEmpty(b.Aktenzeichen)) {
                        b.Aktenzeichen = aktId;
                      } else {
                        Trace.Write("Das geht dann nicht");
                      }
                      BuchungsSaldo salden = buchungsDict[aktId].SaldiereBuchungen();
                      if (salden.Letzte_Zahlung_Am > DateTime.Now.AddDays(-2000)) {
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
                        string kostenOffen;
                        string kostenAgOffen;
                        decimal kAgOffen = salden.Spesen_Auftraggeber - salden.Spesen_Auftraggeber_Abgerechnet;
                        if (kAgOffen < decimal.Zero) {
                          kAgOffen = decimal.Zero;
                        }
                        kostenOffen = (salden.Kosten_Unverzinslich - kAgOffen).ToString("#0.00");
                        kostenAgOffen = kAgOffen.ToString("#0.00");


                        if (kAgOffen  > decimal.Zero) {
                          string msgKostenAg = $"KOSTEN AG NOCH OFFEN AKT: {aktId} K-AG: {kAgOffen}";
                          Console.WriteLine(msgKostenAg);
                          _kostenAgOffen.AppendLine(msgKostenAg);
                        } else {
                          string msgKostenAg = $"KOSTEN AG ERLEDIGT AKT: {aktId} K-AG: {kAgOffen}";
                          Console.WriteLine(msgKostenAg);
                          _kostenAgErledigt.AppendLine(msgKostenAg);
                        }



                        fieldSet[subitoMgr.Find_Column_by_Name("Kosten")] = kostenAgOffen;
                        fieldSet[subitoMgr.Find_Column_by_Name("Kosten 2 Betrag")] = kostenOffen;
                        fieldSet[subitoMgr.Find_Column_by_Name("Zinssatz ab")] =    new DateTime(2024, 01, 31).ToString("dd.MM.yyyy");
                        fieldSet[subitoMgr.Find_Column_by_Name("Kostenzins ab")] =  new DateTime(2024, 01, 31).ToString("dd.MM.yyyy");
                        fieldSet[subitoMgr.Find_Column_by_Name("Verrechnung ab")] = new DateTime(2024, 01, 31).ToString("dd.MM.yyyy");
                        fieldSet[subitoMgr.Find_Column_by_Name("Valuta")] =         new DateTime(2024, 01, 31).ToString("dd.MM.yyyy");
                        fieldSet[subitoMgr.Find_Column_by_Name("Abweichendes Übergabedatum")] = new DateTime(2024, 01, 31).ToString("dd.MM.yyyy");
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

            File.WriteAllText(
              Path.Combine(_config.BasePath, $"KostenAgOffen_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.txt"),
              _kostenAgOffen.ToString());

            File.WriteAllText(
              Path.Combine(_config.BasePath, $"KostenAgErledigt_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.txt"),
              _kostenAgErledigt.ToString());
      */

    } //end     static void Main(string[] args)

  } //end   internal class Program

} //end namespace Splitbuchungen_Auflösen

