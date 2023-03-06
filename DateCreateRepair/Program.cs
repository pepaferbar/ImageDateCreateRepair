using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace DateCreateRepair
{
    class Program
    {
        static void Main(string[] args)
        {
            
            string path;

            path = System.AppDomain.CurrentDomain.BaseDirectory;

            //path= @"c:\_temp\iCloud Photos\";

            System.Threading.Thread.Sleep(500);

            int count = 0;
            int countTotal = System.IO.Directory.GetFiles(path).Count();

            foreach (var filename in System.IO.Directory.GetFiles(path))
            {
                switch (System.IO.Path.GetExtension(filename).ToLower())
                {
                    case ".jpg":
                    case ".jpeg":
                        count++;

                        DateTime newDate=default(DateTime);

                        if (DateTaken(filename, ref newDate))
                        {
                            String fName = System.IO.Path.GetFileName(filename);
                            Console.Write("{0}/{1} - {2}", count, countTotal, fName);

                            if (newDate != System.IO.File.GetLastWriteTime(filename))
                            {
                                Console.Write(" > update");
                                System.IO.File.SetLastWriteTime(filename, newDate);
                            }

                            Console.WriteLine("");

                        }

                        break;
                }
            }

            Console.WriteLine("");
            Console.WriteLine("hotovo");

            System.Threading.Thread.Sleep(2000);
        }

        static bool DateTaken(string imagepath, ref DateTime newDate)
        {
            try
            {
                //http://snipplr.com/view/25074/date-taken-exif-data-for-a-picture/

                int DateTakenValue = 36867;

                Image myImg = new Bitmap(imagepath);

                string dateTakenTag = System.Text.Encoding.ASCII.GetString(myImg.GetPropertyItem(DateTakenValue).Value);

                myImg.Dispose();

                dateTakenTag = dateTakenTag.Substring(0, 19).Replace(" ", ":");

                String[] parts = dateTakenTag.Split(':');


                Int16 Year = Convert.ToInt16(parts[0]);
                Int16 Month = Convert.ToInt16(parts[1]);
                Int16 Day = Convert.ToInt16(parts[2]);

                Int16 Hour = Convert.ToInt16(parts[3]);
                Int16 Minute = Convert.ToInt16(parts[4]);
                Int16 Second = Convert.ToInt16(parts[5]);

                newDate = new DateTime(Year, Month, Day, Hour, Minute, Second);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}

