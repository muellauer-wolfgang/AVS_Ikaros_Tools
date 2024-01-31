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

    public Ikaros_Xlsx_Reader(IConfigProvider cfg)
    {
      _configProvider = cfg;
      Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NAaF5cWWJCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdnWX5ceXVTQmVdVEd1Vkc=");
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

        for (int r = 2; r <= rowCount; r++) {
          Akt_Einzelbuchung_DTO ebDTO = new Akt_Einzelbuchung_DTO();
          ebDTO.Aktenzeichen = worksheet[r, 1].Value;
          if (DateTime.TryParse(worksheet[r, 15].Value, null, DateTimeStyles.None, out DateTime valutadatum)) {
            ebDTO.Valutadatum = valutadatum;
          } else {
            ebDTO.Valutadatum = DateTime.MinValue;
          }
          ebDTO.Kürzel = worksheet[r, 16].Value;
          ebDTO.Kurztext = worksheet[r, 17].Value;
          if (decimal.TryParse(worksheet[r, 18].Value, NumberStyles.Any, null, out decimal betrag)) {
            ebDTO.Betrag = betrag;
          } else {
            ebDTO.Betrag = decimal.Zero;
          }
          if (decimal.TryParse(worksheet[r, 19].Value, NumberStyles.Any, null, out decimal kv)) {
            ebDTO.Kosten_Verzinst = kv;
          } else {
            ebDTO.Kosten_Verzinst = decimal.Zero;
          }
          if (decimal.TryParse(worksheet[r, 20].Value, NumberStyles.Any, null, out decimal ku)) {
            ebDTO.Kosten_Unverzinst = ku;
          } else {
            ebDTO.Kosten_Unverzinst = decimal.Zero;
          }
          if (decimal.TryParse(worksheet[r, 21].Value, NumberStyles.Any, null, out decimal kz)) {
            ebDTO.Kosten_Zinsen = kz;
          } else {
            ebDTO.Kosten_Zinsen = decimal.Zero;
          }
          if (decimal.TryParse(worksheet[r, 22].Value, NumberStyles.Any, null, out decimal kh)) {
            ebDTO.Kosten_Hauptforderung = kh;
          } else {
            ebDTO.Kosten_Hauptforderung = decimal.Zero;
          }
          //jetzt noch kontrollieren, ob ich Verzinsungs-Info finde
          string zinsart = worksheet[r, 26].Value;
          if (!string.IsNullOrEmpty(zinsart)) {
            Hauptforderung_Verzinsung hfz = new Hauptforderung_Verzinsung();
            hfz.Zinsart = worksheet[r, 26].Value;
            
            if (decimal.TryParse(worksheet[r, 27].Value, NumberStyles.Any, null, out decimal zinssatz)) {
              hfz.Zinssatz = zinssatz;
            } else {
              hfz.Zinssatz = decimal.Zero;
            }

            if (decimal.TryParse(worksheet[r, 28].Value, NumberStyles.Any, null, out decimal zinsenaus)) {
              hfz.ZinsenAus = zinsenaus;
            } else {
              hfz.ZinsenAus = decimal.Zero;
            }

            hfz.Forderungsart = worksheet[r, 29].Value;

            if (decimal.TryParse(worksheet[r, 30].Value, NumberStyles.Any, null, out decimal forderungsanteil)) {
              hfz.Forderungsanteil = forderungsanteil;
            } else {
              hfz.Forderungsanteil = decimal.One;
            }

            if (DateTime.TryParse(worksheet[r, 31].Value, null, DateTimeStyles.None, out DateTime verzinstAb)) {
              hfz.ZinsenAb = verzinstAb;
            } else {
              hfz.ZinsenAb = DateTime.MinValue;
            }
            ebDTO.VerzinsungsInfo = hfz;
          } else {
            ebDTO.VerzinsungsInfo = null;
          }

          yield return ebDTO;
        }
        excelEngine.Dispose();
      }
    }

    public IEnumerable<string> Retrieve_FirstColumn(string filename)
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

        if (rowCount < 2) { yield break; }
        yield return worksheet[1, 1].Value;

        for (int r = 2; r < rowCount; r++) {
          yield return worksheet[r, 1].Value;
        }
        excelEngine.Dispose();
      }
    }


  } //end   public class Ikaros_Xlsx_Reader

} //end namespace Splitbuchungen_Auflösen.Services


/*

*/