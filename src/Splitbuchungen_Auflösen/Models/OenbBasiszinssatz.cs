using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splitbuchungen_Auflösen.Models
{
  /// <summary>
  /// Diese Klasse kapselt die Information des Basiszinssatzes, wie
  /// er von der OENB publiziert wird
  /// </summary>
  public class OenbBasiszinssatz : IComparable<OenbBasiszinssatz>
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


} //end namespace Splitbuchungen_Auflösen.Models


