using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Splitbuchungen_Auflösen.Models;

namespace Splitbuchungen_Auflösen.Services
{
  public class Verzinsungs_Service
  {
    private static List<OenbBasiszinssatz> _zinssatzTabelle;
    private static HashSet<int> _monate_31_tage;
    private static HashSet<int> _monate_30_tage;

    /// <summary>
    /// Diese Methode errechnet die Zinsbelastung für dieses Kapital von bis und
    /// zu den übergebenen Konditionen. Wenn im Intervall ein Sprung in den Zinsen
    /// ist, werden zwei Buchungen erstellt.
    /// </summary>
    /// <param name="zinsInfo"></param>
    /// <param name="betrag"></param>
    /// <param name="von"></param>
    /// <param name="bis"></param>
    /// <returns></returns>
    public List<Einzelbuchung> Calculate_Zinsen(decimal betrag, DateTime von, DateTime bis, Hauptforderung_Verzinsung zinsInfo)
    {
      List<Einzelbuchung> zinsbelastungen = new();
      //Kontrolle Plausibilität der Parameter
      if (betrag <= decimal.Zero) { return zinsbelastungen; }
      if (von >= bis) { return zinsbelastungen; } 

      if (Intervall_in_zwei_Zinsperioden(von, bis) == false ) {
        //normale Zinsberechnung, alles in einer Periode
        decimal effektiveZinsen;
        int kompensationstage = Calculate_Kompensationstage(von, bis);

        OenbBasiszinssatz basiszins = this.Get_Valid_OentBasiszinssatz(von, bis);
        if (zinsInfo.Zinsart.Equals("Basis")) {
          effektiveZinsen = basiszins.Zinssatz + zinsInfo.Zinssatz;
        } else if (zinsInfo.Zinsart.Equals("Fix")) {
          effektiveZinsen = zinsInfo.Zinssatz;
        } else {
          effektiveZinsen = 5M;
        }
        TimeSpan diffInDays = bis - von;
        decimal zinstage = new decimal(diffInDays.TotalDays) + 1M;
        decimal zinsenInMoney = betrag * (effektiveZinsen / 100M)  * (zinstage / 360M);
        Einzelbuchung ebZinsenbelastung = new Einzelbuchung {
          Valutadatum = bis,
          Kürzel = "K0099",
          Kurztext  = "Zinsbelastung",
          Betrag = zinsenInMoney,
          Kosten_Zinsen = zinsenInMoney
        };
        zinsbelastungen.Add(ebZinsenbelastung);
      } else {
        //Splitten der Perioden, und dann rekursiv in die Berechnung
        DateTime stichtag = Calculate_first_Zinsänderungs_Day(von, bis);
        List<Einzelbuchung> list1 = Calculate_Zinsen(betrag, von, stichtag.AddDays(-1), zinsInfo);
        List<Einzelbuchung> list2 = Calculate_Zinsen(betrag, stichtag, bis, zinsInfo);
        List<Einzelbuchung> newList = new List<Einzelbuchung>();
        newList.AddRange(list1);
        newList.AddRange(list2);
        return newList;
      }
      return zinsbelastungen;
    }


    /// <summary>
    /// Ich habe händisch mit Excel ermittelt, dass ich den Zins-Algorithmus von 
    /// Ikaros nachbilden kann, wenn ich Kompensationstage ausrechne, die 
    /// die 30-er und 31-er Monate ausgleichen. Und für jedes Jahr geht das 
    /// mit 12 Tagen pro Jahr. Ich glaube, das ist ein Bug, aber was solls, ich
    /// probiere es.....
    /// </summary>
    /// <param name="von"></param>
    /// <param name="bis"></param>
    /// <returns></returns>
    private int Calculate_Kompensationstage(DateTime von, DateTime bis)
    {
      int kompensation = 0;
      //im ersten Schrit berechne ich die Zahl der Kompensations-Tage.
      int abstandInJahren = bis.Year - von.Year;
      if (abstandInJahren >= 2) {
        kompensation -= abstandInJahren * 12;
      } else if (abstandInJahren < 1 ) {
        if (!_monate_30_tage.Contains(von.Month)) {
          kompensation --;
        }
      }  else  {
        DateTime datumLaufend = new DateTime(von.Year, von.Month, 1);
        DateTime enddatum = new DateTime(bis.Year, bis.Month, 1);
        while (datumLaufend < enddatum) {
          if (_monate_31_tage.Contains(datumLaufend.Month)) {
            kompensation--;
          }
          datumLaufend = datumLaufend.AddMonths(1);
        }
      }
      return kompensation;
    }



