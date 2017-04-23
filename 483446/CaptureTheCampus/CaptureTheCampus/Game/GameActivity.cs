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
        public double latLngToKM;

        private Utilities utilities;
        private CancellationTokenSource cancelationTokenSource;
        private Task finishTask, scoreTask, hazardTask, serverTask, heartbeatTask, clientTask, cameraTask;
        public TextView areaTextView, scoreTextView;
        public double initialArea;
        volatile public int area;
        volatile public bool finishBool;

        private GameMap gameMap;
        public GoogleMap googleMap;
        public GamePlayArea gamePlayArea;
        public bool cameraInitiallySet;

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

            latLngToKM = 105.65;

            utilities = new Utilities(this);

            cancelationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancelationTokenSource.Token;

            finishTask = new Task(() => FinishCheck(cancellationToken), cancelationTokenSource.Token, TaskCreationOptions.LongRunning);
            scoreTask = new Task(() => EndConditions(cancellationToken), cancelationTokenSource.Token, TaskCreationOptions.LongRunning);
            hazardTask = new Task(() => RunHazards(cancellationToken), cancelationTokenSource.Token, TaskCreationOptions.LongRunning);
            cameraTask = new Task(() => MoveCamera(cancellationToken), cancelationTokenSource.Token, TaskCreationOptions.LongRunning);

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

            cameraInitiallySet = false;

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

                    serverTask = new Task(() => watchdog.Input(cancellationToken), cancelationTokenSource.Token, TaskCreationOptions.LongRunning);

                    heartbeatTask = new Task(() => Heartbeat(cancellationToken), cancelationTokenSource.Token, TaskCreationOptions.LongRunning);

                    clientTask = new Task(() => HostClient(cancellationToken), cancelationTokenSource.Token, TaskCreationOptions.LongRunning);
                }
                else
                {
                    if (gameType == "Join")
                    {
                        numberOfPlayers = int.Parse(Intent.GetStringExtra("numberOfPlayers"));

                        playerArray = new Player[numberOfPlayers];

                        playerPosition = int.Parse(Intent.GetStringExtra("playerPosition")) - 1;

                        ip = Intent.GetStringExtra("ip");

                        clientTask = new Task(() => JoinClient(cancellationToken), cancelationTokenSource.Token, TaskCreationOptions.LongRunning);
                    }
                }
            }

            for (int i = 0; i < numberOfPlayers; i++)
            {
                playerArray[i] = new Player();

                playerArray[i].score = 0;

                playerArray[i].currentPosition = new LatLng(0, 0);
                playerArray[i].vertices = new LinkedList<LatLng>();
                playerArray[i].drawingBool = false;
                playerArray[i].positionBool = true;
                playerArray[i].deathBool = false;
            }

            scoreTextView.Text = "Score: " + playerArray[playerPosition].score.ToString();

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

            playerArray[killedPlayerPosition].vertices.Clear();
            playerArray[killedPlayerPosition].polyline.Remove();

            playerArray[killedPlayerPosition].drawingBool = false;
            playerArray[killedPlayerPosition].positionBool = false;
            playerArray[killedPlayerPosition].deathBool = true;
        }

        private void MoveCamera(CancellationToken cancellationToken)
        {
            int count = 0;
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
                    RunOnUiThread(() =>
                    {
                        if (!googleMap.Projection.VisibleRegion.LatLngBounds.Contains(playerArray[playerPosition].currentPosition) || count >= 10)
                        {
                            count = 0;

                            utilities.SetCamera(playerPosition);
                        }
                    });
                }
                catch (Exception ex)
                {
#if DEBUG
                    //This prints to the screen an error message
                    Console.WriteLine("ERROR: " + ex.ToString());
#endif
                }

                count++;

                Thread.Sleep(1000);
            }
        }

        private void Heartbeat(CancellationToken cancellationToken)
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

        private void HostClient(CancellationToken cancellationToken)
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

            RunOnUiThread(() =>
            {
                client.Input(new string[] { "-t", "player" + playerPosition.ToString(), Static.Serialise.SerialiseLatLng(playerArray[playerPosition].currentPosition) });

                client.Input(new string[] { "-t", "playArea", Static.Serialise.SerialiseLatLngLinkedList(gamePlayArea.vertices) });
                client.Input(new string[] { "-t", "circleCentre", Static.Serialise.SerialiseLatLng(circle.Center) });
                client.Input(new string[] { "-t", "circleRadius", Static.Serialise.SerialiseString(circle.Radius.ToString()) });
            });

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
                    Parallel.For(0, numberOfPlayers, (i) =>
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
                                LatLng temporaryPosition;

                                RunOnUiThread(() =>
                                {
                                    temporaryPosition = Static.Serialise.DeserialiseLatLng(Regex.Split(client.Input(new string[] { "-t", "player" + (i).ToString() }), ": ")[1], out temporaryString);

                                    if (temporaryPosition.Latitude != playerArray[i].currentPosition.Latitude || temporaryPosition.Longitude != playerArray[i].currentPosition.Longitude)
                                    {
                                        playerArray[i].currentPosition = temporaryPosition;

                                        utilities.UpdateLocationInformation(i, playerArray[i].currentPosition);
                                    }
                                });
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
                            RunOnUiThread(() => client.Input(new string[] { "-t", "player" + playerPosition.ToString(), Static.Serialise.SerialiseLatLng(playerArray[playerPosition].currentPosition) }));
                        }
                    });

                    RunOnUiThread(() => client.Input(new string[] { "-t", "circleCentre", Static.Serialise.SerialiseLatLng(circle.Center) }));
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

        private void JoinClient(CancellationToken cancellationToken)
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
                    client.Input(new string[] { "-t", "-i", ip, "test", "123" });

                    int.Parse(Regex.Split(client.Input(new string[] { "-t", "-i", ip, "test" }), ": ")[1]);
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

            Thread.Sleep(3000);

            string temporaryString;

            RunOnUiThread(() =>
            {
                client.Input(new string[] { "-t", "-i", ip, "player" + playerPosition.ToString(), Static.Serialise.SerialiseLatLng(playerArray[playerPosition].currentPosition) });

                gamePlayArea.vertices = Static.Serialise.DeserialiseLatLngLinkedList(Regex.Split(client.Input(new string[] { "-t", "-i", ip, "playArea" }), ": ")[1]);

                gamePlayArea.polygons.RemoveFirst();
                gamePlayArea.playAreaDrawnBool = false;

                utilities.SetPolygon(gamePlayArea.vertices);

                playerArray[playerPosition].vertices = new LinkedList<LatLng>();
                playerArray[playerPosition].drawingBool = false;
                playerArray[playerPosition].positionBool = true;
                playerArray[playerPosition].deathBool = false;

                circle.Center = Static.Serialise.DeserialiseLatLng(Regex.Split(client.Input(new string[] { "-t", "-i", ip, "circleCentre", }), ": ")[1], out temporaryString);
                circle.Radius = double.Parse(Static.Serialise.DeserialiseString(Regex.Split(client.Input(new string[] { "-t", "-i", ip, "circleRadius", }), ": ")[1], out temporaryString));
            });

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
                    Parallel.For(0, numberOfPlayers, (i) =>
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
                               LatLng temporaryPosition;

                               RunOnUiThread(() =>
                               {
                                   temporaryPosition = Static.Serialise.DeserialiseLatLng(Regex.Split(client.Input(new string[] { "-t", "-i", ip, "player" + (i).ToString() }), ": ")[1], out temporaryString);

                                   if (temporaryPosition.Latitude != playerArray[i].currentPosition.Latitude || temporaryPosition.Longitude != playerArray[i].currentPosition.Longitude)
                                   {
                                       playerArray[i].currentPosition = temporaryPosition;

                                       utilities.UpdateLocationInformation(i, playerArray[i].currentPosition);
                                   }
                               });
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
                           RunOnUiThread(() => client.Input(new string[] { "-t", "-i", ip, "player" + playerPosition.ToString(), Static.Serialise.SerialiseLatLng(playerArray[playerPosition].currentPosition) }));
                       }
                   });

                    RunOnUiThread(() => circle.Center = Static.Serialise.DeserialiseLatLng(Regex.Split(client.Input(new string[] { "-t", "-i", ip, "circleCentre", }), ": ")[1], out temporaryString));
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
            cameraTask.Start();

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