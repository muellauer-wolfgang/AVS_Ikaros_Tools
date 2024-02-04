using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splitbuchungen_Auflösen.Models
{
  /// <summary>
  /// Diese Klasse kapselt eine Einzelbuchung mit den Konten
  /// KostenVerzinst, KostenUnverzinst, Zinsen, Kapital
  /// Das Feld Betrag enthält den Gesamtbetrag bei Split
  /// </summary>
  public class Einzelbuchung : IComparable<Einzelbuchung>
  {
    public DateTime Valutadatum { get; set; }
    public string Kürzel { get; set; }
    public string Kurztext { get; set; }
    public decimal Umsatz { get; set; }
    public decimal Kosten_Unverzinslich { get; set; }
    public decimal Kosten_Verzinslich { get; set; }
    public decimal Zinsen { get; set; }
    public decimal Hauptforderung { get; set; }
    public Verzinsung VerzinsungsInfo { get; set; }

    /// <summary>
    /// Default ctor, wird in der Regel nicht verwendent
    /// </summary>
    public Einzelbuchung() { }

    /// <summary>
    /// Copy ctor, so wird das Objekt in der Regel erzeugt. Wichtig ist, dass die
    /// Einzelbuchungen immer positiv sind und erst aus dem Kürzel hervorgeht,
    /// ob es eine Belastung, oder Gutschrift ist. Aber ich erledige das hier
    /// Zahlungen mit Z... werden negativ 
    /// </summary>
    /// <param name="dto"></param>
    public Einzelbuchung(Einzelbuchung_DTO dto)
    {
      this.Valutadatum = dto.Valutadatum;
      if (!string.IsNullOrEmpty(dto.Kürzel)) { this.Kürzel = dto.Kürzel; } else { this.Kürzel = "N.N."; }
      if (!string.IsNullOrEmpty(dto.Kurztext)) { this.Kurztext = dto.Kurztext; } else { this.Kurztext = "N.N."; }
      if (dto.Kürzel.Equals("Z001", StringComparison.InvariantCultureIgnoreCase)) {
        this.Umsatz = dto.Umsatz * -1M;
      } else {
        this.Umsatz = dto.Umsatz;
      }
      this.Kosten_Unverzinslich = dto.Kosten_Unverzinslich;
      this.Kosten_Verzinslich = dto.Kosten_Verzinslich;
      this.Zinsen = dto.Zinsen;
      this.Hauptforderung = dto.Hauptforderung;
      this.VerzinsungsInfo = dto.VerzinsungsInfo;
    }

    public override string ToString()
    {
      return 
        $"EINZELBUCHUNG Datum:{Valutadatum:yyyy-MM-dd} Kürzel:{Kürzel:-6} Umsatz:{Umsatz:#,##0.00}" +
        Environment.NewLine +
        $"K_Unverzinslich:{Kosten_Unverzinslich:#,##0.00} K_Verzinslich{Kosten_Verzinslich:#,##0.00} Zinsen:{Zinsen:#,##0.00} Hauptforderung:{Hauptforderung:#,##0.00}";
    }

    public int CompareTo(Einzelbuchung rhs)
    {
      if (rhs == null) return 1;
      if (this == rhs) return 0;
      return this.Valutadatum.CompareTo(rhs.Valutadatum); 
    }

  } //end   public class Einzelbuchung

} //end namespace Splitbuchungen_Auflösen.Models

