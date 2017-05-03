using System.Threading;

namespace ToUDPServer
{
    class ToUDPServer
    {
        private static void Main()
        {
            UDPServer.UDPServer udpServer = new UDPServer.UDPServer();
            CancellationToken cancellationToken = new CancellationToken();

            udpServer.Input(cancellationToken);
        }
    }
}