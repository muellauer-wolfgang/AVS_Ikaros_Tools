using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Splitbuchungen_Auflösen.DataServices.Interfaces;

namespace Splitbuchungen_Auflösen.DataServices
{

  /// <summary>
  /// Diese Klasse ist eine einfach Facade in den SQL Anywhere 
  /// Server, wo es eine embedded Java Procedure für das Berechnen
  /// von Zinsen gibt. 
  /// </summary>
  public class SQL_Anywhere_Service : IDisposable, ISQL_Anywhere_Service
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

    public void Dispose()
    {
      _connection.Close();
    }

  } //end   public  class SQL_Anywhere_Service

} //end namespace Splitbuchungen_Auflösen.DataServices

