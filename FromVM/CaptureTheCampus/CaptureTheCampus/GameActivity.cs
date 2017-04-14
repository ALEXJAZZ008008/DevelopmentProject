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

namespace CaptureTheCampus
{
    [Activity(Label = "@string/GameActivityLabel", Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait)]
    public class GameActivity : Activity
    {
        public struct PlayArea
        {
            volatile public LinkedList<LatLng> vertices;
            public LinkedListNode<LatLng> verticesNode;
            public LinkedList<Polygon> polygons;
            public LinkedListNode<Polygon> polygonsNode;
            public bool playAreaDrawnBool;
        }

        public struct Player
        {
            public Marker marker;
            public int score;
            public LatLng currentPosition;
            public LinkedList<LatLng> vertices;
            public LinkedListNode<LatLng> verticesNode;
            public Polyline polyline;
            public bool drawingBool, positionBool, deathBool;
        }

        private Task finishTask, scoreTask, hazardTask;
        private string gameType;
        public TextView areaTextView, scoreTextView;
        public double initialArea;
        volatile public int area;
        volatile public bool finishBool;

        private GameMap mapClass;
        public GoogleMap map;
        public PlayArea playArea;

        private GamePosition positionClass;
        public GoogleApiClient apiClient;
        public Player[] player;
        public int playerPosition;

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
            finishTask = new Task(() => FinishCheck());
            scoreTask = new Task(() => EndConditions());
            hazardTask = new Task(() => RunHazards());

            gameType = Intent.GetStringExtra("gameType");
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
                player = new Player[1];

                playerPosition = 0;

                player[playerPosition].score = 0;

                scoreTextView.Text = "Score: " + player[playerPosition].score.ToString();

                player[playerPosition].currentPosition = new LatLng(0, 0);
                player[playerPosition].vertices = new LinkedList<LatLng>();
                player[playerPosition].drawingBool = false;
                player[playerPosition].positionBool = true;
                player[playerPosition].deathBool = false;
            }

            rotationClass = new Rotation(this);
            gravity = new float[3];
            geoMagnetic = new float[3];
            azimuth = 0;

            hazards = new Hazards(this);
        }

        private void FinishCheck()
        {
            while(true)
            {
                if(finishBool == true)
                {
                    Finish();

                    break;
                }

                Thread.Sleep(1000);
            }
        }

        private void EndConditions()
        {
            while (true)
            {
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

            Intent intent = new Intent(this, typeof(ScoreActivity));
            intent.PutExtra("score", player[playerPosition].score);
            StartActivity(intent);

            finishBool = true;
        }

        private void RunHazards()
        {
            while (true)
            {
                if(circle != null)
                {
                    break;
                }

                Thread.Sleep(1000);
            }

            hazards.SetCircle(circle);

            LinkedList<LatLng> temporaryPlayAreaVertices = new LinkedList<LatLng>();
            LinkedList<LatLng> temporaryPathVertices = new LinkedList<LatLng>();
            bool temporaryBool = new bool();

            while (true)
            {
                CopyVertices(temporaryPlayAreaVertices, playArea.vertices);
                CopyVertices(temporaryPathVertices, player[playerPosition].vertices);
                CopyBool(out temporaryBool, player[playerPosition].deathBool);

                hazards.RunHazards(temporaryPlayAreaVertices, temporaryPathVertices, temporaryBool);

                if(finishBool == true)
                {
                    break;
                }

                Thread.Sleep(1000);
            }
        }

        private void CopyBool(out bool temporaryBool, bool deathBool)
        {
            temporaryBool = deathBool;
        }

        public void KillPlayer()
        {
            player[playerPosition].score = player[playerPosition].score / 2;

            player[playerPosition].polyline.Remove();

            player[playerPosition].drawingBool = false;
            player[playerPosition].positionBool = false;
            player[playerPosition].deathBool = true;
        }

        private void CopyVertices(LinkedList<LatLng> temporaryVertices, LinkedList<LatLng> inVertices)
        {
            LatLng[] temporaryArray = new LatLng[inVertices.Count];

            inVertices.CopyTo(temporaryArray, 0);

            temporaryVertices.Clear();

            for (int i = 0; i < temporaryArray.Length; i++)
            {
                temporaryVertices.AddLast(temporaryArray[i]);
            }
        }

        private void GetVertices()
        {
            playArea.verticesNode = playArea.vertices.AddFirst(new LatLng(Intent.GetDoubleExtra("markerOneLatitude", 0), Intent.GetDoubleExtra("markerOneLongitude", 0)));
            playArea.verticesNode = playArea.vertices.AddAfter(playArea.verticesNode, new LatLng(Intent.GetDoubleExtra("markerTwoLatitude", 0), Intent.GetDoubleExtra("markerTwoLongitude", 0)));

            playArea.vertices.AddBefore(playArea.verticesNode, new LatLng(playArea.vertices.First.Value.Latitude, playArea.vertices.Last.Value.Longitude));
            playArea.vertices.AddLast(new LatLng(playArea.vertices.Last.Value.Latitude, playArea.vertices.First.Value.Longitude));
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