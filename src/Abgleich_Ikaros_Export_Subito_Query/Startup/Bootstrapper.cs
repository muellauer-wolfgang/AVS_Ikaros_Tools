using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Abgleich_Ikaros_Export_Subito_Query.Infrastructure.Interfaces;
using Abgleich_Ikaros_Export_Subito_Query.Infrastructure;
using Abgleich_Ikaros_Export_Subito_Query.Services.Interfaces;
using Abgleich_Ikaros_Export_Subito_Query.Services;

using System.Reflection.Metadata.Ecma335;

namespace Abgleich_Ikaros_Export_Subito_Query.Startup
{
  public class Bootstrapper
  {
    public IContainer Bootstrap()
    {
      ContainerBuilder builder = new ContainerBuilder();
      builder.RegisterType<ConfigProvider>().As<IConfigProvider>().SingleInstance() ;
      builder.RegisterType<Xlsx_Reader>().As<IXlsx_Reader>().SingleInstance() ;
      builder.RegisterType<Csv_Reader_Writer>().As<ICsv_Reader_Writer>().SingleInstance() ;
      return builder.Build();
    }

  } //end   public class Bootstrapper

} //end namespace Abgleich_Ikaros_Export_Subito_Query.Startup

