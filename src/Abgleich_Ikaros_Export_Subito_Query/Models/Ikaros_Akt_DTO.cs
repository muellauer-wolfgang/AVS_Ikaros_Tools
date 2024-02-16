using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abgleich_Ikaros_Export_Subito_Query.Models
{
  public class Ikaros_Akt_DTO
  {
    public string IkarosAnr { get; set; }
    public string Auftraggeber { get; set; }

    public override string ToString()
    {
      return $"AZ:{IkarosAnr} AG:{Auftraggeber}";
    }

  } //end public class Akt_Einzelbuchung_DTO

} //end namespace Abgleich_Ikaros_Export_Subito_Query.Models

