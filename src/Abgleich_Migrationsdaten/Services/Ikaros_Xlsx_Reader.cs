using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.XlsIO;
using Abgleich_Migrationsdaten.Infrastructure.Interfaces;
using Abgleich_Migrationsdaten.Services.Interfaces;
using System.Reflection.Metadata.Ecma335;
using System.Diagnostics;
using Abgleich_Migrationsdaten.Models;

namespace Abgleich_Migrationsdaten.Services
{
  /// <summary>
  /// Diese Methode liest DTOs aus dem Excel Sheet mit den relevanten
  /// Daten der Buchungen.
  /// </summary>
  public class Ikaros_Xlsx_Reader : IXlsx_Reader
  {
    private IDictionary<string, int> _columnMapping = new Dictionary<string, int>();

    public Ikaros_Xlsx_Reader(IConfigProvider cfg)
    {
      Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NAaF5cWWJCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdnWX5ceXVTQmVdVEd1Vkc=");
    }

    /// <summary>
    /// Hier wird ein Worksheet gelesen und die DTOs werden übergeben
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public IEnumerable<Migrations_Report_DTO> Retrieve_Buchungen(string fileName, string worksheetName)
    {
      if (string.IsNullOrEmpty(fileName)) { yield break; }
      if (!File.Exists(fileName)) { yield break; }
      using (ExcelEngine excelEngine = new ExcelEngine()) {
        IApplication application = excelEngine.Excel;
        application.DefaultVersion = ExcelVersion.Xlsx;
        FileStream inputStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        IWorkbook workbook = application.Workbooks.Open(inputStream, ExcelOpenType.Automatic);

        IWorksheet worksheet = workbook.Worksheets[worksheetName];
        int rowCount = worksheet.Rows.Length;
        int colCount = worksheet.Columns.Length;

        if (rowCount < 2) { yield break; }
        if (colCount < 5) { yield break; }

        //Lesen der Columns der Zeile 1 für Init Dictionary
        for (int i =1; i <= colCount; i++) {
          _columnMapping[worksheet[1, i].Value] = i;
        }

        for (int r = 2; r <= rowCount; r++) {
          Migrations_Report_DTO mrDTO = new ();
          mrDTO.IkarosAnr = worksheet[r, Find_Column_by_Name("IkarosAnr")].Value;
          mrDTO.SubitoAnr = worksheet[r, Find_Column_by_Name("SubitoAnr")].Value;
          mrDTO.Auftraggeber = worksheet[r, Find_Column_by_Name("Auftraggeber")].Value;
          mrDTO.Gläubiger = worksheet[r, Find_Column_by_Name("Gläubiger")].Value;
          mrDTO.ProcessingState = worksheet[r, Find_Column_by_Name("ProcessingState")].Value;
          yield return mrDTO;
        }
        excelEngine.Dispose();
      }
    }

    private int Find_Column_by_Name(string name)
    {
      if (_columnMapping == null) {
        throw new InvalidDataException("CSV-ColumnMapping dict fehlt");
      }
      if (!_columnMapping.ContainsKey(name)) {
        throw new InvalidDataException("CSV-ColumnMapping Columns not Found");
      }
      return _columnMapping[name];
    }


  } //end   public class Ikaros_Xlsx_Reader

} //end namespace Splitbuchungen_Auflösen.Services

