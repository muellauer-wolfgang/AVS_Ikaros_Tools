using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokumente_2_Subito.Infrastructure.Interfaces
{
  public interface IConfigProvider
  {
    string ConnectionString_ASA {  get; } 
    string ConnectionString_MySql { get; }  
    string BasePath {  get; }
    string ExportPath { get; }
    string IkarosDocumentPath { get; }
    string SchulderAbrechnungPath { get; }

  } //end   public interface IConfigProvider

} //end namespace Dokumente_2_Subito.Infrastructure.Interfaces

