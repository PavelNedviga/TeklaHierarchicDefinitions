using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using Syncfusion.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures;
using Tekla.Structures.Drawing;
using Tekla.Structures.Model;
using TeklaHierarchicDefinitions.TeklaAPIUtils;

namespace TeklaHierarchicDefinitions.Models
{
    internal static class ReplacingName
    {
        public static bool SetUserPropertyWrapper(this Tekla.Structures.Model.ProjectInfo projectInfo, string name, string value)
        {
            return projectInfo.SetUserProperty(name.ToTeklaProp(), value);
        }

        public static bool GetUserPropertyWrapper(this Tekla.Structures.Model.ProjectInfo projectInfo, string name, ref string value)
        {            
            return projectInfo.GetUserProperty(name.ToTeklaProp(), ref value);
        }

        public static bool GetUserPropertyWrapper(this Drawing drawing, string name, ref string value)
        {
            return drawing.GetUserProperty(name.ToTeklaProp(), ref value);
        }

        public static bool SetUserPropertyWrapper(this Drawing drawing, string name, string value)
        {
            return drawing.SetUserProperty(name.ToTeklaProp(), value);
        }
    }
}

namespace TeklaHierarchicDefinitions.TeklaAPIUtils
{
    internal static class UDANameConverter
    {
        private static Dictionary<string,Dictionary<string,string>> renamingWithVersion;
        private static Dictionary<string, string> renaming;

        public static string TeklaVersion()
        {
            var version = TeklaStructuresInfo.GetCurrentProgramVersion();
            string versionShort = "";
            if (version.Contains("2021"))
                versionShort = "2021";
            if (version.Contains("2022"))
                versionShort = "2022";
            if (version.Contains("2023"))
                versionShort = "2023";
            if (version.Contains("2024"))
                versionShort = "2024";            
            return versionShort;
        }

        public static void ReadRenameDict(string version)
        {
            string dir = string.Empty;
            //TeklaStructuresSettings.GetAdvancedOption("XSDATADIR", ref dir);
            dir   = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            string ApplicationConfigName = "UDAMapping.json";
            string applicationConfigPath = Path.Combine(dir, ApplicationConfigName);
            if (File.Exists(applicationConfigPath))
            {
                string readedConfig = File.ReadAllText(applicationConfigPath);
                renamingWithVersion = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(readedConfig);
                renaming = renamingWithVersion
                    .Where(t => t.Value.Keys.Contains(version))
                    .ToDictionary(t => t.Key, t => t.Value[version]);
            }
            else
                renaming = new  Dictionary<string, string>();
        }
            
        public static string ToTeklaProp(this string name) 
        {
            if (renaming == null)
                ReadRenameDict(TeklaVersion());
            string value = "";
            if (renaming.TryGetValue(name, out value))
                return value;
            return name;
        }

        public static ArrayList ToTeklaProp(this ArrayList names)
        {
            ArrayList updatedNames = new ArrayList();
            foreach (string name in names)
            {
                updatedNames.Add(ToTeklaProp(name));
            }
            return updatedNames;
        }
    }
}
