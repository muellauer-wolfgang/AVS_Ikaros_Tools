using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dokumente_2_Subito.Infrastructure.Interfaces;

namespace Dokumente_2_Subito.Infrastructure
{
  public class ConfigProvider : IConfigProvider
  {
    public string ConnectionString_ASA => "DSN=IKAROS-VM";

    public string ConnectionString_MSSQL => @"Data Source=localhost\MDC;Initial Catalog=avs;Persist Security Info=True;Trusted_Connection=Yes;TrustServerCertificate=True";

    public string BasePath => @"H:\tmp\AVS\Tmp";

    public string ExportPath => @"H:\tmp\AVS\Export_Msg";

    public string IkarosDocumentPath => @"H:\mdc\Kunden\AVS\Software_Ikaros\_Ikaros_Dokumente\DB_Managed";

    public string SchulderAbrechnungPath => @"H:\tmp";

  } //end   public class ConfigProvider


} //end namespace Dokumente_2_Subito.Infrastructure

