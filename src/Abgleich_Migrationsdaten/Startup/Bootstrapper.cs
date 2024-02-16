using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Abgleich_Migrationsdaten;
using Abgleich_Migrationsdaten.Infrastructure.Interfaces;
using Abgleich_Migrationsdaten.Infrastructure;
using Abgleich_Migrationsdaten.Services.Interfaces;
using Abgleich_Migrationsdaten.Services;

namespace Abgleich_Migrationsdaten.Startup
{
  public class Bootstrapper
  {
    public IContainer Bootstrap()
    {
      ContainerBuilder builder = new ContainerBuilder();
      builder.RegisterType<ConfigProvider>().As<IConfigProvider>().SingleInstance();
      builder.RegisterType<Ikaros_Xlsx_Reader>().As<IXlsx_Reader>().SingleInstance();
      builder.RegisterType<Subito_Csv_Reader>().As<ICsv_Reader>().SingleInstance();
      return builder.Build();
    }

  } //end   public class Bootstrapper

} //end namespace Splitbuchungen_Auflösen.Startup

