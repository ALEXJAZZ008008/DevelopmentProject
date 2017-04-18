using Android.Content;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CaptureTheCampus
{
    public class Host
    {
        private SearchActivity searchActivity;
        private Context context;

        private Task tcpServerTask, heartbeatTask, udpServerTask, clientTask;

        public Host(Context inContext)
        {
            searchActivity = (SearchActivity)inContext;

            context = inContext;

            Initialise();

            StartHost();
        }

        private void Initialise()
        {
            Watchdog.Watchdog watchdog = new Watchdog.Watchdog();

            searchActivity.cancelationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = searchActivity.cancelationTokenSource.Token;

            tcpServerTask = new Task(() => watchdog.Input(cancellationToken), searchActivity.cancelationTokenSource.Token);

            heartbeatTask = new Task(() => HeartbeatTask(cancellationToken), searchActivity.cancelationTokenSource.Token);

            UDPServer.UDPServer udpServer = new UDPServer.UDPServer();

            udpServerTask = new Task(() => udpServer.Input(cancellationToken), searchActivity.cancelationTokenSource.Token);

            clientTask = new Task(() => ClientTask(cancellationToken), searchActivity.cancelationTokenSource.Token);
        }

        private void HeartbeatTask(CancellationToken cancellationToken)
        {
            Client.Client client = new Client.Client();

            while (true)
            {
                // Poll on this property if you have to do
                // other cleanup before throwing.
                if (cancellationToken.IsCancellationRequested)
                {
                    // Clean up here, then...
                    cancellationToken.ThrowIfCancellationRequested();
                }

                client.Input(new string[] { "-t", "dateTime", (DateTime.Now + TimeSpan.FromMilliseconds(3000)).ToString() });

                Thread.Sleep(1000);
            }
        }

        private void ClientTask(CancellationToken cancellationToken)
        {
            Client.Client client = new Client.Client();

            string test;

            while(true)
            {
                // Poll on this property if you have to do
                // other cleanup before throwing.
                if (cancellationToken.IsCancellationRequested)
                {
                    // Clean up here, then...
                    cancellationToken.ThrowIfCancellationRequested();
                }

                client.Input(new string[] { "-t", "test", "123" });

                test = Regex.Split(client.Input(new string[] { "-t", "test" }), ": ")[1];

                if(test == "123")
                {
                    break;
                }

                Thread.Sleep(1000);
            }

            client.Input(new string[] { "-t", "start", "false" });
            client.Input(new string[] { "-t", "numberOfPlayers", "1" });

            searchActivity.playerPosition = 1;
            searchActivity.numberOfPlayers = 1;

            searchActivity.RunOnUiThread(() => searchActivity.searchButton.Text = context.Resources.GetString(Resource.String.HostSearchButtonLabelPlay) + " " + searchActivity.numberOfPlayers + " " + context.Resources.GetString(Resource.String.HostSearchButtonLabelPlayers));

            int temporaryNumberOfPlayers;

            while (true)
            {
                // Poll on this property if you have to do
                // other cleanup before throwing.
                if (cancellationToken.IsCancellationRequested)
                {
                    // Clean up here, then...
                    cancellationToken.ThrowIfCancellationRequested();
                }

                try
                {
                    temporaryNumberOfPlayers = int.Parse(Regex.Split(client.Input(new string[] { "-t", "numberOfPlayers" }), ": ")[1]);

                    if (temporaryNumberOfPlayers > searchActivity.numberOfPlayers)
                    {
                        searchActivity.numberOfPlayers = temporaryNumberOfPlayers;

                        searchActivity.RunOnUiThread(() => searchActivity.searchButton.Text = context.Resources.GetString(Resource.String.HostSearchButtonLabelPlay) + " " + searchActivity.numberOfPlayers + " " + context.Resources.GetString(Resource.String.HostSearchButtonLabelPlayers));
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    //This prints to the screen an error message
                    Console.WriteLine("ERROR: " + ex.ToString());
#endif
                }

                Thread.Sleep(1000);
            }
        }

        public void StartHost()
        {
            searchActivity.ip = IP.GetIP();

            searchActivity.searchTextView.Text = context.Resources.GetString(Resource.String.HostSearchTitle) + " " + searchActivity.ip;

            searchActivity.searchButton.Text = context.Resources.GetString(Resource.String.HostSearchButtonLabelPlay) + " " + context.Resources.GetString(Resource.String.HostSearchButtonLabelPlayers);

            StartTasks();
        }

        private void StartTasks()
        {
            tcpServerTask.Start();
            heartbeatTask.Start();
            clientTask.Start();
            udpServerTask.Start();
        }
    }
}