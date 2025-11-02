using System;
using System.Windows.Forms;

// TÍMTO ŘÁDKEM OPRAVÍME VŠECHNY CHYBY CS0104
using Application = System.Windows.Forms.Application;

namespace DateCreateRepair2
{
  internal static class Program
  {
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      // Nyní můžeme bezpečně použít 'Application'
      Application.SetHighDpiMode(HighDpiMode.SystemAware);
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new MainForm());
    }
  }
}