using Android.App;
using Android.Content.PM;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Hardware;
using Android.OS;
using Android.Util;
using Android.Widget;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Android.Content;
using System;
using System.Text.RegularExpressions;

namespace CaptureTheCampus.Game
{
    [Activity(Label = "@string/GameActivityLabel", Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait)]
    public class GameActivity : Activity
    {
        public string gameType, ip;
        public int playerPosition, numberOfPlayers;

        private Utilities utilities;
        private CancellationTokenSource cancelationTokenSource;
        private Task finishTask, scoreTask, hazardTask, serverTask, heartbeatTask, clientTask;
        public TextView areaTextView, scoreTextView;
        public double initialArea;
        volatile public int area;
        volatile public bool finishBool;

        private GameMap gameMap;
        public GoogleMap googleMap;
        public GamePlayArea gamePlayArea;

        private GamePosition gamePosition;
        public GoogleApiClient googleAPIClient;
        public Player[] playerArray;

        private Rotation rotation;
        public SensorManager sensorManager;
        public float[] gravity, geoMagnetic;
        public float azimuth;

        private Hazards hazards;
        volatile public Circle circle;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Log.Debug("OnCreate", "OnCreate called, initializing views...");

            // Set our view from the "Game" layout resource
            SetContentView(Resource.Layout.Game);

            Initialise();

            StartMap();
            StartPosition();
            //StartRotation();

            StartTasks();
        }

        protected override void OnResume()
        {
            base.OnResume();
            Log.Debug("OnResume", "OnResume called, connecting to clients...");

            StartUpdatePosition();
            //StartUpdateRotation();
        }

        protected override void OnPause()
        {
            Log.Debug("OnPause", "OnPause called, stopping client updates");

            StopUpdatePosition();
            //StopUpdateRotation();

            base.OnPause();
        }

        private void Initialise()
        {
            gameType = Intent.GetStringExtra("gameType");

            utilities = new Utilities(this);

            cancelationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancelationTokenSource.Token;

            finishTask = new Task(() => FinishCheck(cancellationToken), cancelationTokenSource.Token);
            scoreTask = new Task(() => EndConditions(cancellationToken), cancelationTokenSource.Token);
            hazardTask = new Task(() => RunHazards(cancellationToken), cancelationTokenSource.Token);

            areaTextView = (TextView)FindViewById(Resource.Id.Area);
            scoreTextView = (TextView)FindViewById(Resource.Id.Score);
            area = 100;
            areaTextView.Text = "Area: " + area.ToString();
            finishBool = false;

            gameMap = new GameMap(this);

            gamePlayArea = new GamePlayArea();
            gamePlayArea.vertices = new LinkedList<LatLng>();

            GetVertices();

            gamePlayArea.polygons = new LinkedList<Polygon>();
            gamePlayArea.playAreaDrawnBool = false;

            gamePosition = new GamePosition(this);

            if (gameType == "Single Player")
            {
                numberOfPlayers = 1;

                playerArray = new Player[numberOfPlayers];

                playerPosition = numberOfPlayers - 1;
            }
            else
            {
                if (gameType == "Host")
                {
                    numberOfPlayers = int.Parse(Intent.GetStringExtra("numberOfPlayers"));

                    playerArray = new Player[numberOfPlayers];

                    playerPosition = int.Parse(Intent.GetStringExtra("playerPosition")) - 1;

                    Watchdog.Watchdog watchdog = new Watchdog.Watchdog();

                    serverTask = new Task(() => watchdog.Input(cancellationToken), cancelationTokenSource.Token);

                    heartbeatTask = new Task(() => HeartbeatTask(cancellationToken), cancelationTokenSource.Token);

                    clientTask = new Task(() => HostClientTask(cancellationToken), cancelationTokenSource.Token);
                }
                else
                {
                    if (gameType == "Join")
                    {
                        numberOfPlayers = int.Parse(Intent.GetStringExtra("numberOfPlayers"));

                        playerArray = new Player[numberOfPlayers];

                        playerPosition = int.Parse(Intent.GetStringExtra("playerPosition")) - 1;

                        ip = Intent.GetStringExtra("ip");

                        clientTask = new Task(() => JoinClientTask(cancellationToken), cancelationTokenSource.Token);
                    }
                }
            }

            for (int i = 0; i < numberOfPlayers; i++)
            {
                playerArray[i] = new Player();

                playerArray[i].score = 0;

                scoreTextView.Text = "Score: " + playerArray[i].score.ToString();

                playerArray[i].currentPosition = new LatLng(0, 0);
                playerArray[i].vertices = new LinkedList<LatLng>();
                playerArray[i].drawingBool = false;
                playerArray[i].positionBool = true;
                playerArray[i].deathBool = false;
            }

            rotation = new Rotation(this);
            gravity = new float[3];
            geoMagnetic = new float[3];
            azimuth = 0;

            hazards = new Hazards(this);
        }

