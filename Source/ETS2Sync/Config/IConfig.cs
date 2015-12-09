using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Palow.Library.Config
{
    public interface IConfig
    {
        Boolean IsCorrectlyLoaded { get; set; }
    }
}
