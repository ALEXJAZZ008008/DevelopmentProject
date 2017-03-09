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

namespace CaptureTheCampus
{
    [Activity(Label = "@string/GameActivityLabel", Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait)]
    public class GameActivity : Activity
    {
        public struct PlayArea
        {
            public LinkedList<LatLng> vertices;
            public LinkedListNode<LatLng> verticesNode;
            public List<Polygon> polygons;
        }

        public struct Path
        {
            public LatLng currentPosition;
            public LinkedList<LatLng> vertices;
            public LinkedListNode<LatLng> verticesNode;
            public Polyline polyline;
            public bool drawing;
        }

        private string gameType;
        public TextView areaTextView, scoreTextView;
        public double initialArea;
        public int area, score;

        private GameMap mapClass;
        public GoogleMap map;
        public List<Marker> markers;
        public PlayArea playArea;

        private Position positionClass;
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
            areaTextView = (TextView)FindViewById(Resource.Id.Area);
            scoreTextView = (TextView)FindViewById(Resource.Id.Score);
            area = 100;
            areaTextView.Text = "Area: " + area.ToString();
            score = 0;
            scoreTextView.Text = "Score: " + score.ToString();

            mapClass = new GameMap(this);
            markers = new List<Marker>();
            playArea = new PlayArea();
            playArea.vertices = new LinkedList<LatLng>();

            GetVertices();

            playArea.polygons = new List<Polygon>();

            positionClass = new Position(this);
            path = new Path();
            path.currentPosition = new LatLng(0, 0);
            path.vertices = new LinkedList<LatLng>();
            path.drawing = false;

            rotationClass = new Rotation(this);
            gravity = new float[3];
            geoMagnetic = new float[3];
            azimuth = 0;
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

        public override void Finish()
        {
            base.Finish();
        }
    }
}