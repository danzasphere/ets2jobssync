using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Collections;
using log4net;
using System.IO;

namespace Palow.Library.Config
{
    /// <summary>
    /// Beispielconfig
    /// </summary>
    /// 
    public class ExampleConfig : ConfigBase, IConfig
    {
        public string Version { get { return "1"; } set { } }
        public String ObjectType { get { return System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name; } set { } } // Must Contain correct end-ClassName
        public string ObjectSubType { get { return ""; } set { } }

        public class S_General
        {
            public String PersonFirstName { get; set; }
            public String PersonLastName { get; set; }
            public String PersonWebsite { get; set; }


            public S_General()
            {
                this.PersonFirstName = "Hans";
                this.PersonLastName = "Müller";
                this.PersonWebsite = "http://palow.org";
            }
        }

        public S_General General { get; set; }


        // ******************************* Constructor mit den Default-Values für alle Properties
        public ExampleConfig()
        {
            this.General = new S_General();
        }

        public static ExampleConfig LoadConfig(string assemblyPath)
        {
            return LoadConfig(assemblyPath, true);
        }

        public static ExampleConfig LoadConfig(String assemblyPath, Boolean UseConfigDir)
        {
            ILog lg = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            string pd = assemblyPath;
            if (UseConfigDir)
            {
                pd = System.IO.Path.Combine(pd, "..", "Config");
            }
            string fn = System.IO.Path.Combine(pd, "Palow.DefaultConfig.xml");
            lg.Debug(String.Format("Using Path:'{0}'", fn));
            ConfigXmlFile cxf = new ConfigXmlFile(fn, true);
            ExampleConfig cfg = cxf.Get<ExampleConfig>();
            return cfg;
        }

        public static void SaveConfig(string assemblyPath, ExampleConfig cfg)
        {
            SaveConfig(assemblyPath, cfg, true);
        }

        public static void SaveConfig(String assemblyPath, ExampleConfig cfg, Boolean UseConfigDir)
        {
            ILog lg = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            string pd = assemblyPath;
            if (UseConfigDir)
            {
                pd = System.IO.Path.Combine(pd, "..", "Config");
                if (!Directory.Exists(pd))
                {
                    Directory.CreateDirectory(pd);
                }
            }
            string fn = System.IO.Path.Combine(pd, "Palow.DefaultConfig.xml");
            lg.Debug(String.Format("Using Path:'{0}'", fn));
            ConfigXmlFile cxf = new ConfigXmlFile(fn, false);
            cxf.Save(cfg);
        }
    }

}


