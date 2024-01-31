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
namespace Splitbuchungen_Auflösen.Startup
{
  public class Bootstrapper
  {
    public IContainer Bootstrap()
    {
      ContainerBuilder builder = new ContainerBuilder();
      builder.RegisterType<ConfigProvider>().As<IConfigProvider>().SingleInstance();
      builder.RegisterType<Ikaros_Xlsx_Reader>().As<IXlsx_Reader>().SingleInstance();


      return builder.Build();
    }

  } //end   public class Bootstrapper

} //end namespace Splitbuchungen_Auflösen.Startup

