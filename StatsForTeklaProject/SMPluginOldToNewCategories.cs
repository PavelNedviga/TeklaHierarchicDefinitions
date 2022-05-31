#pragma warning disable 1633 // Unrecognized #pragma directive
#pragma reference "Tekla.Macros.Wpf.Runtime"
#pragma reference "Tekla.Macros.Akit"
#pragma reference "Tekla.Macros.Runtime"
#pragma warning restore 1633 // Unrecognized #pragma directive

using System.IO;
using Tekla.Structures;
using System.Text.RegularExpressions;
using Tekla.Structures.Model;
using Tekla.Structures.Model.UI;
//using Tekla.Structures.Dialogs;
using System.Windows.Forms;
using System.Collections.Generic;
using System;
using System.Collections;

namespace UserMacros
{
    public sealed class Macro
    {
        [Tekla.Macros.Runtime.MacroEntryPointAttribute()]
        public static void Run(Tekla.Macros.Runtime.IMacroRuntime runtime)
        {
            Tekla.Macros.Akit.IAkitScriptHost akit = runtime.Get<Tekla.Macros.Akit.IAkitScriptHost>();
            Tekla.Macros.Wpf.Runtime.IWpfMacroHost wpf = runtime.Get<Tekla.Macros.Wpf.Runtime.IWpfMacroHost>();
            Tekla.Macros.Akit.AkitMacroBackend akitMacroBackend = runtime.Get<Tekla.Macros.Akit.AkitMacroBackend>();

            var model = new Model();
            if (model.GetConnectionStatus())
            {
                List<int> array = new List<int>() { 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 };
                Dictionary<string, string> categoryMapping = new Dictionary<string, string>();

                var ht = new Hashtable();
                if(model.GetProjectInfo().GetStringUserProperties(ref ht))
                {
                    foreach (int i in array)
                    {
                        if(ht.ContainsKey("cm_kat_" + i.ToString()))
                            categoryMapping.Add(i.ToString(), ht["cm_kat_" + i.ToString()].ToString());
                        else
                            categoryMapping.Add(i.ToString(), "");
                    } 
                }

                Tekla.Structures.Model.UI.ModelObjectSelector modelObjectSelector = new Tekla.Structures.Model.UI.ModelObjectSelector();
                if(modelObjectSelector.GetSelectedObjects().GetSize() > 0)
                {
                    bool res = false;
                    foreach(var so in modelObjectSelector.GetSelectedObjects())
                    {
                        if(so is Part)
                        {
                            Part part = so as Part;
                            var cat = string.Empty;
                            if(!part.GetUserProperty("RU_BOM_CTG",ref cat))
                            {
                                int seqCatPos = -1;
                                if(part.GetUserProperty("cm_kat", ref seqCatPos))
                                {
                                    part.SetUserProperty("RU_BOM_CTG", categoryMapping[(seqCatPos +5).ToString()]);
                                    part.Modify();
                                    res = true;
                                }                                
                            }
                        }
                    }
                    model.CommitChanges("Категории обновлены");
                }                
            }
        }
    }
}
