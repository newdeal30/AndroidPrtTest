using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AndroidPrtTest {
  public interface IBluetoothPrinter {

    Task<bool> PrintAsync(string macAdr, byte[] bytes);

    List<Printer> GetPrinters();
    
  }

  public class Printer {
    public string Name { get; set; }
    public string MacAdress { get; set; }
    public Printer(string name, string macAdr) {
      Name = name;
      MacAdress = macAdr;
    }
  }
}
