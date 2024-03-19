using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Copy_Content_dot_Json.Infrastructure.Interfaces
{
  public interface IConfigProvider
  {
    string BasePath { get; }
    string ExportPath { get; }

  }

}
