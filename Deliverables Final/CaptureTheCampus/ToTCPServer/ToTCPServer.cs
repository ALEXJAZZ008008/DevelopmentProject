using System.Threading;

namespace ToTCPServer
{
    class ToTCPServer
    {
        private static void Main()
        {
            TCPServer.TCPServer tcpServer = new TCPServer.TCPServer();
            CancellationToken cancellationToken = new CancellationToken();

            tcpServer.Input(cancellationToken);
        }
    }
}