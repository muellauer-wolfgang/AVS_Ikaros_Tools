using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Splitbuchungen_Auflösen;
using Splitbuchungen_Auflösen.Infrastructure.Interfaces;
using Splitbuchungen_Auflösen.Infrastructure;
using Splitbuchungen_Auflösen.Services.Interfaces;
using Splitbuchungen_Auflösen.Services;
using Splitbuchungen_Auflösen.DataServices;

namespace Splitbuchungen_Auflösen.Startup
{
  public class Bootstrapper
  {
    public IContainer Bootstrap()
    {
      ContainerBuilder builder = new ContainerBuilder();
      builder.RegisterType<ConfigProvider>().As<IConfigProvider>().SingleInstance();
      builder.RegisterType<Ikaros_Xlsx_Reader>().As<IXlsx_Reader>().SingleInstance();
      builder.RegisterType<Subito_Csv_Manager>().As<ISubito_Csv_Manager>().SingleInstance();
      builder.RegisterType<SQL_Anywhere_Service>().AsSelf().SingleInstance();
      builder.RegisterType<Verzinsungs_Service>().AsSelf().SingleInstance();  

      return builder.Build();
    }

  } //end   public class Bootstrapper

} //end namespace Splitbuchungen_Auflösen.Startup

