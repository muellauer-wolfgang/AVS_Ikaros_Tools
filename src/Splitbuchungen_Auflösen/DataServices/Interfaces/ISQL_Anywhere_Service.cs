using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Splitbuchungen_Auflösen.Models;


namespace Splitbuchungen_Auflösen.DataServices.Interfaces
{
  public interface ISQL_Anywhere_Service
  {
    decimal CalcZinsen(decimal betrag, decimal zinsatz, DateTime von, DateTime bis);
    public BuchungsSaldo CalcSaldo(string aktenzeichen, DateTime bis);

  }

}
