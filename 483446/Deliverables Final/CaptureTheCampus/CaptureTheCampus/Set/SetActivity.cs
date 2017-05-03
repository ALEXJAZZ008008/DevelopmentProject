using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Util;
using Android.Widget;

namespace CaptureTheCampus.Set
{
    [Activity(Label = "@string/SetActivityLabel", Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait)]
    public class SetActivity : Activity
    {
        private string gameType, numberOfPlayers, playerPosition;

        private SetMap mapClass;
        public GoogleMap map;
        private Marker[] markers;
        public SetPlayArea setPlayArea;
        private int markerNumber;
        public bool buttonClickBool;

        private SetPosition positionClass;
        public GoogleApiClient apiClient;
        public bool positionBool;

        private Button setFirstMarkerButton, setSecondMarkerButton, completePlayAreaButton;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Log.Debug("OnCreate", "OnCreate called, initializing views...");

            // Set our view from the "Game" layout resource
            SetContentView(Resource.Layout.Set);

            Initialise();

            StartMap();
            StartPosition();
        }

        protected override void OnResume()
        {
            base.OnResume();
            Log.Debug("OnResume", "OnResume called, starting event handlers...");

            StartUpdatePosition();

            StartButtonEventHandlers();

            if (buttonClickBool == true)
            {
                StartMapClickEventHandler();
            }
        }

        protected override void OnPause()
        {
            Log.Debug("OnPause", "OnPause called, stopping event handlers");

            StopUpdatePosition();

            StopButtonEventHandlers();
            StopMapClickEventHandler();

            base.OnPause();
        }

        private void Initialise()
        {
            gameType = Intent.GetStringExtra("gameType");

            if(gameType == "Host")
            {
                numberOfPlayers = Intent.GetStringExtra("numberOfPlayers");
                playerPosition = Intent.GetStringExtra("playerPosition");
            }

            mapClass = new SetMap(this);
            setPlayArea = new SetPlayArea();
            setPlayArea.vertices = new LatLng[4];

            setPlayArea.vertices[0] = new LatLng(0, 0);
            setPlayArea.vertices[1] = new LatLng(0, 0);
            setPlayArea.vertices[2] = new LatLng(0, 0);
            setPlayArea.vertices[3] = new LatLng(0, 0);

            markers = new Marker[2];
            markerNumber = -1;
            buttonClickBool = false;

            positionClass = new SetPosition(this);
            positionBool = false;

            setFirstMarkerButton = FindViewById<Button>(Resource.Id.SetFirstMarkerButton);
            setSecondMarkerButton = FindViewById<Button>(Resource.Id.SetSecondMarkerButton);
            completePlayAreaButton = FindViewById<Button>(Resource.Id.CompletePlayAreaButton);
        }

        private void StartMap()
        {
            Log.Debug("StartMap", "Starting map");

            FragmentManager.FindFragmentById<MapFragment>(Resource.Id.SetMap).GetMapAsync(mapClass);
        }

        private void StartPosition()
        {
            Log.Debug("StartPosition", "Starting position client");

            // pass in the Context, ConnectionListener and ConnectionFailedListener
            apiClient = new GoogleApiClient.Builder(this, positionClass, positionClass).AddApi(LocationServices.API).Build();
        }

        private void StartUpdatePosition()
        {
            Log.Debug("StartUpdatePosition", "Starting updates from position client");

            apiClient.Connect();
        }

        private void StartButtonEventHandlers()
        {
            setFirstMarkerButton.Click += (sender, e) => FirstButtonClickEvent();
            setSecondMarkerButton.Click += (sender, e) => SecondButtonClickEvent();
            completePlayAreaButton.Click += (sender, e) => GoToGameActivity();
        }

        public void StopUpdatePosition()
        {
            if (apiClient.IsConnected == true)
            {
                Log.Debug("StopUpdatePosition", "Stopping updates from position client");

                // stop location updates, passing in the LocationListener
                LocationServices.FusedLocationApi.RemoveLocationUpdates(apiClient, positionClass);

                apiClient.Disconnect();
            }
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
                Log.Error("OnCreate", "Map not ready");
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
                    setPlayArea.vertices[0].Latitude = e.Point.Latitude;
                    setPlayArea.vertices[0].Longitude = e.Point.Longitude;
                }
                else
                {
                    setPlayArea.vertices[2].Latitude = e.Point.Latitude;
                    setPlayArea.vertices[2].Longitude = e.Point.Longitude;
                }

                if (markers[0] != null && markers[1] != null)
                {
                    setPlayArea.vertices[1].Latitude = setPlayArea.vertices[0].Latitude;
                    setPlayArea.vertices[1].Longitude = setPlayArea.vertices[2].Longitude;
                    setPlayArea.vertices[3].Latitude = setPlayArea.vertices[2].Latitude;
                    setPlayArea.vertices[3].Longitude = setPlayArea.vertices[0].Longitude;

                    mapClass.SetPolygon(setPlayArea.vertices);
                }
            }
        }

        private void GoToGameActivity()
        {
            if (markers[0] != null && markers[1] != null)
            {
                Intent intent = new Intent(this, typeof(Game.GameActivity));
                intent.PutExtra("gameType", gameType);

                if(gameType == "Host")
                {
                    intent.PutExtra("numberOfPlayers", numberOfPlayers);
                    intent.PutExtra("playerPosition", playerPosition);
                }

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