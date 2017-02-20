using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watchdog
{
    class ToWatchdog
    {
        public static void Main()
        {
            Watchdog watchdog = new Watchdog();

            watchdog.Output(true);
        }
    }
}
