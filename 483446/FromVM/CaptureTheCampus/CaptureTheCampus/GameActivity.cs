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
            public LinkedList<LatLng> vertices;
            public LinkedListNode<LatLng> verticesNode;
            public LinkedList<Polygon> polygons;
            public LinkedListNode<Polygon> polygonsNode;
            public bool playAreaDrawnBool;
        }

        public struct Path
        {
            public LatLng currentPosition;
            public LinkedList<LatLng> vertices;
            public LinkedListNode<LatLng> verticesNode;
            public Polyline polyline;
            public bool drawingBool;
        }

        private Task finishTask, scoreTask, hazardTask;
        private string gameType;
        public TextView areaTextView, scoreTextView;
        public double initialArea;
        public int area, score;
        public bool finishBool;

        private GameMap mapClass;
        public GoogleMap map;
        public List<Marker> markers;
        public int markerNumber;
        public PlayArea playArea;
        public Circle circle;

        private GamePosition positionClass;
        public GoogleApiClient apiClient;
        public Path path;

        private Rotation rotationClass;
        public SensorManager sensorManager;
        public float[] gravity, geoMagnetic;
        public float azimuth;

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
            score = 0;
            scoreTextView.Text = "Score: " + score.ToString();
            finishBool = false;

            mapClass = new GameMap(this);
            markers = new List<Marker>();

            if(gameType == "Single Player")
            {
                markerNumber = 0;
            }

            playArea = new PlayArea();
            playArea.vertices = new LinkedList<LatLng>();

            GetVertices();

            playArea.polygons = new LinkedList<Polygon>();
            playArea.playAreaDrawnBool = false;

            positionClass = new GamePosition(this);
            path = new Path();
            path.currentPosition = new LatLng(0, 0);
            path.vertices = new LinkedList<LatLng>();
            path.drawingBool = false;

            rotationClass = new Rotation(this);
            gravity = new float[3];
            geoMagnetic = new float[3];
            azimuth = 0;
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
            intent.PutExtra("score", score);
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

            Hazards hazards = new Hazards(this, circle);

            while (true)
            {
                hazards.RunHazards();

                if(finishBool == true)
                {
                    break;
                }

                Thread.Sleep(1000);
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