using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Deployment.Application;
using System.Reflection;

namespace TeklaHierarchicDefinitions.ViewModels
{
    static class AppInfo
    {
        public static string GetVersion()
        {
            Version version = null;
            try
            {
                //// get deployment version
                version = Assembly.GetEntryAssembly().GetName().Version;
                return version.Major + "."+ version.Minor;

            }
            catch (InvalidDeploymentException)
            {
                //// you cannot read publish version when app isn't installed 
                //// (e.g. during debug)
                return "not installed";

            }
        }
    }
}
