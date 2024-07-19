using NLog;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeklaHierarchicDefinitions.Logging
{
    public static class Logging
    {
        public static Logger Logs { get; }

        public static string AppDirectory { get => Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location); }

        static Logging()
        {
            var presets = Path.Combine(AppDirectory, "Logging/NLog.config");
            LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration(presets);
            Logs = LogManager.GetLogger("Logger");
        }

        public static void ExceptionLog(this Logger logger, Exception ex)
        {
            logger.Error(ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.TargetSite + "\r\n" + ex.HelpLink);
        }
    }
}
