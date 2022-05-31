#pragma warning disable 1633 // Unrecognized #pragma directive
#pragma reference "Tekla.Macros.Wpf.Runtime"
#pragma reference "Tekla.Macros.Akit"
#pragma reference "Tekla.Macros.Runtime"
//#pragma reference "System.Windows"
#pragma warning restore 1633 // Unrecognized #pragma directive

using System.IO;
using Tekla.Structures;
using System.Text.RegularExpressions;
using Tekla.Structures.Model;
//using Tekla.Structures.Dialogs;
using System.Windows.Forms;
using System.Collections.Generic;
using System;

namespace UserMacros {
    public sealed class Macro {
        [Tekla.Macros.Runtime.MacroEntryPointAttribute()]
        public static void Run(Tekla.Macros.Runtime.IMacroRuntime runtime) {
            Tekla.Macros.Akit.IAkitScriptHost akit = runtime.Get<Tekla.Macros.Akit.IAkitScriptHost>();
            Tekla.Macros.Wpf.Runtime.IWpfMacroHost wpf = runtime.Get<Tekla.Macros.Wpf.Runtime.IWpfMacroHost>();
            Tekla.Macros.Akit.AkitMacroBackend akitMacroBackend = runtime.Get<Tekla.Macros.Akit.AkitMacroBackend>();

            var model = new Model();
            var m = model.GetConnectionStatus();

            string sourceDir = Path.Combine(model.GetInfo().ModelPath, "Спецификация металлопроката");
            string[] picList = Directory.GetFiles(sourceDir, "*.xlsx");
            var cu = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var statAdress = Path.Combine(cu,@"ООО «АйДи Инжиниринг»\ASD-Presets - База объектов-аналогов");

            Regex rx = new Regex(@"[0-9]{4}-[0-9]{2}-[0-9]{4}-[А-Я]{2,3}[0-9].*СМ.*", RegexOptions.Compiled);
            Regex rxIncorrect = new Regex(@"[0-9]{4}-[0-9]{2}-[0-9]{4}-[А-Яа-яA-Za-z]{2,3}[0-9].*(СМ|CM|cm|см).*", RegexOptions.Compiled);
            Regex album = new Regex(@"[0-9]{4}-[0-9]{2}-[0-9]{4,5}", RegexOptions.Compiled);
            List<string> correctNames = new List<string>();
            List<string> incorrectNames = new List<string>();
            HashSet<string> objectList = new HashSet<string>();

            bool copyDict = false;

            // Copy files.
            foreach (string f in picList)
            {
                var fileName = Path.GetFileNameWithoutExtension(f);
                if (rx.Match(fileName).Success)
                {
                    var objectStatAdress = Path.Combine(statAdress, album.Match(fileName).Value, model.GetInfo().ModelName.Replace(".db1", ""));
                    if (!Directory.Exists(objectStatAdress))
                    {
                        Directory.CreateDirectory(objectStatAdress);
                    }
                    // Remove path from the file name.
                    string fName = f.Substring(sourceDir.Length + 1);
                    // Use the Path.Combine method to safely append the file name to the path.
                    // Will overwrite if the destination file already exists.
                    File.Copy(Path.Combine(sourceDir, fName), Path.Combine(objectStatAdress, fName), true);
                    correctNames.Add(fileName);
                    copyDict = true;
                    objectList.Add(objectStatAdress); //Path.Combine(statAdress, album.Match(fileName).Value)
                }
                else if (rxIncorrect.Match(fileName).Success)
                {
                    incorrectNames.Add(fileName);
                }

            }


            wpf.InvokeCommand("CommandRepository", "Reports.CreateReport");
            akit.ListSelect("xs_report_dialog", "xs_report_list", new string[] {
                        "#CategoryGroups"});
            akit.ValueChange("xs_report_dialog", "xs_report_file", model.GetInfo().ModelName.Replace(".db1", "") + ".csv");
            akit.TabChange("xs_report_dialog", "Container_516", "Container_519");
            akit.ValueChange("xs_report_dialog", "display_created_report", "0");
            akit.ModalDialog(1);
            akit.PushButton("xs_report_all", "xs_report_dialog");
            akit.PushButton("xs_report_cancel", "xs_report_dialog");

            foreach(var objectStatAdress in objectList)
            {
                File.Copy(Path.Combine(model.GetInfo().ModelPath, "Отчеты", model.GetInfo().ModelName.Replace(".db1", ".csv")), Path.Combine(objectStatAdress, model.GetInfo().ModelName.Replace(".db1", "") + ".csv"), true);
                
            }
            bool nothingFound = true;
            if (incorrectNames.Count > 0 | correctNames.Count > 0)
            {
                nothingFound = false;
                var message = string.Empty;
                if (correctNames.Count > 0)
                    message += "Внесено в статистику: \r\n" + string.Join("\r\n", correctNames) +"\r\n";                
                if (incorrectNames.Count > 0)
                    message += "Проверьте имена файлов (латиница): \r\n" + string.Join("\r\n", incorrectNames);

                MessageBox.Show(message);
            }

            if(nothingFound)
                MessageBox.Show("Проверьте наименования файлов!");

            //wpf.View("CatalogTree.CatalogTreeView").InvokeCommand("CatalogInvokeItem", "CatalogPluginComponentItem?SMPluginModel");
            //akit.CommandEnd();
            //akit.CommandStart("ail_create_plugin", "SMPluginModel", "main_frame");
            //wpf.View("CatalogTree.CatalogTreeView").InvokeCommand("CatalogInvokeItem", "CatalogPluginComponentItem?SMPluginModel");
            //akit.CommandEnd();
            //akit.CommandStart("ail_create_plugin", "SMPluginModel", "main_frame");
        }
    }
}
