using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abgleich_Migrationsdaten.Models
{
  /// <summary>
  /// Dieses DTO-Objekt wird beim Lesen des xlsx Files erzeugt
  /// und entält alle Daten einer Buchung
  /// </summary>
  public class Ikaros_Akt_DTO 
  {
    public string IkarosAnr { get; set; }
    public string Auftraggeber { get; set; }

    public override string ToString()
    {
      return $"AZ:{IkarosAnr} AG:{Auftraggeber}";
    }

  } //end public class Akt_Einzelbuchung_DTO

} //end namespace Splitbuchungen_Auflösen.Models

