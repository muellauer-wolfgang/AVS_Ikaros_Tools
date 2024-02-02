using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.XlsIO;
using Splitbuchungen_Auflösen.Infrastructure.Interfaces;
using Splitbuchungen_Auflösen.Services.Interfaces;
using System.Reflection.Metadata.Ecma335;
using System.Diagnostics;
using Splitbuchungen_Auflösen.Models;

namespace Splitbuchungen_Auflösen.Services
{
  public class Ikaros_Xlsx_Reader : IXlsx_Reader
  {
    private IConfigProvider _configProvider;
    private IDictionary<string, int> _columnMapping = new Dictionary<string, int>();

    public Ikaros_Xlsx_Reader(IConfigProvider cfg)
    {
      _configProvider = cfg;
      Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NAaF5cWWJCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdnWX5ceXVTQmVdVEd1Vkc=");
    }

    public int Find_Column_by_Name(string name)
    {
      if (_columnMapping == null) {
        throw new InvalidDataException("CSV-ColumnMapping dict fehlt");
      }
      if (!_columnMapping.ContainsKey(name)) {
        throw new InvalidDataException("CSV-ColumnMapping Columns not Found");
      }
      return _columnMapping[name];
    }


    public IEnumerable<Akt_Einzelbuchung_DTO> Retrieve_Buchungen(string filename)
    {
      if (string.IsNullOrEmpty(filename)) { yield break; }
      if (!filename.StartsWith(_configProvider.BasePath)) { filename = Path.Combine(_configProvider.BasePath, filename); }
      if (!File.Exists(filename)) { yield break; }

      using (ExcelEngine excelEngine = new ExcelEngine()) {
        IApplication application = excelEngine.Excel;
        application.DefaultVersion = ExcelVersion.Xlsx;
        FileStream inputStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
        IWorkbook workbook = application.Workbooks.Open(inputStream, ExcelOpenType.Automatic);

        IWorksheet worksheet = workbook.Worksheets[0];
        int rowCount = worksheet.Rows.Length;
        int colCount = worksheet.Columns.Length;

        if (rowCount < 2) { yield break; }
        if (colCount < 23) { yield break; }

        //Lesen der Line 1 Columns für Init Dictionary
        for (int i =1; i <= colCount; i++) {
          _columnMapping[worksheet[1, i].Value] = i;
        }

        for (int r = 2; r <= rowCount; r++) {
          Akt_Einzelbuchung_DTO ebDTO = new Akt_Einzelbuchung_DTO();
          ebDTO.Aktenzeichen = worksheet[r, Find_Column_by_Name("Aktenzeichen")].Value;
          if (DateTime.TryParse(worksheet[r, Find_Column_by_Name("Datum")].Value, null, DateTimeStyles.None, out DateTime valutadatum)) {
            ebDTO.Valutadatum = valutadatum;
          } else {
            ebDTO.Valutadatum = DateTime.MinValue;
          }
          ebDTO.Kürzel = worksheet[r, Find_Column_by_Name("Kürzel")].Value;
          ebDTO.Kurztext = worksheet[r, Find_Column_by_Name("Kurztext")].Value;
          if (decimal.TryParse(worksheet[r, Find_Column_by_Name("Betrag")].Value, NumberStyles.Any, null, out decimal betrag)) {
            ebDTO.Betrag = betrag;
          } else {
            ebDTO.Betrag = decimal.Zero;
          }
          if (decimal.TryParse(worksheet[r, Find_Column_by_Name("Kosten_Verzinst")].Value, NumberStyles.Any, null, out decimal kv)) {
            ebDTO.Kosten_Verzinst = kv;
          } else {
            ebDTO.Kosten_Verzinst = decimal.Zero;
          }
          if (decimal.TryParse(worksheet[r, Find_Column_by_Name("Kosten_Unverzinst")].Value, NumberStyles.Any, null, out decimal ku)) {
            ebDTO.Kosten_Unverzinst = ku;
          } else {
            ebDTO.Kosten_Unverzinst = decimal.Zero;
          }
          if (decimal.TryParse(worksheet[r, Find_Column_by_Name("Kosten_Zinsen")].Value, NumberStyles.Any, null, out decimal kz)) {
            ebDTO.Kosten_Zinsen = kz;
          } else {
            ebDTO.Kosten_Zinsen = decimal.Zero;
          }
          if (decimal.TryParse(worksheet[r, Find_Column_by_Name("Kosten_Hauptforderung")].Value, NumberStyles.Any, null, out decimal kh)) {
            ebDTO.Kosten_Hauptforderung = kh;
          } else {
            ebDTO.Kosten_Hauptforderung = decimal.Zero;
          }
          //jetzt noch kontrollieren, ob ich Verzinsungs-Info finde
          string zinsart = worksheet[r, Find_Column_by_Name("Zinsart")].Value;
          if (!string.IsNullOrEmpty(zinsart)) {
            Hauptforderung_Verzinsung hfz = new Hauptforderung_Verzinsung();
            hfz.Zinsart = worksheet[r, Find_Column_by_Name("Zinsart")].Value;
            
            if (decimal.TryParse(worksheet[r, Find_Column_by_Name("Zinssatz")].Value, NumberStyles.Any, null, out decimal zinssatz)) {
              hfz.Zinssatz = zinssatz;
            } else {
              hfz.Zinssatz = decimal.Zero;
            }

            if (decimal.TryParse(worksheet[r, Find_Column_by_Name("Zinsen aus")].Value, NumberStyles.Any, null, out decimal zinsenaus)) {
              hfz.ZinsenAus = zinsenaus;
            } else {
              hfz.ZinsenAus = decimal.Zero;
            }

            hfz.Forderungsart = worksheet[r, Find_Column_by_Name("Forderungsart")].Value;

            if (decimal.TryParse(worksheet[r, Find_Column_by_Name("Forderungsanteil")].Value, NumberStyles.Any, null, out decimal forderungsanteil)) {
              hfz.Forderungsanteil = forderungsanteil;
            } else {
              hfz.Forderungsanteil = decimal.One;
            }

            if (DateTime.TryParse(worksheet[r, Find_Column_by_Name("Zinssatz Von")].Value, null, DateTimeStyles.None, out DateTime verzinstAb)) {
              hfz.ZinsenAb = verzinstAb;
            } else {
              hfz.ZinsenAb = DateTime.MinValue;
            }
            ebDTO.VerzinsungsInfo = hfz;
          } else {
            ebDTO.VerzinsungsInfo = null;
          }

          //kontrolle, ob es eine wirklich sinnvolle Zeile ist:
          if (string.IsNullOrEmpty(ebDTO.Aktenzeichen)) {
            Debug.WriteLine($"Error in Line {r}");
            continue;
            //ohne Aktenzeichen hat das keinen Sinn
          }
          if (ebDTO.Kürzel.Equals("H00") && ebDTO.VerzinsungsInfo == null) {
            Debug.WriteLine("sollte es nicht geben");
            continue;
          }
          yield return ebDTO;
        }
        excelEngine.Dispose();
      }
    }
     
  } //end   public class Ikaros_Xlsx_Reader

} //end namespace Splitbuchungen_Auflösen.Services

