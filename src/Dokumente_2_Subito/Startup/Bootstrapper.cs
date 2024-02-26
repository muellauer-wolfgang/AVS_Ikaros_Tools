using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Dokumente_2_Subito.Infrastructure.Interfaces;
using Dokumente_2_Subito.Infrastructure;
using Dokumente_2_Subito.DataServices;
using Dokumente_2_Subito.DataServices.Interfaces;

namespace Dokumente_2_Subito.Startup
{
    public class Bootstrapper
  {
    public IContainer Bootstrap()
    {
      ContainerBuilder builder = new ContainerBuilder();
      builder.RegisterType<ConfigProvider>().As<IConfigProvider>().SingleInstance();  
      builder.RegisterType<IkarosDataService>().As<IIkarosDataService>().SingleInstance();  
      builder.RegisterType<MySqlDataService>().As<IMySqlDataService>().SingleInstance();
      return builder.Build();
    } //end     public IContainer Bootstrap()

  } //end   public class Bootstrapper

} //end namespace Dokumente_2_Subito.Startup

