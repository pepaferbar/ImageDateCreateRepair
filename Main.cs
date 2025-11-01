using DateCreateRepair2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
  public class Main
  {
    public static int count = 0;
    public static int countFalse = 0;
    public static int countTrue = 0;
    public static int countSkip = 0;
    public static int countTotal = 0;

    public Main()
    {
      string path = System.AppDomain.CurrentDomain.BaseDirectory;

      //path= @"c:\_temp\iCloud Photos\";
      //path = @"d:\_temp2\___fotky_2025\_milošek\";
      //path = @"d:\_temp2\___fotky_2025\_2025\";

      System.Threading.Thread.Sleep(500);

      countTotal = System.IO.Directory.GetFiles(path).Count();

      ExifData exifData = new ExifData();
      HeicConverter heicConverter = new HeicConverter();

      //heic conver
      foreach (var filename in System.IO.Directory.GetFiles(path))
      {
        switch (System.IO.Path.GetExtension(filename).ToLower())
        {
          case ".heic":
            try
            {
              string fName = System.IO.Path.GetFileName(filename);
              string jpgCesta = Path.ChangeExtension(filename, ".jpg");

              Console.WriteLine($"Zahajuji převod... {fName}");

              heicConverter.ConvertHeicToJpg(filename, jpgCesta, 95);
              Console.WriteLine($"Hotovo! Soubor uložen do: {jpgCesta}");

              string bakCesta = filename + ".bak";
              File.Move(filename, bakCesta);
              Console.WriteLine($"Hotovo! Soubor přejmenován na: {bakCesta}");

            }
            catch (Exception ex)
            {
              Console.WriteLine($"Chyba při převodu: {ex.Message}");
            }            
            break;
        }
      }

      //DateTaken
      foreach (var filename in System.IO.Directory.GetFiles(path))
      {
        count++;
        switch (System.IO.Path.GetExtension(filename).ToLower())
        {
          case ".jpg":
          case ".jpeg":
            //case ".heic":
            DateTime newDate = default(DateTime);
            string fName = System.IO.Path.GetFileName(filename);

            if (exifData.DateTaken(filename, ref newDate))
            {
              if (newDate != System.IO.File.GetLastWriteTime(filename))
              {
                countTrue++;
                logMe(fName, "update", ConsoleColor.Green);
                System.IO.File.SetLastWriteTime(filename, newDate);
              }
              else
              {
                logMe(fName, "same date");
              }
            }
            else
            {
              countFalse++;
              logMe(filename, "not date", ConsoleColor.DarkRed);
            }
            break;

          default:
            countSkip++;
            break;
        }
      }

      Console.WriteLine("");

      Console.WriteLine($"Celkem: {countTotal}, Projito: {count}, Aktualizované: {countTrue}, Neaktualizované: {countFalse}, Skipnuté: {countSkip}");

      Console.ReadKey();
    }

    private static void logMe(string fName, string text, ConsoleColor color = ConsoleColor.Gray)
    {
      Console.ForegroundColor = color;

      Console.WriteLine($"{count}/{countTotal} - {fName} > {text}");

      Console.ResetColor();
    }
  }

}
