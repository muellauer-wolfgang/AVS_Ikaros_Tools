using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splitbuchungen_Auflösen.Infrastructure.Interfaces
{
  public interface IConfigProvider
  {
    string BasePath { get; }
    string Infile_Buchungen {  get; }
    string Infile_4_Subito { get; } 
    string Outfile_4_Subito { get; }

  }

} //end namespace Splitbuchungen_Auflösen.Infrastructure.Interfaces

