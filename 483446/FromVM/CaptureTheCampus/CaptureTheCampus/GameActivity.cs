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
        private string gameType;

        private Dictionary<string, Task> taskDictionary;
        private Task task;

        private GameMap mapClass;
        public GoogleMap map;
        public List<Marker> markers;
        public LatLng[] playAreaVertices;
        public PolygonOptions playArea;

        private Position positionClass;
        public GoogleApiClient apiClient;
        public LatLng position;
        public PolylineOptions path;

        private Rotation rotationClass;
        public SensorManager sensorManager;
        public int rotation;

        private Utilities utilities;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Log.Debug("OnCreate", "OnCreate called, initializing views...");

            // Set our view from the "Game" layout resource
            SetContentView(Resource.Layout.Game);

            Initialise();

            StartTask(StartMap());
            StartTask(StartPosition());
            StartTask(StartRotation());
        }

        protected override void OnResume()
        {
            base.OnResume();
            Log.Debug("OnResume", "OnResume called, connecting to clients...");

            StartTask(StartUpdatePosition());
            StartTask(StartUpdateRotation());
        }

        protected override void OnPause()
        {
            Log.Debug("OnPause", "OnPause called, stopping client updates");

            StartTask(StopUpdateRotation());
            StartTask(StopUpdatePosition());

            base.OnPause();
        }

        private void Initialise()
        {
            gameType = Intent.GetStringExtra("gameType");

            taskDictionary = new Dictionary<string, Task>();

            markers = new List<Marker>();
            playAreaVertices = new LatLng[4];

            playAreaVertices[0] = new LatLng(Intent.GetDoubleExtra("markerOneLatitude", 0), Intent.GetDoubleExtra("markerOneLongitude", 0));
            playAreaVertices[2] = new LatLng(Intent.GetDoubleExtra("markerTwoLatitude", 0), Intent.GetDoubleExtra("markerTwoLongitude", 0));

            playAreaVertices[1] = new LatLng(playAreaVertices[0].Latitude, playAreaVertices[2].Longitude);
            playAreaVertices[3] = new LatLng(playAreaVertices[2].Latitude, playAreaVertices[0].Longitude);

            playArea = new PolygonOptions();

            utilities = new Utilities(this);

            position = new LatLng(0, 0);
            path = new PolylineOptions();
            rotation = 0;
        }

        private void StartTask(Action method)
        {
            Log.Debug("StartTask", "Starting task");

            string taskName = "Task" + taskDictionary.Count;

            task = new Task(() => method());

            taskDictionary.Add(taskName, task);

            task.Start();
        }

        private Action StartMap()
        {
            Log.Debug("StartMap", "Starting map");

            mapClass = new GameMap(this);

            FragmentManager.FindFragmentById<MapFragment>(Resource.Id.GameMap).GetMapAsync(mapClass);

            return null;
        }

        private Action StartPosition()
        {
            Log.Debug("StartPosition", "Starting position client");

            positionClass = new Position(this);

            // pass in the Context, ConnectionListener and ConnectionFailedListener
            apiClient = new GoogleApiClient.Builder(this, positionClass, positionClass).AddApi(LocationServices.API).Build();

            return null;
        }

        private Action StartRotation()
        {
            Log.Debug("StartRotation", "Starting rotation client");

            rotationClass = new Rotation(this);
            sensorManager = (SensorManager)GetSystemService(SensorService);

            return null;
        }

        private Action StartUpdatePosition()
        {
            Log.Debug("StartUpdatePosition", "Starting updates from position client");

            if (!apiClient.IsConnected)
            {
                apiClient.Connect();
            }

            return null;
        }

        private Action StartUpdateRotation()
        {
            Log.Debug("StartUpdateRotation", "Starting updates from rotation client");

            sensorManager.RegisterListener(rotationClass, sensorManager.GetDefaultSensor(SensorType.Orientation), SensorDelay.Game);

            return null;
        }

        private Action StopUpdatePosition()
        {
            Log.Debug("StopUpdatePosition", "Stopping updates from position client");

            if (apiClient.IsConnected)
            {
                // stop location updates, passing in the LocationListener
                LocationServices.FusedLocationApi.RemoveLocationUpdates(apiClient, positionClass);

                apiClient.Disconnect();
            }

            return null;
        }

        private Action StopUpdateRotation()
        {
            Log.Debug("StopUpdateRotation", "Stopping updates from rotation client");

            sensorManager.UnregisterListener(rotationClass);

            return null;
        }
    }
}