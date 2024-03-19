using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Move_Big_Documents.Infrastructure.Interfaces;

namespace Move_Big_Documents.Infrastructure
{
  public class ConfigProvider : IConfigProvider
  {
    //public string BasePath => @"H:\tmp\AVS\Export_Documents";
    public string BasePath => @"F:\Documents\Export_Documents";

    //public string ExportPath => @"H:\tmp\AVS\Fat_Documents";
    public string ExportPath => @"F:\Documents\Fat_Documents";

    public int MaxFileSize => 1_000_000;

  }


} //end namespace Move_Big_Documents.Infrastructure

