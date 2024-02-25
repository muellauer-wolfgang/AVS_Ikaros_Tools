using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokumente_2_Subito.Models
{
  public class Gläubiger
  {
    public string Name { get; set; }
    public string InkassoBüro { get; set; }
    public List<SubitoAkt> Akten { get; private set; } = new();

  } //end   public class Gläubiger

} //end namespace Dokumente_2_Subito.Models

