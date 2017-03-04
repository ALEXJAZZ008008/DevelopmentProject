using Android.App;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.OS;
using Android.Util;
using Android.Widget;
using Android.Content;
using Android.Gms.Maps.Model;

namespace CaptureTheCampus
{
    [Activity(Label = "@string/SetActivityLabel", Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait)]
    public class SetActivity : Activity
    {
        private SetMap mapClass;
        public GoogleMap map;
        private Marker[] markers;
        private int markerNumber;
        public bool buttonClickBool;

        public struct PlayArea
        {
            public LatLng[] vertices;
            public PolygonOptions polygonOptions;
            public Polygon polygon;
        }
        public PlayArea playArea;

        private Button setFirstMarkerButton, setSecondMarkerButton, completePlayAreaButton;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Log.Debug("OnCreate", "OnCreate called, initializing views...");

            // Set our view from the "Game" layout resource
            SetContentView(Resource.Layout.Set);

            Initialise();

            StartMap();
        }

        protected override void OnResume()
        {
            base.OnResume();
            Log.Debug("OnResume", "OnResume called, starting event handlers...");

            StartButtonEventHandlers();

            if(buttonClickBool == true)
            {
                StartMapClickEventHandler();
            }
        }

        protected override void OnPause()
        {
            Log.Debug("OnPause", "OnPause called, stopping event handlers");

            StopButtonEventHandlers();
            StopMapClickEventHandler();

            base.OnPause();
        }

        private void Initialise()
        {
            mapClass = new SetMap(this);
            markers = new Marker[2];
            markerNumber = -1;
            buttonClickBool = false;
            playArea = new PlayArea();
            playArea.vertices = new LatLng[4];

            playArea.vertices[0] = new LatLng(0, 0);
            playArea.vertices[1] = new LatLng(0, 0);
            playArea.vertices[2] = new LatLng(0, 0);
            playArea.vertices[3] = new LatLng(0, 0);

            playArea.polygonOptions = new PolygonOptions();

            setFirstMarkerButton = FindViewById<Button>(Resource.Id.SetFirstMarkerButton);
            setSecondMarkerButton = FindViewById<Button>(Resource.Id.SetSecondMarkerButton);
            completePlayAreaButton = FindViewById<Button>(Resource.Id.CompletePlayAreaButton);
        }

        private void StartMap()
        {
            Log.Debug("StartMap", "Starting map");

            FragmentManager.FindFragmentById<MapFragment>(Resource.Id.SetMap).GetMapAsync(mapClass);
        }

        private void StartButtonEventHandlers()
        {
            setFirstMarkerButton.Click += (sender, e) => FirstButtonClickEvent();
            setSecondMarkerButton.Click += (sender, e) => SecondButtonClickEvent();
            completePlayAreaButton.Click += (sender, e) => GoToGameActivity();
        }

        private void StopButtonEventHandlers()
        {
            setFirstMarkerButton.Click -= (sender, e) => FirstButtonClickEvent();
            setSecondMarkerButton.Click -= (sender, e) => SecondButtonClickEvent();
            completePlayAreaButton.Click -= (sender, e) => GoToGameActivity();
        }

        private void FirstButtonClickEvent()
        {
            markerNumber = 0;

            CheckForStartMapClickEventHandler();
        }

        private void SecondButtonClickEvent()
        {
            markerNumber = 1;

            CheckForStartMapClickEventHandler();
        }

        private void CheckForStartMapClickEventHandler()
        {
            if (map != null)
            {
                if (buttonClickBool == false)
                {
                    buttonClickBool = true;

                    StartMapClickEventHandler();
                }
            }
            else
            {
                Toast.MakeText(this, "Map not ready", ToastLength.Long).Show();
            }
        }

        public void StartMapClickEventHandler()
        {
            map.MapClick += (sender, e) => MapClickEvent(e);
        }

        private void StopMapClickEventHandler()
        {
            map.MapClick -= (sender, e) => MapClickEvent(e);
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

                if (markerNumber == 0)
                {
                    playArea.vertices[0].Latitude = e.Point.Latitude;
                    playArea.vertices[0].Longitude = e.Point.Longitude;
                }
                else
                {
                    playArea.vertices[2].Latitude = e.Point.Latitude;
                    playArea.vertices[2].Longitude = e.Point.Longitude;
                }

                if (markers[0] != null && markers[1] != null)
                {
                    playArea.vertices[1].Latitude = playArea.vertices[0].Latitude;
                    playArea.vertices[1].Longitude = playArea.vertices[2].Longitude;
                    playArea.vertices[3].Latitude = playArea.vertices[2].Latitude;
                    playArea.vertices[3].Longitude = playArea.vertices[0].Longitude;

                    mapClass.SetPolygon(playArea.polygonOptions, playArea.vertices);
                }
            }
        }

        private void GoToGameActivity()
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