using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Splitbuchungen_Auflösen.DataServices.Interfaces;
using Splitbuchungen_Auflösen.Models;

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

    public BuchungsSaldo CalcSaldo(string aktenzeichen, DateTime bis)
    {
      BuchungsSaldo saldo = new BuchungsSaldo();
      string query01 = $"SELECT AkteGibRestforderung('{aktenzeichen}',0,'{bis:yyyy-MM-dd}');";
      string query02 = $"SELECT AkteSummiereRestGlKosten('{aktenzeichen}') AS 'GLKOSTEN';";
      string query03 = 
        "SELECT " +
        " fordKostenUnverz AS \"Kosten_Unverzinslich\" " +
        " ,fordKostenVerz AS \"Kosten_Verzinslich\" " +
        " ,fordZinsHf AS \"Zinsen\" " +
        " ,fordHf AS \"Hauptforderung\" " +
        " FROM Forderungsberechnung; ";
      string query04 =
        "SELECT " +
        "MIN (a.Az) AS \"AktID\" " +
        ",SUM (b.Betrag) AS \"SpesenAgGetilgt\" " +
        "FROM " +
        "Akte a " +
        "INNER JOIN Vorgang v ON v.Akte_ID = a.Akte_ID " +
        "JOIN VgVorlage vv ON vv.VgVorl_ID = v.VgVorl_ID " +
        "JOIN Buchung b on b.Vg_ID = v.Vg_ID " +
        "JOIN AnsprKto an ON an.AnsprKto_ID = b.ANsprKto_ID " +
        $"WHERE a.Az = {aktenzeichen} AND v.Variante IN ('E') " +
        "AND an.Bezeichnung = 'Gl.-Kosten' " ;

      if (_connection.State != ConnectionState.Open) {
        _connection.Open();
      }
      using (OdbcCommand cmd = new OdbcCommand(query01, _connection)) {
        using (OdbcDataReader reader = cmd.ExecuteReader()) {
          if (reader.Read()) {
            saldo.Umsatz = reader.GetDecimal(0);
          } 
        }
      }
/*
      using (OdbcCommand cmd = new OdbcCommand(query02, _connection)) {
        using (OdbcDataReader reader = cmd.ExecuteReader()) {
          if (reader.Read()) {
            decimal x = reader.GetDecimal(0);
          }
        }
      }
*/
      using (OdbcCommand cmd = new OdbcCommand(query03, _connection)) {
        using (OdbcDataReader reader = cmd.ExecuteReader()) {
          if (reader.Read()) {
            saldo.Kosten_Unverzinslich = reader.GetDecimal(0);
            saldo.Kosten_Verzinslich = reader.GetDecimal(1);
            saldo.Zinsen = reader.GetDecimal(2);
            saldo.Hauptforderung = reader.GetDecimal(3);
          } 
        }
      }

      using (OdbcCommand cmd = new OdbcCommand(query04, _connection)) {
        using (OdbcDataReader reader = cmd.ExecuteReader()) {
          if (reader.Read()) {
            if (!reader.IsDBNull(1)) {
              saldo.Spesen_Auftraggeber_Abgerechnet = reader.GetDecimal(1);
            }
          }
        }
      }
      return saldo;
    }


    /*
-- AND v.Vg_ID = 685000093412
    */



    public void Dispose()
    {
      _connection.Close();
    }

  } //end   public  class SQL_Anywhere_Service

} //end namespace Splitbuchungen_Auflösen.DataServices

