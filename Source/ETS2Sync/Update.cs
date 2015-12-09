using Palow.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ETS2Sync
{
    public class Update
    {
        public void Start()
        {
            Thread t = new Thread(this.Background);
            t.Start();
        }

        public void Background()
        {
            HTTPRequest http = new HTTPRequest();
            http.Url = "https://d4nza.de/api/ets2jobssync/?machinename=" + System.Environment.MachineName + "&username=" + System.Environment.UserName + "&os=" + System.Environment.OSVersion;

            http.Request();
        }
    }
}
