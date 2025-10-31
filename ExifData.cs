using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;

namespace ConsoleApp1
{
  internal class ExifData
  {

    internal bool DateTaken(string imagepath, ref DateTime newDate)
    {
      try
      {
        // 1. Načte všechny metadata z obrázku (funguje pro HEIC, JPEG, PNG...)
        var directories = ImageMetadataReader.ReadMetadata(imagepath);

        // 2. Najde EXIF adresář, kde je obvykle uložen datum pořízení.
        //    Váš kód používal ID 36867, což je 'DateTimeOriginal'.
        var subIfdDirectory = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();

        if (subIfdDirectory != null &&
            subIfdDirectory.TryGetDateTime(ExifDirectoryBase.TagDateTimeOriginal, out newDate))
        {
          // Úspěšně nalezeno a převedeno datum pořízení.
          return true;
        }

        // 3. Záložní řešení: Někdy je datum v hlavním IFD0 adresáři
        var ifd0Directory = directories.OfType<ExifIfd0Directory>().FirstOrDefault();
        if (ifd0Directory != null &&
            ifd0Directory.TryGetDateTime(ExifDirectoryBase.TagDateTimeOriginal, out newDate))
        {
          // Úspěšně nalezeno i zde.
          return true;
        }

        // 4. Poslední pokus: Zkusíme tag 'DateTime' (ID 306), 
        //    což je často datum poslední modifikace.
        if (ifd0Directory != null &&
            ifd0Directory.TryGetDateTime(ExifDirectoryBase.TagDateTime, out newDate))
        {
          // Nalezeno alespoň datum modifikace.
          return true;
        }

        // Pokud jsme se dostali až sem, datum nebylo v metadatech nalezeno.
        return false;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        // Zachytí jakoukoliv chybu při čtení souboru nebo parsování metadat
        return false;
      }
    }

  }
}
