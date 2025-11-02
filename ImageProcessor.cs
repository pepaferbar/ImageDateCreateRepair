using DateCreateRepair2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DateCreateRepair2
{
  public class ImageProcessor
  {
    // Odebrána všechna počítadla (count, countTrue, atd.)
    // Odebrán _logLock

    private readonly string _path;
    private readonly string _heicPath;
    private readonly string _notsupportPath;

    // Objekt, kterému budeme hlásit stav (může to být formulář, konzole, cokoliv)
    private IProgress<ProgressReport>? _progress;

    public ImageProcessor(string path)
    {
      _path = path;
      _heicPath = Path.Combine(_path, "_heic");
      _notsupportPath = Path.Combine(_path, "_notsupport");
    }

    // Hlavní metoda nyní přijímá IProgress a je asynchronní
    // (Používáme Task.Run ve formuláři, takže async zde není nutný, ale je to dobrá praxe)
    public void ProcessFiles(IProgress<ProgressReport> progress)
    {
      _progress = progress;

      // Počítadla jsou nyní lokální a jejich souhrn se pošle na konci
      int countTrue = 0;
      int countFalse = 0;
      int countSkipped = 0;

      EnsureDirectoryExists(_heicPath);
      EnsureDirectoryExists(_notsupportPath);

      string[] allFiles;
      try
      {
        allFiles = System.IO.Directory.GetFiles(_path);
      }
      catch (Exception ex)
      {
        Report(ReportType.Error, $"Nepodařilo se načíst soubory: {ex.Message}");
        return;
      }

      int countTotal = allFiles.Length;
      Report(ReportType.Info, $"Nalezeno celkem {countTotal} souborů.");

      ExifData exifData = new ExifData();
      HeicConverter heicConverter = new HeicConverter();

      // --- FÁZE 1: Konverze HEIC ---
      Report(ReportType.Info, "Fáze 1: Konverze HEIC souborů...");
      var heicFiles = allFiles.Where(f =>
          System.IO.Path.GetExtension(f).Equals(".heic", StringComparison.OrdinalIgnoreCase))
          .ToList();

      Parallel.ForEach(heicFiles, filename =>
      {
        string fName = Path.GetFileName(filename);
        string jpgCesta = Path.ChangeExtension(filename, ".jpg");
        string heicDestPath = Path.Combine(_heicPath, fName);

        try
        {
          if (File.Exists(jpgCesta))
          {
            Report(ReportType.Warning, $"JPG již existuje, konverze přeskočena: {fName}");
          }
          else
          {
            heicConverter.ConvertHeicToJpg(filename, jpgCesta, 95);
            Report(ReportType.Success, $"Převedeno na JPG: {fName}");
          }

          if (File.Exists(filename))
          {
            if (File.Exists(heicDestPath))
            {
              Report(ReportType.Warning, $"HEIC soubor v cíli ({_heicPath}) již existuje: {fName}");
            }
            else
            {
              File.Move(filename, heicDestPath);
              Report(ReportType.Detail, $"Přesunut HEIC originál: {fName}");
            }
          }
        }
        catch (Exception ex)
        {
          Report(ReportType.Error, $"Chyba při převodu HEIC {fName}: {ex.Message}");
        }
      });

      // --- FÁZE 2: Oprava data ---
      Report(ReportType.Info, "Fáze 2: Oprava EXIF dat...");
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
        Report(ReportType.Error, $"Chyba při načítání JPG souborů: {ex.Message}");
        return;
      }

      int dateJobTotal = filesForDateFix.Length;
      int countDateProcessed = 0;

      Parallel.ForEach(filesForDateFix, filename =>
      {
        DateTime newDate = default(DateTime);
        string fName = Path.GetFileName(filename);
        int currentCount = Interlocked.Increment(ref countDateProcessed);

        // Hlášení pokroku pro ProgressBar
        Report(ReportType.Info, "", currentCount, dateJobTotal);

        try
        {
          if (exifData.DateTaken(filename, ref newDate))
          {
            if (newDate != default(DateTime) && newDate != File.GetLastWriteTime(filename))
            {
              File.SetLastWriteTime(filename, newDate);
              Interlocked.Increment(ref countTrue);
              Report(ReportType.Success, $"Aktualizováno datum: {fName} -> {newDate}");
            }
            else
            {
              Report(ReportType.Detail, $"Datum je již správné: {fName}");
            }
          }
          else
          {
            Interlocked.Increment(ref countFalse);
            Report(ReportType.Warning, $"EXIF datum nenalezeno: {fName}");
          }
        }
        catch (Exception ex)
        {
          Interlocked.Increment(ref countFalse);
          Report(ReportType.Error, $"Chyba při čtení/zápisu data {fName}: {ex.Message}");
        }
      });

      // --- FÁZE 3: Přesun nepodporovaných ---
      Report(ReportType.Info, "Fáze 3: Přesun nepodporovaných souborů...");
      var supportedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".heic" };
      var unsupportedFiles = allFiles.Where(f =>
          File.Exists(f) && // Zkontrolujeme, zda ještě existuje
          !supportedExtensions.Contains(Path.GetExtension(f)))
          .ToList();

      Parallel.ForEach(unsupportedFiles, filename =>
      {
        string fName = Path.GetFileName(filename);
        string destPath = Path.Combine(_notsupportPath, fName);

        try
        {
          if (File.Exists(destPath))
          {
            Report(ReportType.Warning, $"Nepodporovaný soubor již v cíli existuje: {fName}");
          }
          else
          {
            File.Move(filename, destPath);
            Interlocked.Increment(ref countSkipped);
            Report(ReportType.Detail, $"Přesunuto (nepodporováno): {fName}");
          }
        }
        catch (Exception ex)
        {
          Report(ReportType.Error, $"Chyba při přesunu {fName}: {ex.Message}");
        }
      });

      // --- Rekapitulace ---
      Report(ReportType.Info, "--- Zpracování dokončeno ---");
      Report(ReportType.Info, $"Aktualizováno: {countTrue}");
      Report(ReportType.Warning, $"Datum nenalezeno / Chyba: {countFalse}");
      Report(ReportType.Detail, $"Přesunuto (nepodporováno): {countSkipped}");
    }

    private void EnsureDirectoryExists(string path)
    {
      try
      {
        if (!Directory.Exists(path))
        {
          Directory.CreateDirectory(path);
          Report(ReportType.Info, $"Vytvořen adresář: {path}");
        }
      }
      catch (Exception ex)
      {
        Report(ReportType.Error, $"Nepodařilo se vytvořit adresář {path}: {ex.Message}");
      }
    }

    // Jediná metoda pro hlášení stavu
    private void Report(ReportType type, string message, int? current = null, int? total = null)
    {
      _progress?.Report(new ProgressReport
      {
        Type = type,
        Message = message,
        CurrentProgress = current,
        TotalProgress = total
      });
    }
  }
}