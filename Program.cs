// Nový obsah pro Program.cs

using DateCreateRepair2; // Používáme náš sjednocený namespace
using System;
using System.IO;

// Sjednocený namespace
namespace DateCreateRepair2
{
  internal class Program
  {
    static void Main(string[] args)
    {
      // 1. Získání cesty (z argumentu nebo aktuální adresář)
      // Můžete nyní přetáhnout složku na .exe soubor nebo ji zadat do příkazového řádku
      string path = args.Length > 0 ? args[0] : AppDomain.CurrentDomain.BaseDirectory;

      Console.WriteLine($"Zpracovávám adresář: {path}");

      // Kontrola, zda adresář existuje
      if (!Directory.Exists(path))
      {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Chyba: Zadaný adresář neexistuje.");
        Console.ResetColor();
        Console.ReadKey();
        return;
      }

      // 2. Vytvoření a spuštění procesoru
      try
      {
        // Třída 'Main' byla přejmenována na 'ImageProcessor'
        // a logika přesunuta z konstruktoru
        var processor = new ImageProcessor(path);
        processor.ProcessFiles(); // Spuštění hlavní metody
      }
      catch (Exception ex)
      {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Došlo k závažné neočekávané chybě: {ex.Message}");
        Console.ResetColor();
      }

      Console.WriteLine("\nZpracování dokončeno. Stiskněte libovolnou klávesu pro ukončení.");
      Console.ReadKey();
    }
  }
}