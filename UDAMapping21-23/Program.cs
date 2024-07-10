using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using System.Text.Json;
using Tekla.Structures.Model;
using Tekla.Structures.Model.UI;
using System.Collections;
using Tekla.Structures;

namespace UDAMapping21_23
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
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
                string dir = string.Empty;
                TeklaStructuresSettings.GetAdvancedOption("XSDATADIR", ref dir);

                string udaMappingPath = Path.Combine(dir, "Environments\\TEKLA_FIRM\\uda_mapping.json");

                // Загрузим файл маппирования
                if (File.Exists(udaMappingPath))
                {
                    var mapping = LoadMapping(udaMappingPath);

                    var model = new Model();
                    // Выбор объектов пользователем
                    var selectedObjects = new Tekla.Structures.Model.UI.Picker().PickObjects(Picker.PickObjectsEnum.PICK_N_OBJECTS);

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
                    Console.WriteLine(udaMappingPath + " - файл не существует");
                    Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadKey();
            }
        }

        static Dictionary<string, string> LoadMapping(string filePath, string initialVersion = "2021", string destinationVersion = "2023")
        {
            var json = File.ReadAllText(filePath);
            var mapping = JsonSerializer.Deserialize<Dictionary<string,Dictionary<string,string>>>(json);

            var actualMapping = new Dictionary<string, string>();

            foreach (var prop in mapping)
            {
                var k = prop.Value[initialVersion].Replace("USERDEFINED.", "");
                var v = prop.Value[destinationVersion].Replace("USERDEFINED.", "");
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
            foreach(var prop in hashTable.Keys)
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