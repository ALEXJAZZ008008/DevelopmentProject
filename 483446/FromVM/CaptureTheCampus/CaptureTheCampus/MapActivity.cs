using Android.App;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CaptureTheCampus
{
    [Activity(Label = "@string/MapActivityLabel", Icon = "@drawable/icon")]
    public class MapActivity : Activity, IOnMapReadyCallback, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener, Android.Gms.Location.ILocationListener
    {
        private GoogleApiClient apiClient;
        private LocationRequest locRequest;

        private GoogleMap map;

        private List<Marker> markers = new List<Marker>();

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Log.Debug("OnCreate", "OnCreate called, initializing views...");

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Map);

            StartMap();

            // pass in the Context, ConnectionListener and ConnectionFailedListener
            apiClient = new GoogleApiClient.Builder(this, this, this).AddApi(LocationServices.API).Build();

            // generate a location request that we will pass into a call for location updates
            locRequest = new LocationRequest();

            Task.Run(() => CheckKeyentryInput());
        }

        protected override void OnResume()
        {
            base.OnResume();
            Log.Debug("OnResume", "OnResume called, connecting to client...");

            apiClient.Connect();

            if (apiClient.IsConnected)
            {
                Location location = LocationServices.FusedLocationApi.GetLastLocation(apiClient);

                if (location != null)
                {
                    UpdateCurrentLocation(location);

                    Log.Debug("LocationClient", "Last location printed");
                }
            }
            else
            {
                Log.Info("LocationClient", "Please wait for client to connect");
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            Log.Debug("OnPause", "OnPause called, stopping location updates");

            if (apiClient.IsConnected)
            {
                // stop location updates, passing in the LocationListener
                LocationServices.FusedLocationApi.RemoveLocationUpdates(apiClient, this);

                apiClient.Disconnect();
            }
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            Log.Debug("OnMapReady", "OnMapReady called, building initial map");

            bool[] mapSettingsBools = { false, false, false, false, false, false, false, false, false };

            BuildMap(googleMap, mapSettingsBools);

            LatLng defaultPosition = new LatLng(53.7715, -0.3674);

            SetCamera(defaultPosition);
            SetUserMarker(defaultPosition);
        }

        private void StartMap()
        {
            if (map == null)
            {
                Log.Debug("StartMap", "Inflating fragment");

                FragmentManager.FindFragmentById<MapFragment>(Resource.Id.map).GetMapAsync(this);
            }
        }

        private void BuildMap(GoogleMap googleMap, bool[] mapSettingsBools)
        {
            Log.Debug("BuildMap", "Building map");

            map = googleMap;
            map.MapType = GoogleMap.MapTypeTerrain;

            MapSettings(mapSettingsBools);
        }

        private void MapSettings(bool[] mapSettingsBools)
        {
            Log.Debug("MapSettings", "Setting map UI restrictions");

            map.UiSettings.CompassEnabled = mapSettingsBools[0];
            map.UiSettings.IndoorLevelPickerEnabled = mapSettingsBools[1];
            map.UiSettings.MapToolbarEnabled = mapSettingsBools[2];
            map.UiSettings.MyLocationButtonEnabled = mapSettingsBools[3];
            map.UiSettings.RotateGesturesEnabled = mapSettingsBools[4];
            map.UiSettings.ScrollGesturesEnabled = mapSettingsBools[5];
            map.UiSettings.TiltGesturesEnabled = mapSettingsBools[6];
            map.UiSettings.ZoomControlsEnabled = mapSettingsBools[7];
            map.UiSettings.ZoomGesturesEnabled = mapSettingsBools[8];
        }

        private void SetCamera(LatLng position)
        {
            Log.Debug("SetCamera", "Setting camera position");

            int[] builderSettingsInts = { 18, 0, 21 };

            CameraPosition cameraPosition = CameraBuilder(position, builderSettingsInts).Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);

            map.MoveCamera(cameraUpdate);
        }

        private CameraPosition.Builder CameraBuilder(LatLng position, int[] builderSettingsInts)
        {
            Log.Debug("CameraBuilder", "Building Camera");

            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();

            builder.Target(position);
            builder.Zoom(builderSettingsInts[0]);
            builder.Bearing(builderSettingsInts[1]);
            builder.Tilt(builderSettingsInts[2]);

            return builder;
        }

        private void SetUserMarker(LatLng defaultPosition)
        {
            Log.Debug("SetUserMarker", "Setting user position marker");

            float defaultColour = BitmapDescriptorFactory.HueRed;

            markers.Add(map.AddMarker(BuildMarker(defaultPosition, defaultColour)));
        }

        private MarkerOptions BuildMarker(LatLng position, float colour)
        {
            Log.Debug("BuildMarker", "Building marker");

            MarkerOptions marker = new MarkerOptions();

            marker.SetPosition(position);
            marker.SetTitle(Resources.GetString(Resource.String.Username));
            marker.SetIcon(BitmapDescriptorFactory.DefaultMarker(colour));

            return marker;
        }

        public void OnConnected(Bundle connectionHint)
        {
            // This method is called when we connect to the LocationClient. We can start location updated directly form
            // here if desired, or we can do it in a lifecycle method, as shown above 

            // You must implement this to implement the IGooglePlayServicesClientConnectionCallbacks Interface
            Log.Info("LocationClient", "Now connected to client");
        }

        public void OnDisconnected()
        {
            // This method is called when we disconnect from the LocationClient.

            // You must implement this to implement the IGooglePlayServicesClientConnectionCallbacks Interface
            Log.Info("LocationClient", "Now disconnected from client");
        }

        public void OnConnectionSuspended(int cause)
        {
            Log.Info("LocationClient", "Connection suspended");
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            // This method is used to handle connection issues with the Google Play Services Client (LocationClient). 
            // You can check if the connection has a resolution (bundle.HasResolution) and attempt to resolve it

            // You must implement this to implement the IGooglePlayServicesClientOnConnectionFailedListener Interface
            Log.Info("LocationClient", "Connection failed, attempting to reach google play services");
        }

        public void OnLocationChanged(Location location)
        {
            // This method returns changes in the user's location if they've been requested
            
            // You must implement this to implement the Android.Gms.Locations.ILocationListener Interface
            Log.Debug("LocationClient", "going to UpdateLocation");

            UpdateCurrentLocation(location);
        }

        private void UpdateCurrentLocation(Location location)
        {
            LatLng currentPosition = new LatLng(location.Latitude, location.Longitude);

            Log.Debug("Position", currentPosition.ToString());

            UpdateLocation(currentPosition);
        }

        private void UpdateLocation(LatLng position)
        {
            Log.Debug("LocationClient", "Location updated");

            SetCamera(position);
            markers[0].Position = position;
        }

        private void CheckKeyentryInput()
        {
            
        }
    }
}