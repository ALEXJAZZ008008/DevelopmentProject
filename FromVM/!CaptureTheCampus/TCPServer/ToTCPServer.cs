using System.Threading;

namespace TCPServer
{
    class ToTCPServer
    {
        private static void Main()
        {
            TCPServer tcpServer = new TCPServer();
            CancellationToken ct = new CancellationToken();

            tcpServer.Input(ct);
        }
    }
}