        private void FinishCheck(CancellationToken cancellationToken)
        {
            while (true)
            {
                // Poll on this property if you have to do
                // other cleanup before throwing.
                if (cancellationToken.IsCancellationRequested)
                {
                    // Clean up here, then...
                    cancellationToken.ThrowIfCancellationRequested();
                }

                if (finishBool == true)
                {
                    cancelationTokenSource.Cancel();

                    Finish();

                    break;
                }

                Thread.Sleep(1000);
            }
        }

        private void EndConditions(CancellationToken cancellationToken)
        {
            while (true)
            {
                // Poll on this property if you have to do
                // other cleanup before throwing.
                if (cancellationToken.IsCancellationRequested)
                {
                    // Clean up here, then...
                    cancellationToken.ThrowIfCancellationRequested();
                }

                if (area <= 25 && area != 0)
                {
                    GoToScoreActivity();

                    break;
                }

                Thread.Sleep(1000);
            }
        }

        private void GoToScoreActivity()
        {
            Log.Debug("GoToScoreActivity", "GoToScoreActivity called, going to ScoreActivity...");

            Intent intent = new Intent(this, typeof(Score.ScoreActivity));
            intent.PutExtra("score", playerArray[playerPosition].score);
            StartActivity(intent);

            finishBool = true;
        }

        private void RunHazards(CancellationToken cancellationToken)
        {
            while (true)
            {
                // Poll on this property if you have to do
                // other cleanup before throwing.
                if (cancellationToken.IsCancellationRequested)
                {
                    // Clean up here, then...
                    cancellationToken.ThrowIfCancellationRequested();
                }

                if (circle != null)
                {
                    break;
                }

                Thread.Sleep(1000);
            }

            hazards.SetCircle(circle);

            LinkedList<LatLng> temporaryPlayAreaVertices = new LinkedList<LatLng>();

            while (true)
            {
                // Poll on this property if you have to do
                // other cleanup before throwing.
                if (cancellationToken.IsCancellationRequested)
                {
                    // Clean up here, then...
                    cancellationToken.ThrowIfCancellationRequested();
                }

                CopyVertices(temporaryPlayAreaVertices, gamePlayArea.vertices);

                hazards.RunHazards(temporaryPlayAreaVertices);

                Thread.Sleep(1000);
            }
        }

        public void CopyVertices(LinkedList<LatLng> temporaryVertices, LinkedList<LatLng> inVertices)
        {
            LatLng[] temporaryArray = new LatLng[inVertices.Count];

            inVertices.CopyTo(temporaryArray, 0);

            temporaryVertices.Clear();

            for (int i = 0; i < temporaryArray.Length; i++)
            {
                temporaryVertices.AddLast(temporaryArray[i]);
            }
        }

        public void CopyBool(out bool temporaryBool, bool deathBool)
        {
            temporaryBool = deathBool;
        }

