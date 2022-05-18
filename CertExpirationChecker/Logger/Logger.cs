using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertExpirationChecker
{
    public static class Logger
    {
        public static string DestinationFileName;

        public static void LogMessage(string str)
        {
            if (!string.IsNullOrEmpty(DestinationFileName))
            {
                try
                {
                    System.IO.File.AppendAllText(DestinationFileName, str);
                    System.IO.File.AppendAllText(DestinationFileName, Environment.NewLine);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception opening output file\n" + ex.ToString());

                }
            }
            Console.WriteLine(str);
        }
        public static void LogError(string str)
        {
            Console.WriteLine(str);
        }
    }
}
