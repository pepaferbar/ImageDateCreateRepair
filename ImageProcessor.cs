// Upravený obsah souboru Main.cs (nyní třída ImageProcessor)

using DateCreateRepair2; // Pro HeicConverter
using System;
using System.Collections.Generic; // Pro HashSet
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// Sjednocený namespace
namespace DateCreateRepair2
{
  /// <summary>
  /// Hlavní třída zpracovávající obrázkové soubory v adresáři.
  /// Přejmenováno z "Main".
  /// </summary>
  public class ImageProcessor
  {
    // Přesunuto ze statických proměnných na instanční
    private int count = 0;
    private int countFalse = 0;
    private int countTrue = 0;
    private int countTotal = 0;
    private int countSkipped = 0; // NOVÉ: Počítadlo pro přesunuté

    private readonly string _path;
    private readonly string _heicPath; // NOVÉ: Cesta pro _heic
    private readonly string _notsupportPath; // NOVÉ: Cesta pro _notsupport
    private readonly object _logLock = new object(); // Zámek pro bezpečný výpis do konzole

    // Konstruktor nyní přijímá cestu a nastavuje cílové adresáře
    public ImageProcessor(string path)
    {
      _path = path;
      if (string.IsNullOrEmpty(_path))
      {
        throw new ArgumentNullException(nameof(path), "Cesta k adresáři nesmí být prázdná.");
      }

      // NOVÉ: Definice cílových adresářů
      _heicPath = Path.Combine(_path, "_heic");
      _notsupportPath = Path.Combine(_path, "_notsupport");
    }

    /// <summary>
    /// Pomocná metoda pro zajištění existence adresáře
    /// </summary>
    private void EnsureDirectoryExists(string path)
    {
      try
      {
        if (!Directory.Exists(path))
        {
          Directory.CreateDirectory(path);
          logMe("INFO", $"Vytvořen adresář: {path}", ConsoleColor.DarkCyan);
        }
      }
      catch (Exception ex)
      {
        logMe("CHYBA", $"Nepodařilo se vytvořit adresář {path}: {ex.Message}", ConsoleColor.Red);
        // Můžeme zde zvážit i vyhození výjimky, pokud je to kritické
      }
    }

    /// <summary>
    /// Hlavní metoda, která spouští veškeré zpracování.
    /// </summary>
    public void ProcessFiles()
    {
      // NOVÉ: Zajistíme existenci cílových adresářů
      EnsureDirectoryExists(_heicPath);
      EnsureDirectoryExists(_notsupportPath);

      string[] allFiles;
      try
      {
        // Načtení seznamu souborů JEDNOU
        allFiles = System.IO.Directory.GetFiles(_path);
      }
      catch (Exception ex)
      {
        logMe("CHYBA", $"Nepodařilo se načíst soubory z adresáře: {ex.Message}", ConsoleColor.Red);
        return;
      }

      countTotal = allFiles.Length;
      logMe("INFO", $"Nalezeno celkem {countTotal} souborů.", ConsoleColor.Cyan);

      ExifData exifData = new ExifData();
      HeicConverter heicConverter = new HeicConverter();

      // --- FÁZE 1: Konverze HEIC (Paralelně) ---
      logMe("INFO", "Zahajuji paralelní konverzi HEIC souborů...", ConsoleColor.Cyan);

      var heicFiles = allFiles.Where(f =>
          System.IO.Path.GetExtension(f).Equals(".heic", StringComparison.OrdinalIgnoreCase))
          .ToList();

      Parallel.ForEach(heicFiles, filename =>
      {
        string fName = System.IO.Path.GetFileName(filename);
        string jpgCesta = Path.ChangeExtension(filename, ".jpg");
        string heicDestPath = Path.Combine(_heicPath, fName); // UPRAVENO: Cíl je _heic složka

        try
        {
          // Idempotence: Zkontrolujeme, zda .jpg již neexistuje
          if (File.Exists(jpgCesta))
          {
            logMe(fName, "JPG již existuje, konverze přeskočena.", ConsoleColor.Yellow);
          }
          else
          {
            heicConverter.ConvertHeicToJpg(filename, jpgCesta, 95); // Používáme kvalitu 95
            logMe(fName, $"Úspěšně převedeno na JPG.", ConsoleColor.DarkGreen);
          }

          // Robustnost: Přesuneme .heic do _heic, pouze pokud konverze proběhla/existuje
          if (File.Exists(filename))
          {
            if (File.Exists(heicDestPath))
            {
              logMe(fName, $"HEIC soubor v cíli ({_heicPath}) již existuje, originál ponechán.", ConsoleColor.Yellow);
            }
            else
            {
              File.Move(filename, heicDestPath);
              logMe(fName, $"Původní HEIC přesunut do {_heicPath}", ConsoleColor.DarkGray);
            }
          }
        }
        catch (Exception ex)
        {
          logMe(fName, $"Chyba při převodu HEIC: {ex.Message}", ConsoleColor.Red);
        }
      });

      // --- FÁZE 2: Oprava data (Paralelně) ---
      logMe("INFO", "Zahajuji paralelní opravu EXIF dat...", ConsoleColor.Cyan);

      // Musíme znovu načíst soubory, abychom zahrnuli nově vytvořené .jpg
      string[] filesForDateFix;
      try
      {
        filesForDateFix = System.IO.Directory.GetFiles(_path)
            .Where(f => new[] { ".jpg", ".jpeg" }.Contains(
                System.IO.Path.GetExtension(f).ToLowerInvariant()))
            .ToArray();
      }
      catch (Exception ex)
      {
        logMe("CHYBA", $"Nepodařilo se znovu načíst soubory pro opravu data: {ex.Message}", ConsoleColor.Red);
        return;
      }

      int dateJobTotal = filesForDateFix.Length;
      count = 0; // Resetujeme počítadlo pro tuto fázi

      Parallel.ForEach(filesForDateFix, filename =>
      {
        // (Tato část zůstává beze změny)
        int currentCount = Interlocked.Increment(ref count);
        DateTime newDate = default(DateTime);
        string fName = System.IO.Path.GetFileName(filename);

        try
        {
          if (exifData.DateTaken(filename, ref newDate))
          {
            var existingWriteTime = System.IO.File.GetLastWriteTime(filename);

            if (newDate != default(DateTime) && newDate != existingWriteTime)
            {
              System.IO.File.SetLastWriteTime(filename, newDate);
              Interlocked.Increment(ref countTrue);
              logMe(fName, $"Aktualizováno datum na {newDate}", ConsoleColor.Green, currentCount, dateJobTotal);
            }
            else
            {
              logMe(fName, "Datum je již správné.", ConsoleColor.Gray, currentCount, dateJobTotal);
            }
          }
          else
          {
            Interlocked.Increment(ref countFalse);
            logMe(fName, "EXIF datum nenalezeno.", ConsoleColor.DarkRed, currentCount, dateJobTotal);
          }
        }
        catch (Exception ex)
        {
          Interlocked.Increment(ref countFalse);
          logMe(fName, $"Chyba při čtení/zápisu data: {ex.Message}", ConsoleColor.Red, currentCount, dateJobTotal);
        }
      });

      // --- FÁZE 3: Přesun nepodporovaných souborů ---
      logMe("INFO", "Zahajuji přesun nepodporovaných souborů...", ConsoleColor.Cyan);

      // Definujeme, co podporujeme
      var supportedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".heic" };

