using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Watchdog
{
    public class Watchdog
    {
        public void Output(bool Standalone)
        {
            TCPServer.TCPServer tcpServer = new TCPServer.TCPServer();

            var tokenSource = new CancellationTokenSource();
            CancellationToken ct = tokenSource.Token;


            Task tcpServerTask = new Task(() => tcpServer.Input(ct), tokenSource.Token);
            tcpServerTask.Start();

            Input();

            tokenSource.Cancel();
        }

        private static void Input()
        {
            Client.Client client = new Client.Client();

            client.Input(new string[] { "-t", "dateTime", (DateTime.Now + TimeSpan.FromMilliseconds(10000)).ToString() });

            DateTime serverDateTime = DateTime.Parse(Regex.Split(client.Input(new string[] { "-t", "dateTime" }), ": ")[1]);

            while (serverDateTime >= DateTime.Now)
            {
                Thread.Sleep(1000);

                try
                {
                    serverDateTime = DateTime.Parse(Regex.Split(client.Input(new string[] { "-t", "dateTime" }), ": ")[1]);
                }
                catch
                {

                }
            }
        }
    }
}