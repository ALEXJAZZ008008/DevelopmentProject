using System;
using System.Threading;
using System.Threading.Tasks;

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
