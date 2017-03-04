using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Util;
using Android.Views;

namespace CaptureTheCampus
{
    public class SetMap : View, IOnMapReadyCallback
    {
        SetActivity setActivity;

        public SetMap(Context context) : base (context)
        {
            Log.Info("Map", "Map built");

            setActivity = (SetActivity)context;
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            Log.Debug("OnMapReady", "OnMapReady called, building initial map");

            bool[] mapSettingsBools =
            {
                false,
                false,
                false,
                false,
                true,
                true,
                true,
                true,
                true
            };

            BuildMap(googleMap, mapSettingsBools);
        }

        private void BuildMap(GoogleMap googleMap, bool[] mapSettingsBools)
        {
            Log.Debug("BuildMap", "Building map");

            googleMap.MapType = GoogleMap.MapTypeTerrain;
            MapSettings(googleMap, mapSettingsBools);

            setActivity.map = googleMap;
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

        public MarkerOptions BuildMarker(LatLng position)
        {
            Log.Debug("BuildMarker", "Building marker");

            MarkerOptions markerOptions = new MarkerOptions();

            markerOptions.SetPosition(position);

            return markerOptions;
        }

        public void SetPolygon(PolygonOptions polygon, LatLng[] position)
        {
            Log.Debug("SetPolygon", "Setting polygon positions");

            for (int i = 0; i < position.Length; i++)
            {
                polygon.Add(position[i]);
            }

            if (setActivity.playArea.polygon == null)
            {
                BuildPolygon(polygon);
            }
            else
            {
                setActivity.playArea.polygon.Points = position;
            }
        }

        private void BuildPolygon(PolygonOptions polygon)
        {
            Log.Debug("BuildPolygon", "Building polygon");

            setActivity.playArea.polygon = setActivity.map.AddPolygon(polygon);
        }
    }
}