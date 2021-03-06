using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebsitesDashboard.Util;


namespace CertExpirationChecker
{
    internal class Program
    {
        private static string pInputFile = String.Empty;
        private static string singleUrl = string.Empty;
        static async Task Main(string[] args)
        {
            if (!ProcessArguments(args))
                return;

            await ProcessUrls();

        }

        private static async Task ProcessUrls()
        {

            string[] lines = null;
            if (File.Exists(pInputFile))
            {
                lines = File.ReadAllLines(pInputFile);
                foreach (var line in lines)
                {
                    //skip commented lines 
                    if (line.StartsWith("#"))
                        continue;

                    var quoteDelimiter = @"""";
                    string siteName = GetSiteName(line, quoteDelimiter);
                    var httpsBindings = GetHttpsBindings(line);

                    foreach (var binding in httpsBindings)
                    {
                        if (binding == "https://")
                            continue;
                        await ProcessSingleSite(siteName, binding);
                    }
                }
            }
            else if (!string.IsNullOrEmpty(singleUrl))
            {
                lines = new string[1];
                lines[0] = singleUrl;
                await ProcessSingleSite(lines[0], lines[0]);
            }


        }
        private static async Task ProcessSingleSite(string siteName, string binding)
        {
            RemoteUrlInspector inspector = new RemoteUrlInspector();
            await inspector.GetExpiration(siteName, binding);
        }

        private static List<string> GetHttpsBindings(string line)
        {
            return line.Replace("bindings:", "").Split(',').Where(x => x.Contains("443")).Select(x => x.Replace("https/*:443:", "https://")).Select(x => x.Replace("https/:443:", "https://")).ToList();
        }

        private static string GetSiteName(string line, string quoteDelimiter)
        {
            var siteName = line.Substring(line.IndexOf(quoteDelimiter));
            siteName = siteName.Substring(0, siteName.IndexOf(quoteDelimiter));
            siteName = line.Substring(line.IndexOf(quoteDelimiter) + quoteDelimiter.Length);
            siteName = siteName.Substring(0, siteName.IndexOf(quoteDelimiter));
            return siteName;
        }

        private static bool ProcessArguments(string[] args)
        {
            if (args.Length == 1 && args[0].StartsWith("http"))
            {
                singleUrl = args[0];
                return true;
            }
            if (args.Length != 4)
            {
                Console.WriteLine("Usage:\nCertExpirationChecker.exe -i inputfile.txt -o outputfilename.txt");
                Console.WriteLine("Please pass the input (-i) file and output (-o) file arguments");
                return false;
            }

            var pInputCommand = string.Empty;
            var pInputFile = string.Empty;

            var pOutputCommand = string.Empty;
            var pOutputFile = string.Empty;

            if (args.Length == 4)
            {
                pInputCommand = args[0];
                pInputFile = args[1];
                pOutputCommand = args[2];
                pOutputFile = args[3];

                if (pInputCommand.Equals("-i", StringComparison.CurrentCultureIgnoreCase))
                    Console.WriteLine($"reading data from {pInputFile}");

                if (!File.Exists(pInputFile))
                {
                    Console.WriteLine($"Input file {pInputFile} doesn't exist. Exiting. ");
                    return false;
                }

                if (pOutputCommand.Equals("-o", StringComparison.CurrentCultureIgnoreCase))
                    Console.WriteLine($"generating output to console and {pOutputFile}");

                Program.pInputFile = pInputFile;
                Logger.DestinationFileName = pOutputFile;
                return true;
            }
            return false;
        }
    }
}
