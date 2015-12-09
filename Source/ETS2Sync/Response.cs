using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETS2Sync
{
    public class Response
    {
        public class UploadFile
        {
            public bool success { get; set; }
            public string file_path { get; set; }
            public string f_path_synced { get; set; }
            public string size { get; set; }
        }
    }
}
