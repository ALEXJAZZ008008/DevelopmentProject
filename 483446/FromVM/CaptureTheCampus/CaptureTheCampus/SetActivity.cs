using Android.App;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.OS;
using Android.Util;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Android.Widget;
using Android.Content;
using Android.Gms.Maps.Model;

namespace CaptureTheCampus
{
    [Activity(Label = "@string/SetActivityLabel", Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait)]
    public class SetActivity : Activity
    {
        private Dictionary<string, Task> taskDictionary;
        private Task task;

        private Map mapClass;
        public GoogleMap map;
        public bool mapClickBool;
        private Marker[] markers;
        private int markerNumber;

        private Button SetFirstMarkerButton, SetSecondMarkerButton, CompletePlayAreaButton;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Log.Debug("OnCreate", "OnCreate called, initializing views...");

            // Set our view from the "Game" layout resource
            SetContentView(Resource.Layout.Set);

            Initialise();

            StartTask(StartMap());
        }

        protected override void OnResume()
        {
            base.OnResume();
            Log.Debug("OnResume", "OnResume called, starting event handlers...");

            StartTask(StartButtonEventHandlers());

            if(mapClickBool == true)
            {
                StartTask(StartMapClickEventHandler());
            }
        }

        protected override void OnPause()
        {
            Log.Debug("OnPause", "OnPause called, stopping event handlers");

            StartTask(StopButtonEventHandlers());
            StartTask(StopMapClickEventHandler());

            base.OnPause();
        }

        private void Initialise()
        {
            taskDictionary = new Dictionary<string, Task>();

            mapClickBool = false;
            markers = new Marker[2];
            markerNumber = -1;

            SetFirstMarkerButton = FindViewById<Button>(Resource.Id.SetFirstMarkerButton);
            SetSecondMarkerButton = FindViewById<Button>(Resource.Id.SetSecondMarkerButton);
            CompletePlayAreaButton = FindViewById<Button>(Resource.Id.CompletePlayAreaButton);
        }

        public void StartTask(Action method)
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

            mapClass = new Map(this);

            FragmentManager.FindFragmentById<MapFragment>(Resource.Id.Map).GetMapAsync(mapClass);

            return null;
        }

        private Action StartButtonEventHandlers()
        {
            SetFirstMarkerButton.Click += (sender, e) =>
            {
                markerNumber = 0;
            };

            SetSecondMarkerButton.Click += (sender, e) =>
            {
                markerNumber = 1;
            };

            CompletePlayAreaButton.Click += (sender, e) => ReturnIntent();

            return null;
        }

        private Action StopButtonEventHandlers()
        {
            SetFirstMarkerButton.Click -= (sender, e) =>
            {
                markerNumber = 0;
            };

            SetSecondMarkerButton.Click -= (sender, e) =>
            {
                markerNumber = 1;
            };

            CompletePlayAreaButton.Click -= (sender, e) => ReturnIntent();

            return null;
        }

        public Action StartMapClickEventHandler()
        {
            map.MapClick += (sender, e) => MapClickEvent(e);

            return null;
        }

        private Action StopMapClickEventHandler()
        {
            map.MapClick -= (sender, e) => MapClickEvent(e);

            return null;
        }

        private void MapClickEvent(GoogleMap.MapClickEventArgs e)
        {
            if (markerNumber != -1)
            {
                if (markers[markerNumber] == null)
                {
                    markers[markerNumber] = map.AddMarker(mapClass.BuildMarker(new LatLng(e.Point.Latitude, e.Point.Longitude)));
                }
                else
                {
                    markers[markerNumber].Position = new LatLng(e.Point.Latitude, e.Point.Longitude);
                }
            }
        }

        private void ReturnIntent()
        {
            if (markers[0] != null && markers[1] != null)
            {
                Intent intent = new Intent(this, typeof(GameActivity));
                intent.PutExtra("gameType", Intent.GetStringExtra("gameType"));
                intent.PutExtra("markerOneLatitude", markers[0].Position.Latitude);
                intent.PutExtra("markerOneLongitude", markers[0].Position.Longitude);
                intent.PutExtra("markerTwoLatitude", markers[1].Position.Latitude);
                intent.PutExtra("markerTwoLongitude", markers[1].Position.Longitude);
                StartActivity(intent);

                Finish();
            }
            else
            {
                Toast.MakeText(this, "Please place markers", ToastLength.Long).Show();
            }
        }
    }
}