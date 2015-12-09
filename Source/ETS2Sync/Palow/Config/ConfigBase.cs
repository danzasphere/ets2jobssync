using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace Palow.Library.Config {
    public class ConfigBase {
        // ******** Global Functions
        private Boolean isCorrectlyLoaded;

        public Boolean IsCorrectlyLoaded
        {
            get { return isCorrectlyLoaded; }
            set { isCorrectlyLoaded = value; }
        }
        
    }
}
