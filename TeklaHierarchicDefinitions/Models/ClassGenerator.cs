using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using Tekla.Structures.Model;
using TeklaHierarchicDefinitions.TeklaAPIUtils;

namespace TeklaHierarchicDefinitions.Classifiers
{
    static class ClassGenerator
    {
        static Model model = TeklaDB.model;
        public static string Generate(string partMark, string position)
        {

            bool connected = model.GetConnectionStatus();
                if (connected)
            {
                string path = GetClassTablePath();
                if (System.IO.File.Exists(path))
                {

                       var dict = System.IO.File.ReadLines(path)
                            .Select(line => line.Split('\t'))
                            .GroupBy(t => t[0])
                            .ToDictionary(line => line.First()[0], line => line.First()[1]);// GetConversionList(path);



                    var numAlpha = new Regex("(?<Alpha>[a-zA-Zа-яА-ЯёЁ]*)(?<Numeric>[0-9]*)");
                    var match = numAlpha.Match(partMark);

                    var alpha = match.Groups["Alpha"].Value;
                    if (dict.Keys.Contains(alpha))
                    {
                        var encodedPrefix = dict[alpha];
                        var num = match.Groups["Numeric"].Value;
                        var matchPosition = numAlpha.Match(position).Groups["Numeric"].Value;
                        string ext_class = encodedPrefix + num.ToString().PadLeft(2, '0') + matchPosition.PadLeft(1, '0');
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

        public static string GenerateCategory(string partMark)
        {

            bool connected = model.GetConnectionStatus();
            if (connected)
            {
                string path = GetClassTablePath();
                if (System.IO.File.Exists(path))
                {
                    var dict = System.IO.File.ReadLines(path)
                            .Select(line => line.Split('\t'))
                            .GroupBy(t => t[0])
                            .ToDictionary(line => line.First()[0], line => line.First()[2]);// GetConversionList(path);

                    var numAlpha = new Regex("(?<Alpha>[a-zA-Zа-яА-ЯёЁ]*)(?<Numeric>[0-9]*)");
                    var match = numAlpha.Match(partMark);

                    var alpha = match.Groups["Alpha"].Value;
                    if (dict.Keys.Contains(alpha))
                    {
                        var encodedPrefix = dict[alpha];
                        return encodedPrefix;
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }

            }
            return "<нс>";
        }

        internal static string GetClassTablePath()
        {
            string path = model.GetInfo().ModelPath + "\\#ClassConversion.csv";

            return path;
        }
    }
}