      // Bereme původní seznam, odfiltrujeme podporované
      var unsupportedFiles = allFiles.Where(f =>
          !supportedExtensions.Contains(System.IO.Path.GetExtension(f))).ToList();

      int unsupportedTotal = unsupportedFiles.Count;

      Parallel.ForEach(unsupportedFiles, filename =>
      {
        string fName = System.IO.Path.GetFileName(filename);
        string destPath = Path.Combine(_notsupportPath, fName);
        int currentSkipped = Interlocked.Increment(ref countSkipped);

        try
        {
          // Zkontrolujeme, zda soubor stále existuje na původním místě
          if (File.Exists(filename))
          {
            if (File.Exists(destPath))
            {
              logMe(fName, $"Soubor v cíli ({_notsupportPath}) již existuje, originál ponechán.", ConsoleColor.Yellow, currentSkipped, unsupportedTotal);
            }
            else
            {
              File.Move(filename, destPath);
              logMe(fName, $"Přesunuto do _notsupport", ConsoleColor.Blue, currentSkipped, unsupportedTotal);
            }
          }
        }
        catch (Exception ex)
        {
          // Chyba, pokud je soubor zamčený nebo cíl existuje
          logMe(fName, $"Chyba při přesunu: {ex.Message}", ConsoleColor.Red, currentSkipped, unsupportedTotal);
        }
      });


      // --- Rekapitulace ---
      Console.WriteLine("");
      Console.WriteLine($"--- Rekapitulace ---");
      Console.WriteLine($"Celkem relevantních souborů k opravě data: {dateJobTotal}");
      Console.WriteLine($"Aktualizováno: {countTrue} (Zelená)");
      Console.WriteLine($"Datum nenalezeno / Chyba: {countFalse} (Červená)");
      Console.WriteLine($"Datum shodné (Přeskočeno): {dateJobTotal - countTrue - countFalse} (Šedá)");
      Console.WriteLine($"Nepodporováno (přesunuto): {countSkipped} (Modrá)"); // UPRAVENO
    }

    /// <summary>
    /// Zapisuje do konzole bezpečným způsobem z více vláken.
    /// </summary>
    private void logMe(string fName, string text, ConsoleColor color = ConsoleColor.Gray, int? current = null, int? total = null)
    {
      // Zámek zaručí, že se výpisy z různých vláken nepomíchají
      lock (_logLock)
      {
        Console.ForegroundColor = color;

        string prefix = (current.HasValue && total.HasValue)
            ? $"{current.Value}/{total.Value}"
            : (fName == "INFO" || fName == "CHYBA" ? fName : "INFO"); // Upraveno pro logování INFO

        string message = (fName == "INFO" || fName == "CHYBA")
            ? $"[{prefix}] > {text}"
            : $"[{prefix}] - {fName} > {text}";

        Console.WriteLine(message);

        Console.ResetColor();
      }
    }
  }
}