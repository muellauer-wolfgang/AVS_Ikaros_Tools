using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Splitbuchungen_Auflösen.Services;
using Splitbuchungen_Auflösen;
using System.ComponentModel;
using Autofac;
using Splitbuchungen_Auflösen.Services.Interfaces;

namespace Splitbuchungen_Auflösen.Models
{

  /// <summary>
  /// In dieser Klasse fasse ich die Buchungen (Zahlungen und Belastungen)
  /// für jeden Akt zusammen. Es gibt ein Aktenzeichen und dann eine
  /// Liste mit Buchungen. 
  /// </summary>
  public class AktBuchungen
  {
    private IVerzinsungs_Service _zinsenSVC;
    public string Aktenzeichen { get; set; }
    public List<Einzelbuchung> BuchungsListe { get; private set; }
    public Hauptforderung_Verzinsung VerzinsungsInfo { get; private set; }
    public AktBuchungen(IVerzinsungs_Service zinsenSVC)
    {
      BuchungsListe = new();
      VerzinsungsInfo = null;
      _zinsenSVC = zinsenSVC; 
    }

    /// <summary>
    /// Diese Methode trägt eine Buchung in die Liste ein Hierbei ist einiges zu beachten:
    /// der Betrag ist in der Datenbank immer positiv, das ist aber zum Rechnen nicht
    /// sinnvoll. Daher muss ich ihn bei Zahlungen invertieren. Dann sind die Zinsen
    /// nicht korrekt, da ist ein Rundungsfehler. 
    /// </summary>
    public void Add_Einzelbuchung(
      DateTime valutadatum,
      string kürzel, string kurztext,
      decimal betrag,
      decimal kostenVerzinst, decimal kostenUnverzinst, decimal kostenZinsen, decimal hauptforderung)
    {
      Einzelbuchung newEB = new();
      newEB.Valutadatum = valutadatum;
      if (!string.IsNullOrEmpty(kürzel)) { newEB.Kürzel = kürzel; } else { newEB.Kürzel = "N.N."; }
      if (!string.IsNullOrEmpty(kurztext)) { newEB.Kurztext = kurztext; } else { newEB.Kurztext = "N.N."; }
      if (kürzel.Equals("Z001", StringComparison.InvariantCultureIgnoreCase)) {
        newEB.Betrag = betrag * -1M;
      } else {
        newEB.Betrag = betrag;
      }
      newEB.Kosten_Verzinst = kostenVerzinst;
      newEB.Kosten_Unverzinst = kostenUnverzinst;
      newEB.Kosten_Zinsen = kostenZinsen;
      newEB.Kosten_Hauptforderung = hauptforderung;
      //jetzt noch Kontrolle, ob Fehler bei Zinsen und wenn ja: korrigieren
      if (newEB.Betrag != (newEB.Kosten_Verzinst + newEB.Kosten_Unverzinst + newEB.Kosten_Zinsen + newEB.Kosten_Hauptforderung)) {
        if (newEB.Kosten_Zinsen != Decimal.Zero) {
          newEB.Kosten_Zinsen = (newEB.Betrag) - (newEB.Kosten_Unverzinst + newEB.Kosten_Verzinst + newEB.Kosten_Hauptforderung);
        } else {
         // throw new InvalidDataException("RUNDUNGSFEHLER!!!");
        }
      }
      BuchungsListe.Add(newEB);
    }

    public void Add_Einzelbuchung(Akt_Einzelbuchung_DTO dto)
    {
      Add_Einzelbuchung(
        dto.Valutadatum,
        dto.Kürzel,
        dto.Kurztext,
        dto.Betrag,
        dto.Kosten_Verzinst,
        dto.Kosten_Unverzinst,
        dto.Kosten_Zinsen,
        dto.Kosten_Hauptforderung);
      if (dto.VerzinsungsInfo != null) {
        this.VerzinsungsInfo = dto.VerzinsungsInfo;
      }
    }



