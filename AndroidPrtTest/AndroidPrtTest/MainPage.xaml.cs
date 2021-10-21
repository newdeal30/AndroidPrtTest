using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AndroidPrtTest {
  public partial class MainPage : ContentPage {
    public MainPage() {
      InitializeComponent();
    }

    private async void Button_Clicked(object sender, EventArgs e) {
      var printerService = DependencyService.Get<IBluetoothPrinter>();
      var printer = printerService.GetPrinters().FirstOrDefault();
      StringBuilder sb = new StringBuilder();
      for (int x=0; x<20;x++) {
        sb.Append($"Test-Zeile {x}\r\n");
      }
      var bytes = Encoding.Default.GetBytes(sb.ToString());
      await printerService.PrintAsync(printer.MacAdress, bytes);
    }
  }
}
