using Palow.Library;
using Palow.Library.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETS2Sync
{
    public class ETS2SyncConfig : ConfigBase, IConfig
    {
        public string Version { get { return "1"; } set { } }
        public String ObjectType { get { return System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name; } set { } } // Must Contain correct end-ClassName
        public string ObjectSubType { get { return ""; } set { } }

        public class Ets2General
        {
            public Boolean PlaySound { get; set; }
            public Boolean BackupSaveGame { get; set; }

        }

        public Ets2General General { get; set; }


        // ******************************* Constructor mit den Default-Values für alle Properties
        public ETS2SyncConfig()
        {
            this.General = new Ets2General();
            this.General.PlaySound = true;
            this.General.BackupSaveGame = true;


        }

        public static ETS2SyncConfig LoadConfig(string assemblyPath)
        {
            return LoadConfig(assemblyPath, true);
        }

        public static ETS2SyncConfig LoadConfig(String assemblyPath, Boolean UseSettingsDir)
        {
            //ILog lg = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            string pd = assemblyPath;
            if (UseSettingsDir)
            {
                pd = System.IO.Path.Combine(pd, "Settings");
            }
            string fn = System.IO.Path.Combine(pd, "Settings.xml");
            //lg.Debug(String.Format("Using Path:'{0}'", fn));
            ETS2SyncConfig cfg;
            try
            {
                if (File.Exists(fn))
                {
                    ConfigXmlFile cxf = new ConfigXmlFile(fn, true);
                    cfg = cxf.Get<ETS2SyncConfig>();
                }
                else
                {
                    cfg = new ETS2SyncConfig();
                    ETS2SyncConfig.SaveConfig(Functions.GetAssemblyDirectory(), cfg);
                }


            }
            catch (Exception e)
            {
                cfg = new ETS2SyncConfig();

            }

            return cfg;
        }

        public static void SaveConfig(string assemblyPath, ETS2SyncConfig cfg)
        {
            SaveConfig(assemblyPath, cfg, true);
        }

        public static void SaveConfig(String assemblyPath, ETS2SyncConfig cfg, Boolean UseSettingsDir)
        {

            string pd = assemblyPath;
            if (UseSettingsDir)
            {
                pd = System.IO.Path.Combine(pd, "Settings");
                if (!Directory.Exists(pd))
                {
                    Directory.CreateDirectory(pd);
                }
            }
            string fn = System.IO.Path.Combine(pd, "Settings.xml");
            ConfigXmlFile cxf = new ConfigXmlFile(fn, false);
            cxf.Save(cfg);
        }
    }
}
