using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Dokumente_2_Subito.Infrastructure.Interfaces;
using Dokumente_2_Subito.Models;
using Dokumente_2_Subito.DataServices.Interfaces;

namespace Dokumente_2_Subito.DataServices
{
  public class MySqlDataService : IDisposable, IMySqlDataService
  {
    private IConfigProvider _config;
    private IDbConnection _connection;

    public MySqlDataService(IConfigProvider config)
    {
      _config = config;
      _connection = new MySqlConnection(_config.ConnectionString_MySql);
    }

    public void Dispose()
    {
      _connection.Close();
    }

    public IEnumerable<Mapping_Ikaros_Subito> RetrieveAll()
    {
      string query = """
        SELECT  
           dyn_feld.WERT AS "IkarosAnr"
          ,ink_akte.AKTENZEICHEN AS "SubitoAnr"
          ,DATE_FORMAT(ink_akte.ANGELEGT_AM, '%Y-%m-%d') AS "DatumErfassung"
          ,p1.NAME AS "Glaeubiger"
          ,p2.NAME AS "Schuldner"
        FROM ink_akte 
        INNER JOIN dyn_feld ON ink_akte.ID = dyn_feld.REFERENZ_OBJEKT_ID AND dyn_feld.ADM_DYN_FELD_ID = 633819
        INNER JOIN ink_beteiligte_person ON ink_akte.ID = ink_beteiligte_person.ink_akte_id
        INNER JOIN party p1  ON ink_beteiligte_person.PARTY_ID = p1.ID
        INNER JOIN schuldner ON ink_akte.SCHULDNER_ID = schuldner.ID
        INNER JOIN party p2  ON schuldner.PARTY_ID =  p2.ID

        WHERE  
        dyn_feld.WERT <> '-'
        AND ink_akte.GUELTIG_BIS = '9999-12-31 23:59:59.000000'
        ORDER BY dyn_feld.WERT, ink_akte.angelegt_am   ;
        """;
      if (_connection.State != ConnectionState.Open) {
        _connection.Open();
      }
      using (IDbCommand cmd = _connection.CreateCommand()) {
        cmd.CommandText = query;
        using (IDataReader reader = cmd.ExecuteReader()) {
          while (reader.Read()) {
            Mapping_Ikaros_Subito mapping = new Mapping_Ikaros_Subito(reader);
            yield return mapping;
          }
        }
      }
    }

  } //end   public class MssqlDataServic : IDisposable, IMssqlDataService

} //end namespace Dokumente_2_Subito.DataServices.Interfaces


