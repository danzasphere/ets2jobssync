using Palow.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ETS2Sync
{
    public static class Rn
    {
        private static ETS2SyncConfig cfg;
        public static ETS2SyncConfig Config
        {
            get
            {
                if (cfg == null)
                {
                    try
                    {
                        cfg = ETS2SyncConfig.LoadConfig(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

                    }
                    catch (Exception e)
                    {
                        cfg = new ETS2SyncConfig();
                        ETS2SyncConfig.SaveConfig(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), cfg);
                    }
                }
                return cfg;
            }
            set { cfg = value; }
        }
    }
}
