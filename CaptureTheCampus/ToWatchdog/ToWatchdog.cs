using System.Threading;

namespace ToWatchdog
{
    class ToWatchdog
    {
        private static void Main()
        {
            Watchdog.Watchdog watchdog = new Watchdog.Watchdog();
            CancellationToken cancellationToken = new CancellationToken();

            watchdog.Input(cancellationToken);
        }
    }
}