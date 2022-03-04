using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Model;
using TSC = Tekla.Structures.Catalogs;
using System.Text.RegularExpressions;
using TeklaHierarchicDefinitions.TeklaAPIUtils;

namespace TeklaHierarchicDefinitions.Classifiers
{
    static class ClassGenerator
    {
        static Model model = TeklaDB.model;
        public static string Generate(string partMark)
        {
            
            bool connected = model.GetConnectionStatus();
            if (connected)
            {
                string path = GetClassTablePath();
                if (System.IO.File.Exists(path))
                {
                    var dict = System.IO.File.ReadLines(path).Select(line => line.Split('\t')).ToDictionary(line => line[0], line => line[1]);// GetConversionList(path);

                    var numAlpha = new Regex("(?<Alpha>[a-zA-Zа-яА-ЯёЁ]*)(?<Numeric>[0-9]*)");
                    var match = numAlpha.Match(partMark);

                    var alpha = match.Groups["Alpha"].Value;
                    if (dict.Keys.Contains(alpha))
                    {
                        var encodedPrefix = dict[alpha];
                        var num = match.Groups["Numeric"].Value;
                        string ext_class = encodedPrefix + num.ToString().PadLeft(2, '0');
                        return ext_class;
                    }
                    else
                    {
                        return "10000";
                    }
                }
                else
                {
                    return "20000";
                }

            }
            return "30000";
        }

        internal static string GetClassTablePath()
        {
            string path = model.GetInfo().ModelPath + "\\#ClassConversion.csv"; 

            return path;
        }
    }
}
