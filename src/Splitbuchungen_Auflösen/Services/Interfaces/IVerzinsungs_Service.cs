using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Splitbuchungen_Auflösen.Models;

namespace Splitbuchungen_Auflösen.Services.Interfaces
{
  public interface IVerzinsungs_Service
  {
    List<Einzelbuchung> Calculate_Zinsen(decimal betrag, DateTime von, DateTime bis, Verzinsung zinsInfo);
    List<Einzelbuchung> Calculate_Zinsen(List<Einzelbuchung> ebList, DateTime von, DateTime bis);
    public BuchungsSaldo CalcSaldo(string aktenzeichen, DateTime bis);
  }

} //end namespace Splitbuchungen_Auflösen.Services.Interfaces

