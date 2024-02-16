using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abgleich_Ikaros_Export_Subito_Query.Models
{
  public class Migrations_Report_DTO
  {
    public string IkarosAnr { get; set; }
    public string SubitoAnr { get; set; }
    public string Auftraggeber { get; set; }
    public string Gläubiger { get; set; }
    public string ProcessingState { get; set; }
    public override string ToString()
    {
      return $"AZI:{IkarosAnr} AZS:{SubitoAnr}";
    }

  } //end   public class Migrations_Report_DTO

} //end namespace Abgleich_Ikaros_Export_Subito_Query.Models

