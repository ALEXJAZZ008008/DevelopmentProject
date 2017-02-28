using Android.Content;
using Android.Views;
using Android.Util;
using Android.Locations;
using Android.Gms.Maps.Model;
using Android.Gms.Maps;

namespace CaptureTheCampus
{
    public class Utilities : View
    {
        private GameActivity gameActivity;

        public Utilities(Context context) : base (context)
        {
            Log.Info("Position", "Position built");

            gameActivity = (GameActivity)context;
        }

        public void BuildMap(GoogleMap googleMap, bool[] mapSettingsBools)
        {
            Log.Debug("BuildMap", "Building map");

            googleMap.MapType = GoogleMap.MapTypeTerrain;
            MapSettings(googleMap, mapSettingsBools);

            gameActivity.map = googleMap;
        }

        private void MapSettings(GoogleMap googleMap, bool[] mapSettingsBools)
        {
            Log.Debug("MapSettings", "Setting map UI restrictions");

            googleMap.UiSettings.CompassEnabled = mapSettingsBools[0];
            googleMap.UiSettings.IndoorLevelPickerEnabled = mapSettingsBools[1];
            googleMap.UiSettings.MapToolbarEnabled = mapSettingsBools[2];
            googleMap.UiSettings.MyLocationButtonEnabled = mapSettingsBools[3];
            googleMap.UiSettings.RotateGesturesEnabled = mapSettingsBools[4];
            googleMap.UiSettings.ScrollGesturesEnabled = mapSettingsBools[5];
            googleMap.UiSettings.TiltGesturesEnabled = mapSettingsBools[6];
            googleMap.UiSettings.ZoomControlsEnabled = mapSettingsBools[7];
            googleMap.UiSettings.ZoomGesturesEnabled = mapSettingsBools[8];
        }

        public void SetMarker()
        {
            Log.Debug("SetMarker", "Setting marker position");

            gameActivity.markers.Add(gameActivity.map.AddMarker(BuildMarker()));
        }

        private MarkerOptions BuildMarker()
        {
            Log.Debug("BuildMarker", "Building marker");

            MarkerOptions markerOptions = new MarkerOptions();

            markerOptions.SetPosition(gameActivity.position);

            return markerOptions;
        }

        public void UpdateCurrentLocation(Location location)
        {
            gameActivity.position = new LatLng(location.Latitude, location.Longitude);

            Log.Debug("Position", gameActivity.position.ToString());

            UpdateLocation();
            SetPolyline(gameActivity.path);
        }

        private void UpdateLocation()
        {
            Log.Debug("LocationClient", "Location updated");

            SetCamera();
            MoveMarker();
        }

        private void SetCamera()
        {
            Log.Debug("SetCamera", "Setting camera position");

            int[] builderSettingsInts = { 0, 0, 21 };

            CameraPosition cameraPosition = CameraBuilder(builderSettingsInts).Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);

            gameActivity.map.MoveCamera(cameraUpdate);
        }

        private CameraPosition.Builder CameraBuilder(int[] builderSettingsInts)
        {
            Log.Debug("CameraBuilder", "Building Camera");

            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();

            builder.Target(gameActivity.position);
            builder.Zoom(builderSettingsInts[0]);
            builder.Bearing(builderSettingsInts[1]);
            builder.Tilt(builderSettingsInts[2]);

            return builder;
        }

        private void MoveMarker()
        {
            gameActivity.markers[0].Position = gameActivity.position;
        }

        public void SetPolyline(PolylineOptions polyline)
        {
            Log.Debug("SetPolyline", "Setting polyline positions");

            polyline.Add(gameActivity.position);

            BuildPolyline(polyline);
        }

        private void BuildPolyline(PolylineOptions polyline)
        {
            Log.Debug("BuildPolyline", "Building polyline");

            gameActivity.map.AddPolyline(polyline);
        }

        public void SetPolygon(PolygonOptions polygon, LatLng[] position)
        {
            Log.Debug("SetPolygon", "Setting polygon positions");

            for (int i = 0; i < position.Length; i++)
            {
                polygon.Add(position[i]);
            }

            BuildPolygon(polygon);
        }

        private void BuildPolygon(PolygonOptions polygon)
        {
            Log.Debug("BuildPolygon", "Building polygon");

            gameActivity.map.AddPolygon(polygon);
        }
    }
}