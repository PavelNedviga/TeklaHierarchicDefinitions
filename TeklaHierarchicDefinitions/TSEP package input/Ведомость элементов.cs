using System.Diagnostics;
using System;
using System.IO;
using System.Windows.Forms;
using Tekla.Structures;
using TSM = Tekla.Structures.Model;
using Tekla.Structures.Model;
using System.Linq;



namespace Tekla.Technology.Akit.UserScript
{
    public class Script
    {
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr handle);

        public static void Run(Tekla.Technology.Akit.IScript akit)
        {
            TSM.Model CurrentModel = new TSM.Model();
            string dir = string.Empty;
            TeklaStructuresSettings.GetAdvancedOption("XSDATADIR", ref dir);

            string ApplicationName = "THD.exe";
            string ApplicationPath = Path.Combine(dir, "Environments\\common\\extensions\\ElementList\\" + ApplicationName);

            Process NewProcess = new Process();


            if (File.Exists(ApplicationPath))
            {

                var isRunning = Process.GetProcessesByName("THD").Length > 0;
                if (!isRunning)
                {
                    NewProcess.StartInfo.FileName = ApplicationPath;
                    try
                    {
                        NewProcess.Start();
                        NewProcess.WaitForExit();
                    }
                    catch
                    {
                        MessageBox.Show(ApplicationName + " failed to start.");
                    }
                }
                else
                {
                    var p = Process.GetProcessesByName("THD").FirstOrDefault();
                    SetForegroundWindow(p.Handle);

                }
            }
            else
            {
                MessageBox.Show(ApplicationPath + " not found.");
            }
        }
    }
}