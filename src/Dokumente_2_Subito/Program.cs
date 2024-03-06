using Autofac;
using Dokumente_2_Subito.Infrastructure.Interfaces;
using Dokumente_2_Subito.Startup;
using Dokumente_2_Subito.Models;
using Dokumente_2_Subito.DataServices.Interfaces;
using System.Text;
using System.Collections.Generic;
using ZstdSharp.Unsafe;
using System.Diagnostics;

namespace Dokumente_2_Subito
{
  internal class Program
  {
    public static IContainer _container;
    public static IConfigProvider _config;
    //Daten, die direkt aus den Datenbanken gelesen werden
    public static List<Ikaros_Document_Item_DTO> _ikarosItemsListe = new();
    public static List<Mapping_Ikaros_Subito> _mappingListe_IKAROS_2_SUBITO = new();

    //Maps, die aus den Rohdaten berechnet werden
    public static Dictionary<string, List<Mapping_Ikaros_Subito>> _ikaros_2_subito_lookup = new();
    public static Dictionary<string, TransferContainer> _tranferContainerDict = new();
    public static Dictionary<string, Gläubiger> _gläubigerDict = new();
    public static Dictionary<string, List<Ikaros_Document_Item_DTO>> _ikarosOrphanItemsDict = new();
    public static Dictionary<string, List<Ikaros_Document_Item_DTO>> _ikarosMappedItemsDict = new();


