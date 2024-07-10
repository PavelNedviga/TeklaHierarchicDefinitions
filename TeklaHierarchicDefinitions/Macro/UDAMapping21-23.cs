using System.Diagnostics;
using System;
using System.IO;
using System.Windows.Forms;
using Tekla.Structures;
using TSM = Tekla.Structures.Model;
using Tekla.Structures.Model;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Tekla.Structures.Model.UI;



namespace Tekla.Technology.Akit.UserScript
{
    public class Script
    {
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr handle);

        public static void Run(Tekla.Technology.Akit.IScript akit)
        {
            TSM.Model CurrentModel = new TSM.Model();
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
            string dir = string.Empty;
            TeklaStructuresSettings.GetAdvancedOption("XSDATADIR", ref dir);

            string udaMappingPath = Path.Combine(dir, "Environments\\TEKLA_FIRM\\uda_mapping.json");

            if (File.Exists(udaMappingPath))
            {
                // Загрузим файл маппирования

                var mapping = LoadMapping(udaMappingPath);

                var model = new Model();
                // Выбор объектов пользователем
                var selectedObjects = new Picker().PickObjects(Picker.PickObjectsEnum.PICK_N_OBJECTS);

                foreach (var obj in selectedObjects)
                {
                    if (obj is ModelObject modelObject)
                    {
                        ConvertAttributes(modelObject, mapping);
                    }
                }
                model.CommitChanges("Updated attributes with mapping");
            }
            else
            {
                MessageBox.Show(udaMappingPath + " not found.");
            }
        }


        static Dictionary<string, string> LoadMapping(string filePath, string initialVersion = "2021", string destinationVersion = "2023")
        {
            var json = File.ReadAllText(filePath);
            var mapping = JObject.Parse(json);
            var result = new Dictionary<string, Dictionary<string, string>>();

            foreach (var prop in mapping.Properties())
            {
                var versionMapping = new Dictionary<string, string>();
                foreach (var version in prop.Value.Children<JProperty>())
                {
                    versionMapping[version.Name] = version.Value.ToString();
                }
                result[prop.Name] = versionMapping;
            }
            var actualMapping = new Dictionary<string, string>();
            foreach (var rs in result)
            {
                var k = rs.Value[initialVersion].Replace("USERDEFINED.", "");
                var v = rs.Value[destinationVersion].Replace("USERDEFINED.", "");
                if (!actualMapping.ContainsKey(k)) actualMapping[k] = v;
            }

            return actualMapping;
        }

        static void ConvertAttributes(ModelObject modelObject, Dictionary<string, string> mapping)
        {
            var hashTable = new Hashtable();
            List<string> stringPropertyNames = new List<string>();
            List<string> stringValues = new List<string>();
            List<string> doublePropertyNames = new List<string>();
            List<double> doubleValues = new List<double>();
            List<string> intPropertyNames = new List<string>();
            List<int> intValues = new List<int>();
            modelObject.GetAllUserProperties(ref hashTable);
            foreach (var prop in hashTable.Keys)
            {
                if (mapping.ContainsKey(prop.ToString()))
                {
                    if (hashTable[prop].GetType() == typeof(string))
                    {
                        stringPropertyNames.Add(prop.ToString());
                        stringValues.Add(hashTable[prop].ToString());
                        continue;
                    }
                    int intValue;

                    if (hashTable[prop].GetType() == typeof(int))
                    {
                        intPropertyNames.Add(prop.ToString());
                        intValues.Add(Int32.Parse(hashTable[prop].ToString()));
                        continue;
                    }
                    var stringValue = hashTable[prop] as string;
                    if (!string.IsNullOrEmpty(stringValue))
                    {
                        doublePropertyNames.Add(prop.ToString());
                        doubleValues.Add(Double.Parse(hashTable[prop].ToString()));
                        continue;
                    }
                }
            }
            modelObject.SetUserProperties(stringPropertyNames, stringValues,
                doublePropertyNames, doubleValues,
                intPropertyNames, intValues);
            modelObject.Modify();
        }

    }
}