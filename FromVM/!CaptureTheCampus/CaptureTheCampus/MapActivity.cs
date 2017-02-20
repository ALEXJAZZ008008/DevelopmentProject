using Android.App;
using Android.Gms.Maps;
using Android.OS;

namespace CaptureTheCampus
{
    [Activity(Label = "@string/MapActivityName")]
    public class MapActivity : Activity, IOnMapReadyCallback
    {
        private GoogleMap map;

        public void OnMapReady(GoogleMap googleMap)
        {
            map = googleMap;
            map.MapType = GoogleMap.MapTypeNormal;
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Map);
            StartMap();
        }

        private void StartMap()
        {
            if (map == null)
            {
                FragmentManager.FindFragmentById<MapFragment>(Resource.Id.map).GetMapAsync(this);
            }
        }
    }
}