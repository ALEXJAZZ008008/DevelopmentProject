using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Watchdog
{
    public class Watchdog
    {
        CancellationTokenSource cancelationTokenSource;

        public void Input(CancellationToken parentCancellationToken)
        {
            TCPServer.TCPServer tcpServer = new TCPServer.TCPServer();

            cancelationTokenSource = new CancellationTokenSource();
            CancellationToken childCancellationToken = cancelationTokenSource.Token;

            Task tcpServerTask = new Task(() => tcpServer.Input(childCancellationToken), cancelationTokenSource.Token);
            tcpServerTask.Start();

            Output(parentCancellationToken);

            cancelationTokenSource.Cancel();
        }

        private void Output(CancellationToken cancellationToken)
        {
            bool notStartedBool = true;
            Client.Client client = new Client.Client();

            while (notStartedBool)
            {
                // Poll on this property if you have to do
                // other cleanup before throwing.
                if (cancellationToken.IsCancellationRequested)
                {
                    // Clean up here, then...
                    cancellationToken.ThrowIfCancellationRequested();
                }

                notStartedBool = false;

                try
                {
                    client.Input(new string[] { "-t", "test", "123" });

                    int.Parse(Regex.Split(client.Input(new string[] { "-t", "test" }), ": ")[1]);
                }
                catch (Exception ex)
                {
#if DEBUG
                    //This prints to the screen an error message
                    Console.WriteLine("ERROR: " + ex.ToString());
#endif

                    notStartedBool = true;
                }

                Thread.Sleep(1000);
            }

            Console.WriteLine(DateTime.Now + TimeSpan.FromMilliseconds(10000));

            client.Input(new string[] { "-t", "dateTime", (DateTime.Now + TimeSpan.FromMilliseconds(10000)).ToString() });

            Console.WriteLine(Regex.Split(client.Input(new string[] { "-t", "dateTime" }), ": ")[1]);

            DateTime serverDateTime = DateTime.Parse(Regex.Split(client.Input(new string[] { "-t", "dateTime" }), ": ")[1]);

            //while (serverDateTime >= DateTime.Now)
            while(true)
            {
                // Poll on this property if you have to do
                // other cleanup before throwing.
                if (cancellationToken.IsCancellationRequested)
                {
                    cancelationTokenSource.Cancel();

                    // Clean up here, then...
                    cancellationToken.ThrowIfCancellationRequested();
                }

                Thread.Sleep(1000);

                try
                {
                    serverDateTime = DateTime.Parse(Regex.Split(client.Input(new string[] { "-t", "dateTime" }), ": ")[1]);
                }
                catch (Exception ex)
                {
#if DEBUG
                    //This prints to the screen an error message
                    Console.WriteLine("ERROR: " + ex.ToString());
#endif
                }
            }
        }
    }
}