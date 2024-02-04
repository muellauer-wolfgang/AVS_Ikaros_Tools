using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Splitbuchungen_Auflösen.Models
{
  /// Der BuchungsSaldo ist praktisch das, was am Ende hinten
  /// rauskommt. Um ihn korrekt zu berechnen, muss ich bei mehrfachen
  /// Kapitalforderungen, die auch verschiedene Zinssätze haben könnnen,
  /// diese individuellen Salden mitführen, bis sie getiglt sind. 
  /// Ich denke, die Tilgung wird nach dem Datum erfolgen.
  /// </summary>
  public class BuchungsSaldo
  {
    public decimal Umsatz { get; set; }
    public decimal Kosten_Unverzinslich { get; set; }
    public decimal Kosten_Verzinslich { get; set; }
    public decimal Zinsen { get; set; }
    public decimal Hauptforderung { get; set; }
    public DateTime Letzte_Zahlung_Am { get; set; }
    public List<Einzelbuchung> Hauptforderungen_Offen { get; set; } = new();

    //Die Zwischensummern mit _Zs sind nur fürs Debugging interessant, dann kann
    //man die Wert einfach mit dem Papier vergleichen.
    public decimal _ZS_01_Umsatz => Umsatz;
    public decimal _ZS_02_Kosten_Unverzinslich => Kosten_Unverzinslich;
    public decimal _ZS_03_Kosten_Verzinslich => Kosten_Verzinslich;
    public decimal _ZS_04_Zinsen => Zinsen;
    public decimal _ZS_05_Hauptforderung => Hauptforderung;
    public decimal _ZS_06_Offen => this.HF_Zwischensaldo();

    /// <summary>
    /// Default ctor, alles bleibt leer
    /// </summary>
    public BuchungsSaldo() { }

    /// <summary>
    /// Dieser ctor sucht sich gleich die Hauptforderungen aus der
    /// Buchungsliste und startet mit den ganzen HF in place
    /// </summary>
    /// <param name="buchungsliste"></param>
    public BuchungsSaldo(IEnumerable<Einzelbuchung> buchungsliste)
    {
      foreach(Einzelbuchung eb in buchungsliste) {
        if (eb.Kürzel.Equals("H00") || eb.Kürzel.Equals("H04")) {
          this.Hauptforderungen_Offen.Add(eb);  
        }
      }
    }

    /// <summary>
    /// Ich muss jetzt von oben nach unten das Kapital tilgen und 
    /// solange die einzelforderungen reduzieren und entfernen,
    /// bis ich mit der Tilgung fertig bin.
    /// </summary>
    /// <param name="betrag"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void Kapital_Buchen(decimal betrag)
    {
      //wenn der Betrag positiv ist, wird einfach belastet
      if (betrag >= 0 ) {
        this.Hauptforderung += betrag;
      } else {
        this.Hauptforderung += betrag;
        decimal zuBuchen = betrag;
        foreach(Einzelbuchung fo in this.Hauptforderungen_Offen) {
          if (fo.Hauptforderung > zuBuchen * -1) {
            fo.Hauptforderung += zuBuchen;
            zuBuchen = decimal.Zero;
          }
          if (zuBuchen == decimal.Zero) {
            break;
          }
        }
        if (this.Hauptforderungen_Offen.Count > 1 ) {
          //jetzt kann ich noch schauen, ob ich vielleicht Forderungen entfernen kann..
          Trace.WriteLine("Liste ckecken!!!");
        }
      }
      return;
    }

    public override string ToString()
    {
      return
        $"SALDO: Umsatz:{Umsatz:#,##0.00}  UnVerz_K:{Kosten_Unverzinslich:#,##0.00} Verz_K:{Kosten_Verzinslich:#,##0.00} " +
        Environment.NewLine +
        $"Zinsen: {Zinsen:#,##0.00} Hauptforderung: {Hauptforderung:#,##0.00}";
    }

    /// <summary>
    /// Der Zwischensaldo ist zum Debuggen gedacht
    /// </summary>
    /// <returns></returns>
    private decimal HF_Zwischensaldo()
    {
      decimal zs = decimal.Zero;
      foreach(Einzelbuchung eb in this.Hauptforderungen_Offen) {
        zs += eb.Hauptforderung;
      }
      return zs;
    }

  } //end   public class  BuchungsSaldo

} //end namespace Splitbuchungen_Auflösen.Models

