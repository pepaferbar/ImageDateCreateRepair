using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;
using System;
using System.IO;

namespace DateCreateRepair2
{
  public class HeicConverter
  {
    /// <summary>
    /// Převede obrázek ve formátu HEIC na JPG.
    /// </summary>
    /// <param name="heicPath">Cesta ke vstupnímu HEIC souboru.</param>
    /// <param name="jpgPath">Cesta, kam se má uložit výsledný JPG soubor.</param>
    /// <param name="quality">Kvalita JPG (1-100). Výchozí je 90.</param>
    public void ConvertHeicToJpg(string heicPath, string jpgPath, int quality = 90)
    {
      // Použití 'using' je klíčové pro správné uvolnění zdrojů
      using (MagickImage image = new MagickImage(heicPath))
      {
        // Nastavení kvality pro JPG
        image.Quality = (uint)quality; 

        // Nastavení výstupního formátu
        image.Format = MagickFormat.Jpg;

        // Uložení souboru
        image.Write(jpgPath);
      }
    }
  }
}
