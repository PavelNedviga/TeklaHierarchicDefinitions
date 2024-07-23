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

            string ApplicationConfigName = "THD.exe.config";
            string ApplicationConfigPath = Path.Combine(dir, "Environments\\common\\extensions\\ElementList\\" + ApplicationConfigName);

            //if (!File.Exists(ApplicationConfigPath))
            //{
                string ApplicationConfigPathCorrect = Path.Combine(dir, "Environments\\common\\extensions\\ElementList\\Configs\\" + versionShort + "-THD.exe.config");
                File.Copy(ApplicationConfigPathCorrect, ApplicationConfigPath, true);
                //if (TeklaStructuresInfo.GetCurrentProgramVersion().Contains("2021"))
                //    File.Copy(Path.Combine(dir, "nt\\bin\\TeklaStructures.exe.config"), ApplicationConfigPath);
                //else
                //    File.Copy(Path.Combine(dir, "bin\\TeklaStructures.exe.config"), ApplicationConfigPath);
            //}

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