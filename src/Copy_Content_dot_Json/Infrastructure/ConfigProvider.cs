using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Copy_Content_dot_Json.Infrastructure.Interfaces;

namespace Copy_Content_dot_Json.Infrastructure
{
  public class ConfigProvider : IConfigProvider
  {
    //public string BasePath => @"H:\tmp\AVS\Export_Documents";
    public string BasePath => @"F:\Documents\Export_Documents";

    //public string ExportPath => @"H:\tmp\AVS\Fat_Documents";
    public string ExportPath => @"F:\Documents\Fat_Documents";

  } //end   public class ConfigProvider : IConfigProvider

} //end namespace Copy_Content_dot_Json.Infrastructure


