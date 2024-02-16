using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.XlsIO;
using Abgleich_Ikaros_Export_Subito_Query.Infrastructure.Interfaces;
using Abgleich_Ikaros_Export_Subito_Query.Models;
using Abgleich_Ikaros_Export_Subito_Query.Services.Interfaces;

namespace Abgleich_Ikaros_Export_Subito_Query.Services
{
  public class Xlsx_Reader : IXlsx_Reader
  {
    private IDictionary<string, int> _columnMapping = new Dictionary<string, int>();

    public Xlsx_Reader(IConfigProvider cfg)
    {
      Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NAaF5cWWJCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdnWX5ceXVTQmVdVEd1Vkc=");
    }

    public IEnumerable<Subito_Akt_DTO> Retrieve_Akte(string fileName)
    {
      if (string.IsNullOrEmpty(fileName)) { yield break; }
      if (!File.Exists(fileName)) { yield break; }
      using (ExcelEngine excelEngine = new ExcelEngine()) {
        IApplication application = excelEngine.Excel;
        application.DefaultVersion = ExcelVersion.Xlsx;
        FileStream inputStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        IWorkbook workbook = application.Workbooks.Open(inputStream, ExcelOpenType.Automatic);
        IWorksheet worksheet = workbook.Worksheets[0];
        int rowCount = worksheet.Rows.Length;
        int colCount = worksheet.Columns.Length;
        if (rowCount < 2) { yield break; }
        if (colCount < 5) { yield break; }
        //Lesen der Columns der Zeile 1 für Init Dictionary
        for (int i = 1; i <= colCount; i++) {
          _columnMapping[worksheet[1, i].Value] = i;
        }
        for (int r = 2; r <= rowCount; r++) {
          Subito_Akt_DTO mrDTO = new();
          mrDTO.AngelegtAm = worksheet[r, Find_Column_by_Name("Angelegt_am")].Value; 
          mrDTO.SubitoAnr =  worksheet[r, Find_Column_by_Name("Subito_AZ")].Value;
          mrDTO.IkarosAnr =  worksheet[r, Find_Column_by_Name("Ikaros_AZ")].Value;
          mrDTO.Phase =      worksheet[r, Find_Column_by_Name("Phase")].Value;
          mrDTO.Herkunft =   worksheet[r, Find_Column_by_Name("Herkunft")].Value;
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

  } //end   public class Xlsx_Reader : IXlsx_Reader

} //end namespace Abgleich_Ikaros_Export_Subito_Query.Services


