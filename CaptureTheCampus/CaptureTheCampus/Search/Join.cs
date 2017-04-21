using Android.Content;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CaptureTheCampus.Search
{
    public class Join
    {
        private SearchActivity searchActivity;
        private Context context;

        private Task clientTask;

        public Join(Context inContext)
        {
            searchActivity = (SearchActivity)inContext;

            context = inContext;

            Initialise();

            StartJoin();
        }

        private void Initialise()
        {
            searchActivity.cancelationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = searchActivity.cancelationTokenSource.Token;

            clientTask = new Task(() => ClientTask(cancellationToken), searchActivity.cancelationTokenSource.Token);
        }

        private void ClientTask(CancellationToken cancellationToken)
        {
            string temporaryIP = string.Empty;
            Client.Client client = new Client.Client();

            while (searchActivity.ip == string.Empty)
            {
                // Poll on this property if you have to do
                // other cleanup before throwing.
                if (cancellationToken.IsCancellationRequested)
                {
                    // Clean up here, then...
                    cancellationToken.ThrowIfCancellationRequested();
                }

                temporaryIP = client.Input(new string[] { "-u" });

                int temporaryInt;

                if (int.TryParse(temporaryIP.Split(new char[] { '.' })[0], out temporaryInt))
                {
                    searchActivity.ip = temporaryIP;
                }

                if (searchActivity.ip != string.Empty)
                {
                    try
                    {
                        client.Input(new string[] { "-t", "-i", searchActivity.ip, "test", "123" });

                        int.Parse(Regex.Split(client.Input(new string[] { "-t", "-i", searchActivity.ip, "test" }), ": ")[1]);
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        //This prints to the screen an error message
                        Console.WriteLine("ERROR: " + ex.ToString());
#endif

                        searchActivity.ip = string.Empty;
                    }
                }

                Thread.Sleep(1000);
            }

            searchActivity.playerPosition = int.Parse(Regex.Split(client.Input(new string[] { "-t", "-i", searchActivity.ip, "numberOfPlayers" }), ": ")[1]) + 1;

            searchActivity.numberOfPlayers = searchActivity.playerPosition;

            client.Input(new string[] { "-t", "-i", searchActivity.ip, "numberOfPlayers", searchActivity.numberOfPlayers.ToString() });

            searchActivity.RunOnUiThread(() => searchActivity.searchTextView.Text = context.Resources.GetString(Resource.String.JoinSearchTitleConnected) + " " + context.Resources.GetString(Resource.String.JoinSearchTitleWaiting));

            while (true)
            {
                // Poll on this property if you have to do
                // other cleanup before throwing.
                if (cancellationToken.IsCancellationRequested)
                {
                    // Clean up here, then...
                    cancellationToken.ThrowIfCancellationRequested();
                }

                if (Regex.Split(client.Input(new string[] { "-t", "-i", searchActivity.ip, "start" }), ": ")[1] == "true")
                {
                    searchActivity.GoToGameActivity(searchActivity.searchType);
                }

                Thread.Sleep(1000);
            }
        }

        public void StartJoin()
        {
            searchActivity.searchTextView.Text = context.Resources.GetString(Resource.String.JoinSearchTitleWait) + " " + context.Resources.GetString(Resource.String.JoinSearchTitleNotConnected);

            searchActivity.searchButton.Text = context.Resources.GetString(Resource.String.JoinSearchButtonLabel);

            StartTasks();
        }

        private void StartTasks()
        {
            clientTask.Start();
        }
    }
}