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

namespace CaptureTheCampus
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
        public double? initialArea;
        volatile public int area;
        volatile public bool finishBool;

        private GameMap mapClass;
        public GoogleMap map;
        public PlayArea playArea;

        private GamePosition positionClass;
        public GoogleApiClient apiClient;
        public Player[] player;

        private Rotation rotationClass;
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

            mapClass = new GameMap(this);

            playArea = new PlayArea();
            playArea.vertices = new LinkedList<LatLng>();

            GetVertices();

            playArea.polygons = new LinkedList<Polygon>();
            playArea.playAreaDrawnBool = false;

            positionClass = new GamePosition(this);

            if (gameType == "Single Player")
            {
                numberOfPlayers = 1;

                player = new Player[numberOfPlayers];

                playerPosition = numberOfPlayers - 1;
            }
            else
            {
                if (gameType == "Host")
                {
                    numberOfPlayers = int.Parse(Intent.GetStringExtra("numberOfPlayers"));

                    player = new Player[numberOfPlayers];

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

                        player = new Player[numberOfPlayers];

                        playerPosition = int.Parse(Intent.GetStringExtra("playerPosition")) - 1;

                        ip = Intent.GetStringExtra("ip");

                        clientTask = new Task(() => JoinClientTask(cancellationToken), cancelationTokenSource.Token);
                    }
                }
            }

            for (int i = 0; i < numberOfPlayers; i++)
            {
                player[i] = new Player();

                player[i].score = 0;

                scoreTextView.Text = "Score: " + player[i].score.ToString();

                player[i].currentPosition = new LatLng(0, 0);
                player[i].vertices = new LinkedList<LatLng>();
                player[i].drawingBool = false;
                player[i].positionBool = true;
                player[i].deathBool = false;
            }

            rotationClass = new Rotation(this);
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

                if (initialArea != null)
                {
                    if (area <= 25 && area != 0)
                    {
                        GoToScoreActivity();

                        break;
                    }
                }

                Thread.Sleep(1000);
            }
        }

        private void GoToScoreActivity()
        {
            Log.Debug("GoToScoreActivity", "GoToScoreActivity called, going to ScoreActivity...");

            Intent intent = new Intent(this, typeof(ScoreActivity));
            intent.PutExtra("score", player[playerPosition].score);
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

                CopyVertices(temporaryPlayAreaVertices, playArea.vertices);

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
            player[killedPlayerPosition].score = player[killedPlayerPosition].score / 2;

            player[killedPlayerPosition].polyline.Remove();

            player[killedPlayerPosition].drawingBool = false;
            player[killedPlayerPosition].positionBool = false;
            player[killedPlayerPosition].deathBool = true;
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

        private void HostClientTask(CancellationToken cancellationToken)
        {
            Client.Client client = new Client.Client();

            string test;

            while (true)
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

                if (test == "123")
                {
                    break;
                }

                Thread.Sleep(1000);
            }

            client.Input(new string[] { "-t", "start", "true" });

            client.Input(new string[] { "-t", "-i", ip, "player" + playerPosition.ToString(), Serialise.SerialiseLatLng(player[playerPosition].currentPosition) });

            client.Input(new string[] { "-t", "playArea", Serialise.SerialiseLatLngLinkedList(playArea.vertices) });

            while (initialArea == null)
            {
                // Poll on this property if you have to do
                // other cleanup before throwing.
                if (cancellationToken.IsCancellationRequested)
                {
                    // Clean up here, then...
                    cancellationToken.ThrowIfCancellationRequested();
                }

                if (initialArea != null)
                {
                    break;
                }

                Thread.Sleep(1000);
            }

            client.Input(new string[] { "-t", "initialArea", Serialise.SerialiseString(initialArea.ToString()) });

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
                        bool onlineBool = false;

                        while (!onlineBool)
                        {
                           // Poll on this property if you have to do
                           // other cleanup before throwing.
                           if (cancellationToken.IsCancellationRequested)
                            {
                               // Clean up here, then...
                               cancellationToken.ThrowIfCancellationRequested();
                            }

                            onlineBool = true;

                            try
                            {
                                player[i].currentPosition = Serialise.DeserialiseLatLng(Regex.Split(client.Input(new string[] { "-t", "player" + i.ToString() }), ": ")[1], out temporaryString);

                                RunOnUiThread(() => utilities.UpdateLocationInformation(i, player[i].currentPosition));
                            }
                            catch (Exception ex)
                            {
#if DEBUG
                               //This prints to the screen an error message
                               Console.WriteLine("ERROR: " + ex.ToString());
#endif

                               onlineBool = false;
                            }

                            Thread.Sleep(1000);
                        }
                    }
                    else
                    {
                        client.Input(new string[] { "-t", "player" + i.ToString(), Serialise.SerialiseLatLng(player[i].currentPosition) });
                    }
                });

                client.Input(new string[] { "-t", "circle", Serialise.SerialiseLatLng(circle.Center) });

                Thread.Sleep(1000);
            }
        }

        private void JoinClientTask(CancellationToken cancellationToken)
        {
            Client.Client client = new Client.Client();

            string test;

            while (true)
            {
                // Poll on this property if you have to do
                // other cleanup before throwing.
                if (cancellationToken.IsCancellationRequested)
                {
                    // Clean up here, then...
                    cancellationToken.ThrowIfCancellationRequested();
                }

                client.Input(new string[] { "-t", "-i", ip, "test", "123" });

                test = Regex.Split(client.Input(new string[] { "-t", "-i", ip, "test" }), ": ")[1];

                if (test == "123")
                {
                    break;
                }

                Thread.Sleep(1000);
            }

            client.Input(new string[] { "-t", "-i", ip, "player" + playerPosition.ToString(), Serialise.SerialiseLatLng(player[playerPosition].currentPosition) });

            string temporaryString;

            playArea.vertices = Serialise.DeserialiseLatLngLinkedList(Regex.Split(client.Input(new string[] { "-t", "-i", ip, "playArea" }), ": ")[1]);

            initialArea = double.Parse(Serialise.DeserialiseString(Regex.Split(client.Input(new string[] { "-t", "-i", ip, "initialArea", }), ": ")[1], out temporaryString));

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
                        bool onlineBool = false;

                        while (!onlineBool)
                        {
                            // Poll on this property if you have to do
                            // other cleanup before throwing.
                            if (cancellationToken.IsCancellationRequested)
                            {
                                // Clean up here, then...
                                cancellationToken.ThrowIfCancellationRequested();
                            }

                            onlineBool = true;

                            try
                            {
                                player[i].currentPosition = Serialise.DeserialiseLatLng(Regex.Split(client.Input(new string[] { "-t", "-i", ip, "player" + i.ToString() }), ": ")[1], out temporaryString);

                                RunOnUiThread(() => utilities.UpdateLocationInformation(i, player[i].currentPosition));
                            }
                            catch (Exception ex)
                            {
#if DEBUG
                                //This prints to the screen an error message
                                Console.WriteLine("ERROR: " + ex.ToString());
#endif

                                onlineBool = false;
                            }

                            Thread.Sleep(1000);
                        }
                    }
                    else
                    {
                        client.Input(new string[] { "-t", "player" + i.ToString(), Serialise.SerialiseLatLng(player[i].currentPosition) });
                    }
                });

                circle.Center = Serialise.DeserialiseLatLng(Regex.Split(client.Input(new string[] { "-t", "-i", ip, "circle", }), ": ")[1], out temporaryString);

                Thread.Sleep(1000);
            }
        }

        private void GetVertices()
        {
            if (gameType == "Single Player" || gameType == "Host")
            {
                playArea.verticesNode = playArea.vertices.AddFirst(new LatLng(Intent.GetDoubleExtra("markerOneLatitude", 0), Intent.GetDoubleExtra("markerOneLongitude", 0)));
                playArea.verticesNode = playArea.vertices.AddAfter(playArea.verticesNode, new LatLng(Intent.GetDoubleExtra("markerTwoLatitude", 0), Intent.GetDoubleExtra("markerTwoLongitude", 0)));

                playArea.vertices.AddBefore(playArea.verticesNode, new LatLng(playArea.vertices.First.Value.Latitude, playArea.vertices.Last.Value.Longitude));
                playArea.vertices.AddLast(new LatLng(playArea.vertices.Last.Value.Latitude, playArea.vertices.First.Value.Longitude));
            }
            else
            {
                playArea.verticesNode = playArea.vertices.AddFirst(new LatLng(1, 1));
                playArea.verticesNode = playArea.vertices.AddAfter(playArea.verticesNode, new LatLng(-1, -1));

                playArea.vertices.AddBefore(playArea.verticesNode, new LatLng(playArea.vertices.First.Value.Latitude, playArea.vertices.Last.Value.Longitude));
                playArea.vertices.AddLast(new LatLng(playArea.vertices.Last.Value.Latitude, playArea.vertices.First.Value.Longitude));
            }
        }

        private void StartMap()
        {
            Log.Debug("StartMap", "Starting map");

            FragmentManager.FindFragmentById<MapFragment>(Resource.Id.GameMap).GetMapAsync(mapClass);
        }

        private void StartPosition()
        {
            Log.Debug("StartPosition", "Starting position client");

            // pass in the Context, ConnectionListener and ConnectionFailedListener
            apiClient = new GoogleApiClient.Builder(this, positionClass, positionClass).AddApi(LocationServices.API).Build();
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

            apiClient.Connect();
        }

        private void StartUpdateRotation()
        {
            Log.Debug("StartUpdateRotation", "Starting updates from rotation client");

            sensorManager.RegisterListener(rotationClass, sensorManager.GetDefaultSensor(SensorType.MagneticField), SensorDelay.Game);
            sensorManager.RegisterListener(rotationClass, sensorManager.GetDefaultSensor(SensorType.Accelerometer), SensorDelay.Game);
        }

        private void StopUpdatePosition()
        {
            Log.Debug("StopUpdatePosition", "Stopping updates from position client");

            // stop location updates, passing in the LocationListener
            LocationServices.FusedLocationApi.RemoveLocationUpdates(apiClient, positionClass);

            apiClient.Disconnect();
        }

        private void StopUpdateRotation()
        {
            Log.Debug("StopUpdateRotation", "Stopping updates from rotation client");

            sensorManager.UnregisterListener(rotationClass);
        }
    }
}