    public BuchungsSaldo SaldiereBuchungen()
    {
      if (VerzinsungsInfo == null) {
        Debug.WriteLine("Kein VerzinsungsInfo -- on the fly mit defaultwerten erstellen");
        VerzinsungsInfo = new Hauptforderung_Verzinsung() { ZinsenAb = DateTime.Now, Zinsart = "Fix", Zinssatz = Decimal.Zero };
        //return null;
      }

      DateTime letzteVerzinsung = this.VerzinsungsInfo.ZinsenAb;

      BuchungsSaldo saldo = new();
      foreach (Einzelbuchung eb in this.BuchungsListe) {
        if (eb.Kosten_Zinsen < Decimal.Zero) {
          Debug.WriteLine("TriggerWarnung");
          List<Einzelbuchung> zns = _zinsenSVC.Calculate_Zinsen(saldo.Kosten_Hauptforderung, letzteVerzinsung, eb.Valutadatum, VerzinsungsInfo);
          decimal zinsenBelastung = decimal.Zero;
          foreach(Einzelbuchung e in zns) {
            zinsenBelastung += e.Kosten_Zinsen;
          }
          //setzen des neuen Datums und belasten der Zinsen
          letzteVerzinsung = eb.Valutadatum.AddDays(1);
          saldo.Kosten_Zinsen += zinsenBelastung;
        }
        saldo.Betrag += eb.Betrag;
        saldo.Kosten_Verzinst += eb.Kosten_Verzinst;
        saldo.Kosten_Unverzinst += eb.Kosten_Unverzinst;
        saldo.Kosten_Zinsen += eb.Kosten_Zinsen;
        saldo.Kosten_Hauptforderung += eb.Kosten_Hauptforderung;
      }
      return saldo;
    }

  } //end    public class AktBuchungen


  /// <summary>
  /// Diese Klasse kapselt eine Einzelbuchung mit den Konten
  /// KostenVerzinst, KostenUnverzinst, Zinsen, Kapital
  /// Das Feld Betrag enthält die Gesamtbetrag bie Split
  /// </summary>
  public class Einzelbuchung
  {
    public DateTime Valutadatum { get; set; }
    public string Kürzel { get; set; }
    public string Kurztext { get; set; }
    public decimal Betrag { get; set; }
    public decimal Kosten_Verzinst { get; set; }
    public decimal Kosten_Unverzinst { get; set; }
    public decimal Kosten_Zinsen { get; set; }
    public decimal Kosten_Hauptforderung { get; set; }

    public override string ToString()
    {
      return $"D:{Valutadatum:yyyy-MM-dd} K:{Kürzel:-6} B:{Betrag:0,000.00} KV{Kosten_Verzinst:0,000.00} KU:{Kosten_Unverzinst:0,000.00} KZ:{Kosten_Zinsen:0,000.00} KH:{Kosten_Hauptforderung:0,000.00}";
    }

  } //end   public class Einzelbuchung


  public class Akt_Einzelbuchung_DTO
  {
    public DateTime Valutadatum { get; set; }
    public string Aktenzeichen { get; set; }
    public string Kürzel { get; set; }
    public string Kurztext { get; set; }
    public decimal Betrag { get; set; }
    public decimal Kosten_Verzinst { get; set; }
    public decimal Kosten_Unverzinst { get; set; }
    public decimal Kosten_Zinsen { get; set; }
    public decimal Kosten_Hauptforderung { get; set; }
    public Hauptforderung_Verzinsung VerzinsungsInfo { get; set; }

    public override string ToString()
    {
      return $"D:{Valutadatum:yyyy-MM-dd} AZ:{Aktenzeichen:-15} K:{Kürzel:-6} T:{Kurztext} B:{Betrag:0,000.00} KV{Kosten_Verzinst:0,000.00} KU:{Kosten_Unverzinst:0,000.00} KZ:{Kosten_Zinsen:0,000.00} KH:{Kosten_Hauptforderung:0,000.00}";
    }

  } //end public class Akt_Einzelbuchung_DTO


  /// <summary>
  /// Wie verzinst werden soll, steht bei der Hauptforderung und wird aus dem Excel
  /// extrahhert, wenn vorhanden
  /// </summary>
  public class Hauptforderung_Verzinsung
  {
    public string Zinsart { get; set; }
    public decimal Zinssatz { get; set; }
    public decimal ZinsenAus { get; set; }
    public string Forderungsart { get; set; }
    public decimal Forderungsanteil { get; set; }
    public DateTime ZinsenAb { get; set; }
    public Hauptforderung_Verzinsung() { }

  }

  public class BuchungsSaldo
  {
    public decimal Betrag { get; set; }
    public decimal Kosten_Verzinst { get; set; }
    public decimal Kosten_Unverzinst { get; set; }
    public decimal Kosten_Zinsen { get; set; }
    public decimal Kosten_Hauptforderung { get; set; }

    public override string ToString()
    {
      return $"SB:{Betrag:#,##0.00} SKV{Kosten_Verzinst:#,##0.00} SKU:{Kosten_Unverzinst:#,##0.00} SKZ:{Kosten_Zinsen:#,##0.00} SKH:{Kosten_Hauptforderung:#,##0.00}";
    }

  } //end   public class  BuchungsSaldo

} //end namespace Splitbuchungen_Auflösen.Models

