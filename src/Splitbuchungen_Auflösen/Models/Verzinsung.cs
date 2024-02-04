using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splitbuchungen_Auflösen.Models
{


  /// <summary>
  /// Jede Hauptforderung kann theoretisch eine individuelle Verinsung haben.
  /// Verwendet wird dieses Feature hauptsächlich bei Bank-Forderungen, die
  /// sich in der Regel in Kaptial-Forderung und Zins-Forderung unterteilen.
  /// Die Kapital-Forderung wird normal verzinst, aber die Forderung aus
  /// nicht entrichteten Verzugszinsen bleibt unverzinst.
  /// </summary>
  public class Verzinsung
  {
    public string Zinsart { get; set; }
    public decimal Zinssatz { get; set; }
    public decimal ZinsenAus { get; set; }
    public string Forderungsart { get; set; }
    public decimal Forderungsanteil { get; set; }
    public DateTime ZinsenAb { get; set; }

    /// <summary>
    /// default ctor
    /// </summary>
    public Verzinsung() { }

    /// <summary>
    /// Diese statischeMethode liefert ein sogenanntes Null-Objkekt.
    /// Mit diesem Objekt kann man ganz normal rechnent, es kommen 
    /// aber immer nur Zinsen in der Höhe von 0.0M heraus
    /// </summary>
    /// <returns></returns>
    public static Verzinsung NullZinsen()
    {
      return new Verzinsung {
        Zinsart = string.Empty,
        Zinssatz = 0.0M,
        ZinsenAus = 0.0M,
        Forderungsart = string.Empty,
        Forderungsanteil = 1M,
        ZinsenAb = DateTime.MinValue
      };
    }

  } //end   public class Verzinsung

} //end namespace Splitbuchungen_Auflösen.Models

