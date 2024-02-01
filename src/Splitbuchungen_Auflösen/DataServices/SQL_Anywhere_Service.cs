using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Splitbuchungen_Auflösen.DataServices
{

  /// <summary>
  /// Diese Klasse ist ein dummer Spike, mit dem ich auf ganz einfache
  /// Weise Verbindung mit der SQL_Anywhere Datenbank aufbauen will.
  /// Mal sehen, ob ich das so hinrotzen kann....
  /// </summary>
  public class SQL_Anywhere_Service : IDisposable
  {
    private OdbcConnection _connection;

    public SQL_Anywhere_Service()
    {
      _connection = new OdbcConnection("DSN=IKAROS-VM");
      _connection.Open();
    }

    /// <summary>
    /// Diese Methode berechnet die Zinsen mit einer stored Procedure
    /// </summary>
    /// <returns></returns>
    public decimal CalcZinsen(decimal betrag, decimal zinsatz, DateTime von, DateTime bis)
    {
      string betragAsString = XmlConvert.ToString(betrag);
      string zinsatzAsString = XmlConvert.ToString(zinsatz);

      string query = $"SELECT berechneZins( {betragAsString}, {zinsatzAsString}, '{von:yyyy-MM-dd}', '{bis:yyyy-MM-dd}');";
      if (_connection.State != ConnectionState.Open) {
        _connection.Open();
      }
      using (OdbcCommand cmd = new OdbcCommand(query, _connection)) {
        using (OdbcDataReader reader = cmd.ExecuteReader()) {
          if (reader.Read()) { 
            decimal zinsen = reader.GetDecimal(0);
            return zinsen;
          } else {
            return decimal.Zero;
          }

        }
      }
    }

 /*
  * using (OdbcConnection connection = new OdbcConnection("DSN=IKAROS-VM")) {
        string sqlQuery = """
          SELECT berechneZins(4873.25,4.38,'2016-09-20','2018-04-17')
          FROM Akte a
          JOIN Vorgang v ON v.Akte_ID = a.Akte_ID
          JOIN VgVorlage vv ON vv.VgVorl_ID = v.VgVorl_ID
          JOIN Kontakt k ON k.Kontakt_ID = a.Schuldner_ID
          JOIN Zinsen z ON z.Vg_ID = v.Vg_ID
          WHERE 
          a.Az = 20160012675
          AND v.Variante NOT IN ('E','F')
          """;
        OdbcCommand command = new OdbcCommand(sqlQuery, connection);
        connection.Open();
        OdbcDataReader reader = command.ExecuteReader();
        if (reader.Read()) {
          double z = reader.GetDouble(0);
        }
        reader.Close();
        command.Dispose();
      }
    }
 */

    public void Dispose()
    {
      _connection.Close();
    }

  } //end   public  class SQL_Anywhere_Service

} //end namespace Splitbuchungen_Auflösen.DataServices

