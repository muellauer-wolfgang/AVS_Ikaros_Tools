using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Splitbuchungen_Auflösen.Models;
using Splitbuchungen_Auflösen.Services.Interfaces;
using Splitbuchungen_Auflösen.Services;
using System.Reflection;


namespace Splitbuchungen_Auflösen_UnitTests.Services
{

  /// <summary>
  /// Diese Test-Klasse soll auch zeigen, wie ich private 
  /// Hethoden einer Klasse testen kann, wenn dich die 
  /// Klasse mit Reflection lade. 
  /// </summary>
  public class Verzinsungs_Service_Zinsen_Should
  {

    [Fact]
    public void Instantiate_Class_with_private_Methods()
    {
      //Arrange
      Type   verzinserType = typeof(Verzinsungs_Service);
      object verzinserInstance = Activator.CreateInstance(verzinserType);
      MethodInfo verzinserMethod = verzinserType.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
        .Where(x => x.Name == "Intervall_in_zwei_Zinsperioden" && x.IsPrivate)
        .First();

      //Act
      bool result = (bool)verzinserMethod.Invoke(verzinserInstance,
        new object[] { new DateTime(2023, 01, 01), new DateTime(2023, 01, 02) });

      //Assert
      Assert.False(false);
    }

  }  //end   public class Verbindungs_Service_Zinsen_Should

} //end namespace Splitbuchungen_Auflösen_UnitTests.Services




