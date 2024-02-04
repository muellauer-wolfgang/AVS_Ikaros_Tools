using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splitbuchungen_Auflösen.Models
{
  /// <summary>
  /// Dieses DTO-Objekt wird beim Lesen des xlsx Files erzeugt
  /// und entält alle Daten einer Buchung
  /// </summary>
  public class Einzelbuchung_DTO 
  {
    public DateTime Valutadatum { get; set; }
    public string Aktenzeichen { get; set; }
    public string Kürzel { get; set; }
    public string Kurztext { get; set; }
    public decimal Umsatz { get; set; }
    public decimal Kosten_Verzinslich { get; set; }
    public decimal Kosten_Unverzinslich { get; set; }
    public decimal Zinsen { get; set; }
    public decimal Hauptforderung { get; set; }
    public Verzinsung VerzinsungsInfo { get; set; }

    public override string ToString()
    {
      return
        $"EB Datum:{Valutadatum:yyyy-MM-dd} AZ:{Aktenzeichen:-15} Kürzel:{Kürzel:-6} Text:{Kurztext} " +
        Environment.NewLine +
        $"Umsatz:{Umsatz:#.##0.00} Kosten_Verzinslich: {Kosten_Verzinslich:#.##0.00} Kosten_Unverzinslich:{Kosten_Unverzinslich:#.##0.00} " +
        Environment.NewLine +
        $"Zinsen: {Zinsen:#.##0.00} Hauptforderung:{Hauptforderung:#.##0.00}";
    }

  } //end public class Akt_Einzelbuchung_DTO

} //end namespace Splitbuchungen_Auflösen.Models

