using Org.BouncyCastle.Bcpg.Sig;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dokumente_2_Subito.Models
{
  /// <summary>
  /// Diese Klasse stellt die Verbindung zwischen IkarosAkt und SubitoAkt
  /// her. Ich habe hier folgenden Kunstgriff implementiert: 
  /// In Ikaros gibt
  /// es eine Aktnummer als String und dann eine Integer mit einem Index 
  /// grössser gleich 0. 
  /// Sub-Akte haben einen Index > 0. 
  /// In Subito gibt es Akte mit /0 am Ende oder /1..n 
  /// Diese /1..n sind Sub-Akte. 
  /// Um ein Mapping zu ermöglichen, appende ich an die Ikaros Aktnummer
  /// immer /0 oder /n, damit das Mapping zwischen den Haupt-Und Subakten
  /// stimmt.
  /// </summary>
  public class Mapping_Ikaros_Subito
  {
    public string IkarosAnr { get; set; }
    public string SubitoAnr { get; set; }
    public DateTime DatumErfassung { get; set; }
    public string Gläubiger { get; set; }
    public string Schuldner { get; set; }
    public Mapping_Ikaros_Subito() { }
    public Mapping_Ikaros_Subito(IDataReader rdr)
    {
      this.IkarosAnr = rdr["IkarosAnr"].To<string>();
      this.SubitoAnr = rdr["SubitoAnr"].To<string>() ;
      this.DatumErfassung = rdr["DatumErfassung"].To<DateTime>();
      this.Gläubiger = rdr["Glaeubiger"].To<string>();
      this.Schuldner = rdr["Schuldner"].To<string>();
      //Berechnung Postfix von IkarosAnr aus SubitoAnr
      if (_subaktRegex.IsMatch(this.SubitoAnr)) {
        Match match = _subaktRegex.Match(this.SubitoAnr);
        string subaktIndex = match.Groups["IND"].Value;
        this.IkarosAnr = this.IkarosAnr + "/" + subaktIndex;
      }
    }

    private static Regex _subaktRegex = new Regex(@"^[0123456789/]+/(?<IND>\d+)$");

  }  //end  public class Mapping_Ikaros_Subito

} //end namespace Dokumente_2_Subito.Models

//   [0123456789/]+/(?<IND>\d+)