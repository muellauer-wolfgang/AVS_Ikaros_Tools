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
    private IVerzinsungs_Service _verzingsungsSvc;
    public string Aktenzeichen { get; set; }
    public List<Einzelbuchung> BuchungsListe { get; private set; }
    public AktBuchungen(IVerzinsungs_Service zinsenSVC)
    {
      BuchungsListe = new();
      _verzingsungsSvc = zinsenSVC; 
    }

/*
    /// <summary>
    /// Diese Methode trägt eine Buchung in die Liste ein. Hierbei ist einiges zu beachten:
    /// der Betrag ist in der IKAROS Datenbank immer positiv, das ist aber zum Rechnen nicht
    /// sinnvoll. Daher muss ich ihn bei Zahlungen invertieren. 
    /// </summary>
    public void Add_Einzelbuchung(
      DateTime valutadatum,
      string kürzel, 
      string kurztext,
      decimal betrag,
      decimal kostenVerzinst, 
      decimal kostenUnverzinst, 
      decimal kostenZinsen, 
      decimal hauptforderung)
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
*/

    public void Add_Einzelbuchung(Einzelbuchung_DTO dto)
    {
      BuchungsListe.Add(new Einzelbuchung(dto));
      BuchungsListe.Sort();
    }



    public BuchungsSaldo SaldiereBuchungen()
    {
      DateTime letzeZahlung = DateTime.MinValue;
      DateTime letzteVerzinsung = this.Get_Datum_erste_Transaktion().AddDays(1);
      BuchungsSaldo saldo = new(this.BuchungsListe);

      foreach (Einzelbuchung eb in this.BuchungsListe) {
        //Wenn die einzelne Beträge 0 sind, dann gleich weiter
        if (eb.Umsatz == decimal.Zero) {
          continue;
        }
        if (eb.Valutadatum == new DateTime(2012, 01, 03)) {
          Debug.WriteLine("Trigger");
        }

        //bei Akten mit Sub-Akt gibt es wohl alle Zahlungen mehrfach, 
        //ich schmeisse mal die ZU Buchungen weg..
        if (eb.Kürzel.Equals("ZU")) {
          continue;
        }
        if (eb.Zinsen < Decimal.Zero) {
          Debug.WriteLine("TriggerWarnung");
          List<Einzelbuchung> zns = _verzingsungsSvc.Calculate_Zinsen(
            saldo.Hauptforderungen_Offen, 
            letzteVerzinsung.AddDays(1), 
            eb.Valutadatum);
          decimal zinsenBelastung = decimal.Zero;
          foreach(Einzelbuchung e in zns) {
            zinsenBelastung += e.Zinsen;
          }
          //setzen des neuen Datums und belasten der Zinsen
          letzteVerzinsung = eb.Valutadatum.AddDays(1);
          saldo.Zinsen += zinsenBelastung;
          saldo.Umsatz += zinsenBelastung;

          //Entscheidung ob Zahlung, und wenn ja DatumletzteZahlung aktualisieren
          if (eb.Kürzel.StartsWith("Z001")) {
            letzeZahlung = letzeZahlung >  eb.Valutadatum ? letzeZahlung : eb.Valutadatum;
          }
          if (eb.Zinsen != decimal.Zero) {
            letzteVerzinsung = eb.Valutadatum;
          }
        }
        saldo.Umsatz += eb.Umsatz;
        saldo.Kosten_Unverzinslich += eb.Kosten_Unverzinslich;
        saldo.Kosten_Verzinslich += eb.Kosten_Verzinslich;
        saldo.Zinsen += eb.Zinsen;
        saldo.Kapital_Buchen(eb.Hauptforderung);
      }
      //jetzt bin ich fast fertig, ich muss nur noch die Zinsen ab der letzten 
      //verzinsung berechnen und zu den Zinsen addieren
      if (letzteVerzinsung < DateTime.Now) {
        //nachverzinsen
        List<Einzelbuchung> zns = _verzingsungsSvc.Calculate_Zinsen(
          saldo.Hauptforderungen_Offen,
          letzteVerzinsung,
          DateTime.Now);
        foreach(Einzelbuchung zeb in zns) {
          saldo.Zinsen += zeb.Zinsen;
          saldo.Umsatz += zeb.Umsatz;
        }

      }
      saldo.Letzte_Zahlung_Am = letzeZahlung;

      return saldo;
    }

    private DateTime Get_Datum_erste_Transaktion()
    {
      DateTime d = DateTime.MaxValue;
      foreach(Einzelbuchung e in this.BuchungsListe) {
        if (d > e.Valutadatum) { d = e.Valutadatum;}
      }
      return d;
    }

  } //end    public class AktBuchungen

} //end namespace Splitbuchungen_Auflösen.Models

