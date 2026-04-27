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
    private readonly string _path;
    private readonly string _heicPath;
    private readonly string _notsupportPath;
    private IProgress<ProgressReport>? _progress;

    public ImageProcessor(string path)
    {
      _path = path;
      _heicPath = Path.Combine(_path, "_heic");
      _notsupportPath = Path.Combine(_path, "_notsupport");
    }

    public void ProcessFiles(IProgress<ProgressReport> progress, bool convertHeic, bool fixHeicDate)
    {
      _progress = progress;

      // Rozšířená počítadla pro detailní souhrn
      int countDateUpdated = 0;     // (countTrue)
      int countDateError = 0;     // (countFalse)
      int countUnsupportedMoved = 0;  // (countSkipped)
      int countHeicConverted = 0;
      int countHeicFailed = 0;
      int countSameDate = 0;
      int countHeicDateFixed = 0;

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
      var heicFiles = allFiles.Where(f =>
          System.IO.Path.GetExtension(f).Equals(".heic", StringComparison.OrdinalIgnoreCase))
          .ToList();

      if (heicFiles.Count > 0 && (convertHeic || fixHeicDate))
      {
        string phaseMsg = convertHeic ? "Konverze a oprava HEIC" : "Oprava datumu HEIC";
        Report(ReportType.Info, $"Fáze 1: {phaseMsg} souborů ({heicFiles.Count})...");
        int countHeicProcessed = 0;
        int heicTotal = heicFiles.Count;

        // Reset progress baru pro tuto fázi
        Report(ReportType.Info, "", 0, heicTotal);

        Parallel.ForEach(heicFiles, filename =>
        {
          string fName = Path.GetFileName(filename);
          string jpgCesta = Path.ChangeExtension(filename, ".jpg");
          string heicDestPath = Path.Combine(_heicPath, fName);
          int currentCount = Interlocked.Increment(ref countHeicProcessed);

          // Pokusíme se získat datum pro informativní výpis
          DateTime fileDate = default;
          bool hasDate = exifData.DateTaken(filename, ref fileDate);
          string dateInfo = hasDate ? $" [{fileDate:dd.MM.yyyy HH:mm}]" : "";

          Report(ReportType.Info, "", currentCount, heicTotal);

          try
          {
            // 1. OPRAVA DATUMU HEIC (pokud je zapnuto)
            if (fixHeicDate && hasDate)
            {
              if (fileDate != File.GetLastWriteTime(filename))
              {
                File.SetLastWriteTime(filename, fileDate);
                Interlocked.Increment(ref countHeicDateFixed);
                Report(ReportType.Success, $"Opraven datum HEIC: {fName}{dateInfo}");
              }
            }

            // 2. KONVERZE (pokud je zapnuto)
            if (convertHeic)
            {
              if (File.Exists(jpgCesta))
              {
                Report(ReportType.Warning, $"JPG již existuje, konverze přeskočena: {fName}{dateInfo}");
                Interlocked.Increment(ref countHeicConverted);
              }
              else
              {
                heicConverter.ConvertHeicToJpg(filename, jpgCesta, 95);
                Report(ReportType.Success, $"Převedeno na JPG: {fName}{dateInfo}");
                Interlocked.Increment(ref countHeicConverted);
              }

              // PŘESUN ORIGINÁLU (pouze při konverzi)
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
          }
          catch (Exception ex)
          {
            Report(ReportType.Error, $"Chyba při zpracování HEIC {fName}: {ex.Message}");
            Interlocked.Increment(ref countHeicFailed);
          }
        });
      }
      else if (heicFiles.Count > 0)
      {
        Report(ReportType.Warning, "Fáze 1: Zpracování HEIC souborů bylo vypnuto uživatelem.");
      }
      else
      {
        Report(ReportType.Info, "Fáze 1: Žádné HEIC soubory k převodu.");
      }

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

      // Reset progress baru pro tuto fázi
      Report(ReportType.Info, "", 0, dateJobTotal);

      Parallel.ForEach(filesForDateFix, filename =>
      {
        DateTime newDate = default(DateTime);
        string fName = Path.GetFileName(filename);
        int currentCount = Interlocked.Increment(ref countDateProcessed);

        Report(ReportType.Info, "", currentCount, dateJobTotal);

        try
        {
          if (exifData.DateTaken(filename, ref newDate))
          {
            if (newDate != default(DateTime) && newDate != File.GetLastWriteTime(filename))
            {
              File.SetLastWriteTime(filename, newDate);
              Interlocked.Increment(ref countDateUpdated);
              Report(ReportType.Success, $"Aktualizováno datum: {fName} -> {newDate}");
            }
            else
            {
              Interlocked.Increment(ref countSameDate); // Přidáno počítadlo
              Report(ReportType.Detail, $"Datum je již správné: {fName}");
            }
          }
          else
          {
            Interlocked.Increment(ref countDateError);
            Report(ReportType.Warning, $"EXIF datum nenalezeno: {fName}");
          }
        }
        catch (Exception ex)
        {
          Interlocked.Increment(ref countDateError);
          Report(ReportType.Error, $"Chyba při čtení/zápisu data {fName}: {ex.Message}");
        }
      });

      // --- FÁZE 3: Přesun nepodporovaných ---
      Report(ReportType.Info, "Fáze 3: Přesun nepodporovaných souborů...");
      var supportedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".heic" };
      var unsupportedFiles = allFiles.Where(f =>
          File.Exists(f) &&
          !supportedExtensions.Contains(Path.GetExtension(f)))
          .ToList();

      int unsupportedTotal = unsupportedFiles.Count;
      int countUnsupportedProcessed = 0;

      // Reset progress baru pro tuto fázi
      Report(ReportType.Info, "", 0, unsupportedTotal);

      Parallel.ForEach(unsupportedFiles, filename =>
      {
        string fName = Path.GetFileName(filename);
        string destPath = Path.Combine(_notsupportPath, fName);
        int currentCount = Interlocked.Increment(ref countUnsupportedProcessed);

        Report(ReportType.Info, "", currentCount, unsupportedTotal);

        try
        {
          if (File.Exists(destPath))
          {
            Report(ReportType.Warning, $"Nepodporovaný soubor již v cíli existuje: {fName}");
          }
          else
          {
            File.Move(filename, destPath);
            Interlocked.Increment(ref countUnsupportedMoved);
            Report(ReportType.Detail, $"Přesunuto (nepodporováno): {fName}");
          }
        }
        catch (Exception ex)
        {
          Report(ReportType.Error, $"Chyba při přesunu {fName}: {ex.Message}");
        }
      });

      // --- Rekapitulace (UPRAVENO) ---
      Report(ReportType.Info, "------------------------------------------------------");
      Report(ReportType.Info, "--- ZPRACOVÁNÍ DOKONČENO ---");
      Report(ReportType.Info, "------------------------------------------------------");

      Report(ReportType.Info, $"Celkem souborů v adresáři: {countTotal}");

      if (convertHeic || fixHeicDate)
      {
        Report(ReportType.Info, $"\n--- Zpracování HEIC ({heicFiles.Count} nalezeno) ---");
        if (convertHeic)
        {
          Report(ReportType.Success, $"Úspěšně převedeno/přeskočeno: {countHeicConverted}");
          Report(ReportType.Error, $"Chyba konverze: {countHeicFailed}");
        }
        if (fixHeicDate)
        {
          Report(ReportType.Success, $"Datum opraveno u HEIC originálů: {countHeicDateFixed}");
        }
      }
      else
      {
        Report(ReportType.Warning, $"\n--- Zpracování HEIC (VYPNUTO) ---");
        Report(ReportType.Detail, $"Nalezeno {heicFiles.Count} souborů, které byly přeskočeny.");
      }

      Report(ReportType.Info, $"\n--- Zpracování JPG/JPEG ({dateJobTotal} nalezeno) ---");
      Report(ReportType.Success, $"Datum aktualizováno: {countDateUpdated}");
      Report(ReportType.Detail, $"Datum již bylo správné: {countSameDate}");
      Report(ReportType.Warning, $"Datum nenalezeno / Chyba čtení: {countDateError}");

      Report(ReportType.Info, $"\n--- Ostatní soubory ---");
      Report(ReportType.Detail, $"Přesunuto (nepodporováno): {countUnsupportedMoved}");
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