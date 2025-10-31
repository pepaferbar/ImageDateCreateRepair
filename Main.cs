using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
  public class Main
  {
    public Main()
    {
      string path = System.AppDomain.CurrentDomain.BaseDirectory;

      //path= @"c:\_temp\iCloud Photos\";
      //path = @"d:\_temp2\___fotky_2025\_milošek\";

      System.Threading.Thread.Sleep(500);

      int count = 0;
      int countFalse = 0;
      int countTotal = System.IO.Directory.GetFiles(path).Count();

      ExifData exifData = new ExifData();

      foreach (var filename in System.IO.Directory.GetFiles(path))
      {
        switch (System.IO.Path.GetExtension(filename).ToLower())
        {
          case ".jpg":
          case ".jpeg":
          case ".heic":
            count++;

            DateTime newDate = default(DateTime);
            String fName = System.IO.Path.GetFileName(filename);

            if (exifData.DateTaken(filename, ref newDate))
            {
              if (newDate != System.IO.File.GetLastWriteTime(filename))
              {
                logMe(count, countTotal, fName, "update", ConsoleColor.Green);
                System.IO.File.SetLastWriteTime(filename, newDate);
              }
              else
              {
                logMe(count, countTotal, fName, "same date");
              }
            }
            else
            {
              logMe(count, countTotal, filename, "XXX", ConsoleColor.Red);
              countFalse++;
            }
            break;
        }
      }

      Console.WriteLine("");

      Console.WriteLine($"Celkem: {countTotal}, Projito: {count}, Neaktualizované: {countFalse}");

      Console.ReadKey();


    }

    private static void logMe(int count, int countTotal, string fName, string text, ConsoleColor color = ConsoleColor.Gray)
    {
      Console.ForegroundColor = color;

      Console.WriteLine($"{count}/{countTotal} - {fName} > {text}");

      Console.ResetColor();
    }
  }

}
