using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokumente_2_Subito.Models
{
  public class Gläubiger
  {
    public string Name { get; set; } = "N.N.";
    public string InkassoBüro { get; set; } = "N.N.";
    public List<TransferContainer> Containers { get; private set; } = new();

    public override string ToString()
    {
      return $"N:{Name} IB:{InkassoBüro} Count:{Containers.Count}"; 
    }

  } //end   public class Gläubiger

} //end namespace Dokumente_2_Subito.Models