    private bool Intervall_in_zwei_Zinsperioden(DateTime von, DateTime bis)
    {
      foreach(OenbBasiszinssatz z in _zinssatzTabelle) {
        if (von < z.Stichtag && z.Stichtag < bis) {
          return true;
        }
      }
      return false;
    }

    private DateTime Calculate_first_Zinsänderungs_Day(DateTime von, DateTime bis)
    {
      foreach (OenbBasiszinssatz z in _zinssatzTabelle) {
        if (von < z.Stichtag && z.Stichtag < bis) {
          return z.Stichtag;
        }
      }
      return bis;
    }

    private OenbBasiszinssatz Get_Valid_OentBasiszinssatz(DateTime von, DateTime bis)
    {
      for (int i = _zinssatzTabelle.Count - 1; i >= 0; i--) {
        if (von >= _zinssatzTabelle[i].Stichtag && bis >= _zinssatzTabelle[i].Stichtag) {
          return _zinssatzTabelle[i];
        }
      }
      return _zinssatzTabelle.Last();
    }

    static  Verzinsungs_Service()
    {
      _zinssatzTabelle = new List<OenbBasiszinssatz> {
        new OenbBasiszinssatz(1970, 01, 01, 5.00M),
        new OenbBasiszinssatz(2002, 07, 01, 2.75M),
        new OenbBasiszinssatz(2003, 01, 01, 2.20M),
        new OenbBasiszinssatz(2003, 07, 01, 1.47M),
        new OenbBasiszinssatz(2006, 07, 01, 1.97M),
        new OenbBasiszinssatz(2007, 01, 01, 2.67M),
        new OenbBasiszinssatz(2007, 07, 01, 3.19M),
        new OenbBasiszinssatz(2009, 01, 01, 1.88M),
        new OenbBasiszinssatz(2009, 07, 01, 0.38M),
        new OenbBasiszinssatz(2013, 07, 01, -0.12M),
        new OenbBasiszinssatz(2016, 07, 01, -0.62M),
        new OenbBasiszinssatz(2023, 01, 01, 1.88M),
        new OenbBasiszinssatz(2023, 07, 01, 3.38M),
      };
      _zinssatzTabelle.Sort();
      _monate_31_tage = new HashSet<int> { 1, 3, 5, 7, 8, 10, 12 };
      _monate_30_tage = new HashSet<int> { 2, 4, 6, 9, 11 };
    }

    /// <summary>
    /// Diese Klasse kapselt die Information des Basiszinssatzes, wie
    /// er von der OENB publiziert wird
    /// </summary>
    private class OenbBasiszinssatz : IComparable<OenbBasiszinssatz>
    {
      public DateTime Stichtag;
      public decimal Zinssatz;
      public OenbBasiszinssatz(int y, int m, int d, decimal zinssatz)
      {
        this.Stichtag = new DateTime(y, m, d);
        this.Zinssatz = zinssatz;
      }

      public int CompareTo(OenbBasiszinssatz other)
      {
        return this.Stichtag.CompareTo(other.Stichtag);
      }

      public override string ToString()
      {
        return $"Datum:{Stichtag:yyyy-MM-dd} Zins:{Zinssatz:00.00}";
      }

    } //end  private class OenbBasiszinssatz : IComparable<OenbBasiszinssatz>

  } //end   public class Verzinsungs_Service

} //end namespace Splitbuchungen_Auflösen.Services

