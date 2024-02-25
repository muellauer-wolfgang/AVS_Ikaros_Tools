using Autofac;
using Dokumente_2_Subito.Infrastructure.Interfaces;
using Dokumente_2_Subito.Startup;
using Dokumente_2_Subito.Models;
using Dokumente_2_Subito.DataServices.Interfaces;
using System.Text;

namespace Dokumente_2_Subito
{
  internal class Program
  {
    public static IContainer _container;
    public static IConfigProvider _config;
    public static List<Ikaros_Document_Item_DTO> _ikarosItemsListe = new();
    public static List<Mapping_Ikaros_Subito> _mappingListe = new();
    public static Dictionary<string, Mapping_Ikaros_Subito> _ikaros_2_subito_lookup = new();
    public static Dictionary<string, SubitoAkt> _subitoAktDict = new();
    public static Dictionary<string, Gläubiger> _gläubigerDict = new();
    public static Dictionary<string, List<Ikaros_Document_Item_DTO>> _ikarosOrphanItemsDict = new();
    public static Dictionary<string, Ikaros_Document_Item_DTO> _ikarosMappedItemsDict = new();
    public static List<Ikaros_Document_Item_DTO> _ItemsOhneMapping = new(); 
    static void Main(string[] args)
    {
      _container = new Bootstrapper().Bootstrap();
      _config = _container.Resolve<IConfigProvider>();

      IIkarosDataService ikarosDB = _container.Resolve<IIkarosDataService>();
      IMssqlDataService mssqlDB = _container.Resolve<IMssqlDataService>();

      Console.WriteLine("Dokumente_2_Subito");
      Console.WriteLine("Konsolidierung von Dokument-Daten für Übertragung nach Subito");

      Console.WriteLine("Lesen Mapping Subito <--> Ikaros");
      foreach (Mapping_Ikaros_Subito mis in mssqlDB.RetrieveAll()) {
        _mappingListe.Add(mis);
      }
      Console.WriteLine($"Anzahl Mapping Elemente: {_mappingListe.Count}");
      //im ersten Schritt erzeuge ich das Mapping-Dict, das mir hilft, 
      //aus der Ikaros-Aktnummer den Subito Akt zu finden.

      List<Mapping_Ikaros_Subito> doublettenListe = new();

      int map_ikaros_2_subito_errorCnt = 0;
      foreach (Mapping_Ikaros_Subito m in _mappingListe) {
        if (!m.SubitoAnr.EndsWith("/0")) {
          continue;
        }
        if (!_ikaros_2_subito_lookup.ContainsKey(m.IkarosAnr)) {
          _ikaros_2_subito_lookup.Add(m.IkarosAnr, m);
        } else {
          map_ikaros_2_subito_errorCnt++;
          doublettenListe.Add(_ikaros_2_subito_lookup[m.IkarosAnr]);
          doublettenListe.Add(m);
          //Console.WriteLine("Mapping_Ikaros_Subito 1:1 sollte eindeutig sein");
        }
      }
      Console.WriteLine($"Anzahl Fehler Mapping_Ikaros_Subito 1:1 {map_ikaros_2_subito_errorCnt}");
      int doubletten_subito_akteCnt = 0;
      //Dann lege ich alle Subito Akte an und organsiere sie über einen Dict
      foreach (Mapping_Ikaros_Subito m in _mappingListe) {
        if (!_subitoAktDict.ContainsKey(m.SubitoAnr)) {
          _subitoAktDict.Add(m.SubitoAnr, new SubitoAkt());
        } else {
          doubletten_subito_akteCnt++;
        }
      }
      Console.WriteLine($"Doubletten Subito Akte laut Mapping {doubletten_subito_akteCnt}");


      Console.WriteLine("Lesen Ikaros Item DTOs");
      string assigendSubitoAnr;
      foreach (Ikaros_Document_Item_DTO ik in ikarosDB.RetrieveAll() ) {
        assigendSubitoAnr = string.Empty;
 
        _ikarosItemsListe.Add(ik);
        if (!_gläubigerDict.ContainsKey(ik.GläubigerName)) {
          _gläubigerDict[ik.GläubigerName] = new Gläubiger {Name = ik.GläubigerName };
        }
        if (!_ikaros_2_subito_lookup.ContainsKey(ik.IkarosAnr)) {
          //ist ein Orphan, könnte Dadaj Akt sein, auf jeden Fall Orphan
          if (!_ikarosOrphanItemsDict.ContainsKey(ik.IkarosAnr)) {
            _ikarosOrphanItemsDict.Add(ik.IkarosAnr, new());
          }
          _ikarosOrphanItemsDict[ik.IkarosAnr].Add(ik);
        } else {
          _ikarosMappedItemsDict.TryAdd(ik.IkarosAnr, ik);
          assigendSubitoAnr = _ikaros_2_subito_lookup[ik.IkarosAnr].SubitoAnr;
        }
        if (!_subitoAktDict.ContainsKey(assigendSubitoAnr) ) {
          _subitoAktDict.Add(assigendSubitoAnr, new SubitoAkt());
        }
        _subitoAktDict[assigendSubitoAnr].IkarosItems.Add(ik);
        _gläubigerDict[ik.GläubigerName].Akten.Add(_subitoAktDict[assigendSubitoAnr]);
      }

      //Darstellen der Doubletten mit Daten aus Ikaros Akten, Interessant ist das land
      StringBuilder sb = new StringBuilder(); 
      for (int i = 0; i < doublettenListe.Count; i++) {
        if (_ikarosMappedItemsDict.ContainsKey(doublettenListe[i].IkarosAnr)) {
          Console.WriteLine($"IkarosAkt gefunden");
        } else {
          Console.WriteLine($"IkarosAkt NICHT gefunden");

        }
        SubitoAkt sa = _subitoAktDict[doublettenListe[i].SubitoAnr];
        Ikaros_Document_Item_DTO ik = _ikarosMappedItemsDict[doublettenListe[i].IkarosAnr];
        Console.WriteLine($"SAnr: {sa.SubitoAnr} IAnr:{ik.IkarosAnr} IS:{ik.SchuldnerName} ./. IG: {ik.GläubigerName} SLand:{ik.SchuldnerLand}");
        sb.AppendLine($"{ik.IkarosAnr};{doublettenListe[i].SubitoAnr};{ik.Datum};{ik.GläubigerName};{ik.SchuldnerName};{ik.SchuldnerLand}");


      }
      Console.WriteLine($"Anzahl Ikaros Elemente: {_ikarosItemsListe.Count}");
      Console.WriteLine($"Anzahl Gläubiger: {_gläubigerDict.Count}");
      Console.WriteLine($"Anzahl Subito Akte: {_subitoAktDict.Count}");
      Console.WriteLine($"Anzahl Orphan Akte: {_ikarosOrphanItemsDict.Count()}");

      File.WriteAllText(@"h:\tmp\avs\Subito_Doubletten.csv", sb.ToString());  

    } //end     static void Main(string[] args)

  } //end   internal class Program

} //end namespace Dokumente_2_Subito

