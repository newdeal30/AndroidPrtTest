using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Bluetooth;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidPrtTest.Droid.BluetoothPrinter))]
namespace AndroidPrtTest.Droid {
  public class BluetoothPrinter : IBluetoothPrinter {

    private int _timeoutSec = 10;

    public List<Printer> GetPrinters() {
      try {
        var result = new List<Printer>();
        var adapter = BluetoothAdapter.DefaultAdapter;
        var devices = adapter.BondedDevices;
        foreach (var device in devices) {
          int deviceClass = (int)device.BluetoothClass.DeviceClass;
          if (deviceClass == 0 || deviceClass == 1664) //Unknown (Sunmi) || Printer
            result.Add(new Printer(device.Name, device.Address));
        }
        return result;
      }
      catch (Exception ex) {
        Console.WriteLine(ex.Message);
        return new List<Printer>();
      }
    }

    public async Task<bool> PrintAsync(string macAdr, byte[] bytes) {
      if (bytes == null || bytes.Length == 0) 
        return false;
      if (string.IsNullOrEmpty(macAdr)) 
        return false;

      var cntTry = 0;
      DateTime dtTimeout = DateTime.Now.AddSeconds(_timeoutSec);
      while (DateTime.Now < dtTimeout && cntTry <= 3) {
        try {
          cntTry++;
          var adapter = BluetoothAdapter.DefaultAdapter;
          if (!adapter.IsEnabled) 
            return false;
          adapter.CancelDiscovery();
          var devices = adapter.BondedDevices;
          var printer = devices.Where(x => x.Address.ToUpper() == macAdr.ToUpper()).FirstOrDefault();
          if (printer == null) 
            return false;
          try {
            using (BluetoothSocket btSocket = printer.CreateRfcommSocketToServiceRecord(Java.Util.UUID.FromString("00001101-0000-1000-8000-00805F9B34FB"))) {
              await btSocket.ConnectAsync();
              if (!btSocket.IsConnected) 
                continue;
              using (var stream = btSocket.OutputStream) {
                if (!stream.CanWrite) 
                  continue;
                foreach (byte b in bytes)
                  stream.WriteByte(b);
                stream.Flush();
                await Task.Delay(750);
              }
              btSocket.Close();
            }
          }
          catch (Exception exPrint) {
            await Task.Delay(250);
            continue;
          }
          finally {
            printer?.Dispose();
          }
          return true;
        }
        catch (Exception ex) {
          continue;
        }
      }
      return false;
    }

  }
}