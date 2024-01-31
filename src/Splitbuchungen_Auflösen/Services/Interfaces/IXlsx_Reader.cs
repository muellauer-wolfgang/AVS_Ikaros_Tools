using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Splitbuchungen_Auflösen.Models;

namespace Splitbuchungen_Auflösen.Services.Interfaces
{
  public interface IXlsx_Reader
  {
    IEnumerable<string> Retrieve_FirstColumn(string filename);
    IEnumerable<Akt_Einzelbuchung_DTO> Retrieve_Buchungen(string filename);    

  } //end   public interface IXlsx_Reader

} //end namespace Splitbuchungen_Auflösen.Services.Interfaces

