using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Dokumente_2_Subito.Infrastructure.Interfaces;
using Dokumente_2_Subito.DataServices.Interfaces;
using Dokumente_2_Subito.Models;


namespace Dokumente_2_Subito.DataServices
{
  public class IkarosDataService : IDisposable, IIkarosDataService
  {
    private IConfigProvider _config;
    private OdbcConnection _connection;

    public IkarosDataService(IConfigProvider config)
    {
      _config = config;
      _connection = new OdbcConnection(_config.ConnectionString_ASA);
    }

    public void Dispose()
    {
      _connection.Close();
    }

    public IEnumerable<Ikaros_Document_Item_DTO> RetrieveAll()
    {
      string query = """
        SELECT  
          a.Az AS Aktenzeichen 
          ,a.UnterAkte AS UnterAktNr
          ,k.Name1 AS Gläubiger
          ,k.Notizen AS GläubigerNotizen
          ,k2.Suchbegriff AS Schuldner
          ,k2.Notizen AS SchuldnerNotizen
          ,adr.AuslandsKz AS SchuldnerLand
          ,v.Datum AS Datum 
          ,vv.Kuerzel AS Kürzel
          ,v.Kurztext AS Kurztext
          ,v.Betrag AS Betrag
          ,v.Bemerkung AS Bemerkung
          ,CASE
             WHEN (SELECT d.Dok_ID FROM Akte a JOIN Vorgang v2 ON v.Akte_ID = a.Akte_ID JOIN Dokument d ON d.RefID = v.Vg_ID WHERE d.RefTabelle = 'Vorgang' AND d.Speicherungsart = 'F' AND v.Vg_ID = v2.Vg_ID) IS NOT NULL 
             THEN 'C:\ExterneDokumente\' + RIGHT((SELECT d.Dok_ID FROM Akte a JOIN Vorgang v2 ON v.Akte_ID = a.Akte_ID JOIN Dokument d ON d.RefID = v.Vg_ID WHERE d.RefTabelle = 'Vorgang' AND d.Speicherungsart = 'F' AND v.Vg_ID = v2.Vg_ID), 3) + '\'
             ELSE NULL 
           END AS Filepath
          ,(SELECT d.Dok_ID FROM Akte a JOIN Vorgang v2 ON v.Akte_ID = a.Akte_ID JOIN Dokument d ON d.RefID = v.Vg_ID WHERE d.RefTabelle = 'Vorgang' AND d.Speicherungsart = 'F' AND v.Vg_ID = v2.Vg_ID) AS Filename
          ,(SELECT d.Extension FROM Akte a JOIN Vorgang v2 ON v.Akte_ID = a.Akte_ID JOIN Dokument d ON d.RefID = v.Vg_ID WHERE d.RefTabelle = 'Vorgang' AND d.Speicherungsart = 'F' AND v.Vg_ID = v2.Vg_ID) AS Filetype
          ,a.Notizen AS AktenNotiz
        FROM Akte a
          JOIN Vorgang v ON v.Akte_ID = a.Akte_ID
          JOIN VgVorlage vv ON vv.VgVorl_ID = v.VgVorl_ID
          JOIN Kontakt k ON k.Kontakt_ID = a.Mandant_ID
          JOIN Kontakt k2 ON k2.Kontakt_ID = a.Schuldner_ID
          JOIN Adresse adr ON k2.Kontakt_ID = adr.Kontakt_ID
        WHERE
          a.Az IN ('20160005073', '20140000885', '20170014007')
        ORDER BY a.Az, v.Datum, v.lupdate;
        """;
      if (_connection.State != ConnectionState.Open) {
        _connection.Open();
      }
      using (OdbcCommand cmd = new OdbcCommand(query, _connection)) {
        using (OdbcDataReader reader = cmd.ExecuteReader()) {
          while (reader.Read()) {
            Ikaros_Document_Item_DTO ikarosDTO = new Ikaros_Document_Item_DTO(reader);
            yield return ikarosDTO;
          }
        }
      }
    }

  } //end   public class IkarosDataService : IDisposable, IIkarosDataService

} //end namespace Dokumente_2_Subito.DataServices



