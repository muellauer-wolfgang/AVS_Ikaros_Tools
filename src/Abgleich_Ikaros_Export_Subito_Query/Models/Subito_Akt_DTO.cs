using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abgleich_Ikaros_Export_Subito_Query.Models
{
  public class Subito_Akt_DTO
  {
    //Angelegt_am	Subito_AZ	Ikaros_AZ	Phase	Herkunft
    public string AngelegtAm { get; set; }
    public string SubitoAnr { get; set; }
    public string IkarosAnr { get; set; }
    public string Phase { get; set; }
    public string Herkunft { get; set; }
    public override string ToString()
    {
      return $"AZI:{IkarosAnr} AZS:{SubitoAnr}";
    }

  } //end   public class Subito_Akt_DTO

} //end namespace Abgleich_Ikaros_Export_Subito_Query.Models

