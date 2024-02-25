using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

using Dokumente_2_Subito.Infrastructure.Interfaces;
using Dokumente_2_Subito.Models;
using Dokumente_2_Subito.DataServices.Interfaces;

namespace Dokumente_2_Subito.DataServices
{
  public class MssqlDataService : IDisposable, IMssqlDataService
  {
    private IConfigProvider _config;
    private IDbConnection _connection;

    public MssqlDataService(IConfigProvider config)
    {
      _config = config;
      _connection = new SqlConnection(_config.ConnectionString_MSSQL);
    }

    public void Dispose()
    {
      _connection.Close();
    }

    public IEnumerable<Mapping_Ikaros_Subito> RetrieveAll()
    {
      string query = """
        SELECT * FROM mapping_ikaros_subito;
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