        public void KillPlayer(int killedPlayerPosition)
        {
            playerArray[killedPlayerPosition].score = playerArray[killedPlayerPosition].score / 2;

            playerArray[killedPlayerPosition].polyline.Remove();

            playerArray[killedPlayerPosition].drawingBool = false;
            playerArray[killedPlayerPosition].positionBool = false;
            playerArray[killedPlayerPosition].deathBool = true;
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

                try
                {
                    client.Input(new string[] { "-t", "dateTime", (DateTime.Now + TimeSpan.FromMilliseconds(3000)).ToString() });
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

        private void HostClientTask(CancellationToken cancellationToken)
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

            client.Input(new string[] { "-t", "start", "true" });

            client.Input(new string[] { "-t", "-i", ip, "player" + playerPosition.ToString(), Static.Serialise.SerialiseLatLng(playerArray[playerPosition].currentPosition) });

            client.Input(new string[] { "-t", "playArea", Static.Serialise.SerialiseLatLngLinkedList(gamePlayArea.vertices) });

            while (true)
            {
                // Poll on this property if you have to do
                // other cleanup before throwing.
                if (cancellationToken.IsCancellationRequested)
                {
                    // Clean up here, then...
                    cancellationToken.ThrowIfCancellationRequested();
                }

                Parallel.For(0, numberOfPlayers, i =>
                {
                    if (i != playerPosition)
                    {
                        string temporaryString;

                        // Poll on this property if you have to do
                        // other cleanup before throwing.
                        if (cancellationToken.IsCancellationRequested)
                        {
                            // Clean up here, then...
                            cancellationToken.ThrowIfCancellationRequested();
                        }

                        try
                        {
                            playerArray[i].currentPosition = Static.Serialise.DeserialiseLatLng(Regex.Split(client.Input(new string[] { "-t", "player" + i.ToString() }), ": ")[1], out temporaryString);

                            RunOnUiThread(() => utilities.UpdateLocationInformation(i, playerArray[i].currentPosition));
                        }
                        catch (Exception ex)
                        {
#if DEBUG
                            //This prints to the screen an error message
                            Console.WriteLine("ERROR: " + ex.ToString());
#endif
                        }
                    }
                    else
                    {
                        client.Input(new string[] { "-t", "player" + i.ToString(), Static.Serialise.SerialiseLatLng(playerArray[i].currentPosition) });
                    }
                });

                client.Input(new string[] { "-t", "circle", Static.Serialise.SerialiseLatLng(circle.Center) });

                Thread.Sleep(1000);
            }
        }

        private void JoinClientTask(CancellationToken cancellationToken)
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

            client.Input(new string[] { "-t", "-i", ip, "player" + playerPosition.ToString(), Static.Serialise.SerialiseLatLng(playerArray[playerPosition].currentPosition) });

            string temporaryString;

            gamePlayArea.vertices = Static.Serialise.DeserialiseLatLngLinkedList(Regex.Split(client.Input(new string[] { "-t", "-i", ip, "playArea" }), ": ")[1]);

            initialArea = Static.Maths.PolygonArea(gamePlayArea.vertices);
            area = (int)((Static.Maths.PolygonArea(gamePlayArea.vertices) / initialArea) * 100);

            while (true)
            {
                // Poll on this property if you have to do
                // other cleanup before throwing.
                if (cancellationToken.IsCancellationRequested)
                {
                    // Clean up here, then...
                    cancellationToken.ThrowIfCancellationRequested();
                }

                Parallel.For(0, numberOfPlayers, i =>
                {
                    if (i != playerPosition)
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
                            playerArray[i].currentPosition = Static.Serialise.DeserialiseLatLng(Regex.Split(client.Input(new string[] { "-t", "-i", ip, "player" + i.ToString() }), ": ")[1], out temporaryString);

                            RunOnUiThread(() => utilities.UpdateLocationInformation(i, playerArray[i].currentPosition));
                        }
                        catch (Exception ex)
                        {
#if DEBUG
                            //This prints to the screen an error message
                            Console.WriteLine("ERROR: " + ex.ToString());
#endif
                        }
                    }
                    else
                    {
                        client.Input(new string[] { "-t", "player" + i.ToString(), Static.Serialise.SerialiseLatLng(playerArray[i].currentPosition) });
                    }
                });

