using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Move_Big_Documents.Infrastructure.Interfaces
{
  public interface IConfigProvider
  {
    string BasePath { get; }  
    string ExportPath { get; }
    int MaxFileSize { get; }  

  }

}