    public static List<Ikaros_Document_Item_DTO> _ItemsOhneMapping = new(); 
    static void Main(string[] args)
    {
      _container = new Bootstrapper().Bootstrap();
      _config = _container.Resolve<IConfigProvider>();

      IIkarosDataService ikarosDB = _container.Resolve<IIkarosDataService>();
      IMySqlDataService  mysqlDB  = _container.Resolve<IMySqlDataService>();

      Console.WriteLine("Dokumente_2_Subito");
      Console.WriteLine("Konsolidierung von Dokument-Daten für Übertragung nach Subito");

      Console.WriteLine("Lesen Mapping Subito <--> Ikaros aus SUBITO DB");
      foreach (Mapping_Ikaros_Subito mis in mysqlDB.RetrieveAll()) {
        _mappingListe_IKAROS_2_SUBITO.Add(mis);
      }
      Console.WriteLine($"Anzahl Mapping Elemente: {_mappingListe_IKAROS_2_SUBITO.Count}");

      //im ersten Schritt erzeuge ich das Mapping-Dict, das mir hilft, 
      //aus der Ikaros-Aktnummer die Subito Akten zu finden.
      foreach (Mapping_Ikaros_Subito m in _mappingListe_IKAROS_2_SUBITO) {
        if (m.IkarosAnr.StartsWith("20160005073") 
            || m.IkarosAnr.StartsWith("20140000885") 
            || m.IkarosAnr.StartsWith("20160005073"))   {
          Debug.WriteLine("Trigger");
        }
        if (!_ikaros_2_subito_lookup.ContainsKey(m.IkarosAnr)) {
          _ikaros_2_subito_lookup.Add(m.IkarosAnr, new List<Mapping_Ikaros_Subito> {m});
        } else {
          _ikaros_2_subito_lookup[m.IkarosAnr].Add(m);
        }
      }
      Console.WriteLine($"Anzahl Ikaros Akte mit Mappings: {_ikaros_2_subito_lookup.Count}");

      //Dann lege ich alle Subito TransferContainer an und organsiere sie über einen Dict
      //hier ist es ein bisschen kompliziert, da es aus historischen Gründen zu einem 
      //Ikaros Akt mehrere Subito Akten gibt, IKAROS : SUBITO also ein 1:N Mapping ist
      int doubletten_subito_akteCnt = 0;
      foreach (Mapping_Ikaros_Subito m in _mappingListe_IKAROS_2_SUBITO) {
        if (!_tranferContainerDict.ContainsKey(m.IkarosAnr)) {
          _tranferContainerDict.Add(m.IkarosAnr, new TransferContainer(m.IkarosAnr,_config));
        } else {
          doubletten_subito_akteCnt++;
        }
      }
      Console.WriteLine($"Doubletten Transfer Container laut Mapping {doubletten_subito_akteCnt}");

      Console.WriteLine("Lesen Ikaros Item DTOs aus IKAROS DB");
      int itemCnt = 0;
      foreach (Ikaros_Document_Item_DTO ik in ikarosDB.RetrieveAll()) {
        itemCnt++;
        if (itemCnt % 1000 == 0) {
          Console.WriteLine($"Gelesen: {itemCnt}");
        }
        _ikarosItemsListe.Add(ik);
        //Gläubiger beim Vorbeigehen einlesen
        if (!_gläubigerDict.ContainsKey(ik.GläubigerName)) {
          _gläubigerDict[ik.GläubigerName] = new Gläubiger { Name = ik.GläubigerName };
        }
        //Zuweisen des Ikaros Tupel zu den Subito Akten oder den Orphans
        if (!_ikaros_2_subito_lookup.ContainsKey(ik.IkarosAnr)) {
          //ist ein Orphan, könnte Dadaj Akt sein, auf jeden Fall Orphan
          if (!_ikarosOrphanItemsDict.ContainsKey(ik.IkarosAnr)) {
            _ikarosOrphanItemsDict.Add(ik.IkarosAnr, new());
          }
          _ikarosOrphanItemsDict[ik.IkarosAnr].Add(ik);
        } else {
          if (!_ikarosMappedItemsDict.ContainsKey(ik.IkarosAnr)) {
            _ikarosMappedItemsDict.Add(ik.IkarosAnr, new());
          }
          _ikarosMappedItemsDict[ik.IkarosAnr].Add(ik); 
        }
        if (!_tranferContainerDict.ContainsKey(ik.IkarosAnr)) {
          _tranferContainerDict.Add(ik.IkarosAnr, new TransferContainer(ik.IkarosAnr, _config) );
        }
        _tranferContainerDict[ik.IkarosAnr].IkarosItemList.Add(ik);
      }

      //jetzte müsste für jeden Transfer Container gelten, dass alle DTOs drinnen 
      //die gleiche IKaros ANR haben und den gleichen Gläubiger
      Console.WriteLine("Beginn Kontrolle TransferContainers IkarosAnr, Gläubiger");
      foreach (TransferContainer tc in _tranferContainerDict.Values) {
        string iNr;
        string gl;
        if (tc.IkarosItemList.Count > 1) {
          iNr = tc.IkarosItemList[0].IkarosAnr;
          gl = tc.IkarosItemList[0].GläubigerName;
          tc.Gläubiger = gl;
          for (int i=1; i <tc.IkarosItemList.Count; i++) {
            if (!iNr.Equals(tc.IkarosItemList[i].IkarosAnr)) { 
              Console.WriteLine($"Fehler bei IkarosAnr: {iNr}"); 
            }
            if (!gl.Equals(tc.IkarosItemList[i].GläubigerName)) {
              Console.WriteLine($"Fehler bei Gläubiger: {gl}");
            }
          }
        }
      }
      Console.WriteLine("Ende Kontrolle TransferContainers IkarosAnr, Gläubiger");
      Console.WriteLine($"Anzahl Ikaros Elemente: {_ikarosItemsListe.Count}");
      Console.WriteLine($"Anzahl Gläubiger: {_gläubigerDict.Count}");
      Console.WriteLine($"Anzahl Subito Akte: {_tranferContainerDict.Count}");
      Console.WriteLine($"Anzahl Orphan Akte: {_ikarosOrphanItemsDict.Count()}");


      //jetzt berechne ich wie lange eigentlich die Felder mit Text sind
      int maxLenGläubigerNotizen = 0;
      int maxLenSchuldnerNotizen = 0;
      int maxLenBemerkung = 0;
      int maxLenAktenNotiz = 0;

      foreach(TransferContainer tc in _tranferContainerDict.Values ) {
        maxLenGläubigerNotizen = Math.Max(maxLenGläubigerNotizen, tc.CalcMaxLenGläubigerNotizen());
        maxLenSchuldnerNotizen = Math.Max(maxLenSchuldnerNotizen, tc.CalcMaxLenSchuldnerNotzen());
        maxLenBemerkung = Math.Max(maxLenBemerkung, tc.CalcMaxLenBemerkung());  
        maxLenAktenNotiz = Math.Max(maxLenAktenNotiz, tc.CalcMaxLenAktenNotiz());
      }

      Console.WriteLine($"MaxLenGläubigerNotizen: {maxLenGläubigerNotizen}");
      Console.WriteLine($"MaxLenSchuldnerNotizen: {maxLenSchuldnerNotizen}");
      Console.WriteLine($"MaxLenBemerkung:        {maxLenBemerkung}");
      Console.WriteLine($"MaxLenAktenNotiz:       {maxLenAktenNotiz}");


      //Queries sammeln
      List<string> alleQueries = new();

      foreach (TransferContainer tc in _tranferContainerDict.Values) {
        //ich teste nur mit 2 Akten: 
        if (!tc.IkarosAnr.StartsWith("20160005073") 
          && !tc.IkarosAnr.StartsWith("20140000885")
          && !tc.IkarosAnr.StartsWith("20170014007") ) {
          continue;
        }

        //jetzt muss ich für jeden Subito Akt, der zu dem tc passt, die 
        if (_ikaros_2_subito_lookup.ContainsKey(tc.IkarosAnr)) {
          foreach (Mapping_Ikaros_Subito mapping in _ikaros_2_subito_lookup[tc.IkarosAnr]) {
            string subitoAnr = mapping.SubitoAnr;
            tc.PrepareIkarosItemsForMigration(mapping.SubitoAnr);
            tc.MigrateIkarosItems(mapping.SubitoAnr);
            foreach(string q in tc.SqlUpdateQueryList) {
              string qneu = q.Replace("%SUBITO_ANR%", $"'{subitoAnr}'");
              alleQueries.Add(qneu);
            }
          }
        }
      }
      Console.WriteLine($"Anzahl der Queries: {alleQueries.Count}");
      File.WriteAllLines(@"h:\tmp\AVS\Export_Msg\update_documents.sql", alleQueries);

      foreach (var kvp in TransferContainer.GetFileTypeHistogramm()) { 
        Console.WriteLine($"FileType: {kvp.Key} Count: {kvp.Value}");
      }
      Console.WriteLine($"FilesFound Count: {TransferContainer.GetFileFoundCount()}");
      Console.WriteLine($"FilesNotFound Count: {TransferContainer.GetFileNotFoundCount()}");

    } //end     static void Main(string[] args)

  } //end   internal class Program

} //end namespace Dokumente_2_Subito

