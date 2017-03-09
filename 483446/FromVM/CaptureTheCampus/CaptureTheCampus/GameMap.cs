using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Util;

namespace CaptureTheCampus
{
    class GameMap : Activity, IOnMapReadyCallback
    {
        private GameActivity gameActivity;
        private Utilities utilities;

        public GameMap(Context context)
        {
            Log.Info("Map", "Map built");

            gameActivity = (GameActivity)context;
            utilities = new Utilities(gameActivity);
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
                false,
                false,
                false,
                true,
                true
            };

            utilities.BuildMap(googleMap, mapSettingsBools);
            utilities.SetMarker();
            utilities.SetPolygon(gameActivity.playArea.vertices);
        }
    }
}