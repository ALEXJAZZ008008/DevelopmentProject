using Android.App;
using Android.Content.PM;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Hardware;
using Android.OS;
using Android.Util;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Android.Content;

namespace CaptureTheCampus
{
    [Activity(Label = "@string/GameActivityLabel", Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait)]
    public class GameActivity : Activity
    {
        public struct PlayArea
        {
            public List<LatLng> vertices;
            public Polygon polygon;
        }

        public struct Path
        {
            public List<LatLng> vertices;
            public List<Polyline> polylines;
            public bool drawing;
        }

        private string gameType;

        private Dictionary<string, Task> taskDictionary;
        private Task task;

        private GameMap mapClass;
        public GoogleMap map;
        public List<Marker> markers;
        public PlayArea playArea;

        private Position positionClass;
        public GoogleApiClient apiClient;
        public LatLng position;
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
            StartRotation();
        }

        protected override void OnResume()
        {
            base.OnResume();
            Log.Debug("OnResume", "OnResume called, connecting to clients...");

            StartUpdatePosition();
            StartUpdateRotation();
        }

        protected override void OnPause()
        {
            Log.Debug("OnPause", "OnPause called, stopping client updates");

            StopUpdatePosition();
            StopUpdateRotation();

            base.OnPause();
        }

        private void Initialise()
        {
            gameType = Intent.GetStringExtra("gameType");

            taskDictionary = new Dictionary<string, Task>();

            mapClass = new GameMap(this);
            markers = new List<Marker>();
            playArea = new PlayArea();

            GetVertices();

            positionClass = new Position(this);
            position = new LatLng(0, 0);
            path = new Path();
            path.vertices = new List<LatLng>();
            path.polylines = new List<Polyline>();
            path.drawing = false;

            rotationClass = new Rotation(this);
            gravity = new float[3];
            geoMagnetic = new float[3];
            azimuth = 0;
        }

        private void GetVertices()
        {
            playArea.vertices = new List<LatLng>();

            playArea.vertices.Add(new LatLng(Intent.GetDoubleExtra("markerOneLatitude", 0), Intent.GetDoubleExtra("markerOneLongitude", 0)));
            playArea.vertices.Add(new LatLng(Intent.GetDoubleExtra("markerTwoLatitude", 0), Intent.GetDoubleExtra("markerTwoLongitude", 0)));

            playArea.vertices.Insert(1, new LatLng(playArea.vertices[0].Latitude, playArea.vertices[1].Longitude));
            playArea.vertices.Add(new LatLng(playArea.vertices[2].Latitude, playArea.vertices[0].Longitude));
        }

        private void StartTask(Action method)
        {
            Log.Debug("StartTask", "Starting task");

            string taskName = "Task" + taskDictionary.Count;

            task = new Task(() => method());

            taskDictionary.Add(taskName, task);

            task.Start();
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
    }
}