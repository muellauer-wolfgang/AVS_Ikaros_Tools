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
      if (!string.IsNullOrEmpty(Aktenzeichen) && Aktenzeichen.Equals("2023000326")) {
        Debug.WriteLine("Trigger");

      }

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
        //Erkennen der Buchungen für Spesen Auftraggeber gesondert saldieren
        if (eb.Kürzel.Equals("K0003") &&  eb.Kurztext.Contains("Auftraggeber")) {
          saldo.Spesen_Auftraggeber += eb.Kosten_Unverzinslich;
          Debug.WriteLine("Spesen Auftraggeber");
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
      //es wird bis 2024-01-31 verzinst
      if (letzteVerzinsung < new DateTime(2024, 01,31)) {
        //nachverzinsen
        List<Einzelbuchung> zns = _verzingsungsSvc.Calculate_Zinsen(
          saldo.Hauptforderungen_Offen,
          letzteVerzinsung,
          new DateTime(2024, 01, 31));
        foreach(Einzelbuchung zeb in zns) {
          saldo.Zinsen += zeb.Zinsen;
          saldo.Umsatz += zeb.Umsatz;
        }

      }
      saldo.Letzte_Zahlung_Am = letzeZahlung;

      //jetzt führe ich das alles ad absudrum und schau mal nach, 
      //was die Datenbank liefert.
      BuchungsSaldo s = _verzingsungsSvc.CalcSaldo(Aktenzeichen, new DateTime(2024, 01, 31));
      saldo.Umsatz = s.Umsatz;
      saldo.Kosten_Unverzinslich = s.Kosten_Unverzinslich;
      saldo.Kosten_Verzinslich = s.Kosten_Verzinslich;
      saldo.Spesen_Auftraggeber_Abgerechnet = s.Spesen_Auftraggeber_Abgerechnet;
      saldo.Zinsen = s.Zinsen;
      saldo.Hauptforderung = s.Hauptforderung;
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

