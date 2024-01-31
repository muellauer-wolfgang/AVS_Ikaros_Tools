using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Splitbuchungen_Auflösen.Infrastructure.Interfaces;

namespace Splitbuchungen_Auflösen.Infrastructure
{
  public class ConfigProvider : IConfigProvider
  {
    public string BasePath => @"H:\mdc\Kunden\AVS\Software\Ikaros_Tools\daten";

  } //end   public class ConfigProvider : IConfigProvider


} //end namespace Splitbuchungen_Auflösen.Infrastructure

