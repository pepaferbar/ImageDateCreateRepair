// Upravený obsah souboru ExifData.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;

// Sjednocený namespace
namespace DateCreateRepair2
{
  internal class ExifData
  {
    internal bool DateTaken(string imagepath, ref DateTime newDate)
    {
      try
      {
        // 1. Načte všechny metadata z obrázku
        var directories = ImageMetadataReader.ReadMetadata(imagepath);

        // 2. Najde EXIF adresář (DateTimeOriginal)
        var subIfdDirectory = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();

        if (subIfdDirectory != null &&
            subIfdDirectory.TryGetDateTime(ExifDirectoryBase.TagDateTimeOriginal, out newDate))
        {
          return true; // Úspěšně nalezeno
        }

        // 3. Záložní řešení: IFD0 adresář (DateTimeOriginal)
        var ifd0Directory = directories.OfType<ExifIfd0Directory>().FirstOrDefault();
        if (ifd0Directory != null &&
            ifd0Directory.TryGetDateTime(ExifDirectoryBase.TagDateTimeOriginal, out newDate))
        {
          return true; // Úspěšně nalezeno i zde
        }

        // 4. Poslední pokus: tag 'DateTime' (datum modifikace)
        if (ifd0Directory != null &&
            ifd0Directory.TryGetDateTime(ExifDirectoryBase.TagDateTime, out newDate))
        {
          return true; // Nalezeno alespoň datum modifikace
        }

        // Datum nenalezeno
        return false;
      }
      catch (Exception)
      {
        // Chyba bude zachycena a zalogována o úroveň výš (v ImageProcessor)
        // Původní: Console.WriteLine(ex.Message);
        return false;
      }
    }
  }
}