                circle.Center = Static.Serialise.DeserialiseLatLng(Regex.Split(client.Input(new string[] { "-t", "-i", ip, "circle", }), ": ")[1], out temporaryString);

                Thread.Sleep(1000);
            }
        }

        private void GetVertices()
        {
            if (gameType == "Single Player" || gameType == "Host")
            {
                gamePlayArea.verticesNode = gamePlayArea.vertices.AddFirst(new LatLng(Intent.GetDoubleExtra("markerOneLatitude", 0), Intent.GetDoubleExtra("markerOneLongitude", 0)));
                gamePlayArea.verticesNode = gamePlayArea.vertices.AddAfter(gamePlayArea.verticesNode, new LatLng(Intent.GetDoubleExtra("markerTwoLatitude", 0), Intent.GetDoubleExtra("markerTwoLongitude", 0)));

                gamePlayArea.vertices.AddBefore(gamePlayArea.verticesNode, new LatLng(gamePlayArea.vertices.First.Value.Latitude, gamePlayArea.vertices.Last.Value.Longitude));
                gamePlayArea.vertices.AddLast(new LatLng(gamePlayArea.vertices.Last.Value.Latitude, gamePlayArea.vertices.First.Value.Longitude));
            }
            else
            {
                gamePlayArea.verticesNode = gamePlayArea.vertices.AddFirst(new LatLng(1, 1));
                gamePlayArea.verticesNode = gamePlayArea.vertices.AddAfter(gamePlayArea.verticesNode, new LatLng(-1, -1));

                gamePlayArea.vertices.AddBefore(gamePlayArea.verticesNode, new LatLng(gamePlayArea.vertices.First.Value.Latitude, gamePlayArea.vertices.Last.Value.Longitude));
                gamePlayArea.vertices.AddLast(new LatLng(gamePlayArea.vertices.Last.Value.Latitude, gamePlayArea.vertices.First.Value.Longitude));
            }
        }

        private void StartMap()
        {
            Log.Debug("StartMap", "Starting map");

            FragmentManager.FindFragmentById<MapFragment>(Resource.Id.GameMap).GetMapAsync(gameMap);
        }

        private void StartPosition()
        {
            Log.Debug("StartPosition", "Starting position client");

            // pass in the Context, ConnectionListener and ConnectionFailedListener
            googleAPIClient = new GoogleApiClient.Builder(this, gamePosition, gamePosition).AddApi(LocationServices.API).Build();
        }

        private void StartRotation()
        {
            Log.Debug("StartRotation", "Starting rotation client");

            sensorManager = (SensorManager)GetSystemService(SensorService);
        }

        private void StartTasks()
        {
            finishTask.Start();
            scoreTask.Start();
            hazardTask.Start();

            if (gameType == "Host")
            {
                serverTask.Start();
                heartbeatTask.Start();
                clientTask.Start();
            }
            else
            {
                if (gameType == "Join")
                {
                    clientTask.Start();
                }
            }
        }

        private void StartUpdatePosition()
        {
            Log.Debug("StartUpdatePosition", "Starting updates from position client");

            googleAPIClient.Connect();
        }

        private void StartUpdateRotation()
        {
            Log.Debug("StartUpdateRotation", "Starting updates from rotation client");

            sensorManager.RegisterListener(rotation, sensorManager.GetDefaultSensor(SensorType.MagneticField), SensorDelay.Game);
            sensorManager.RegisterListener(rotation, sensorManager.GetDefaultSensor(SensorType.Accelerometer), SensorDelay.Game);
        }

        private void StopUpdatePosition()
        {
            Log.Debug("StopUpdatePosition", "Stopping updates from position client");

            // stop location updates, passing in the LocationListener
            LocationServices.FusedLocationApi.RemoveLocationUpdates(googleAPIClient, gamePosition);

            googleAPIClient.Disconnect();
        }

        private void StopUpdateRotation()
        {
            Log.Debug("StopUpdateRotation", "Stopping updates from rotation client");

            sensorManager.UnregisterListener(rotation);
        }
    }
}