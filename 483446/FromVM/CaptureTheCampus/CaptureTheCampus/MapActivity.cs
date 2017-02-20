using Android.App;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;

namespace CaptureTheCampus
{
    [Activity(Label = "@string/MapActivityLabel", Icon = "@drawable/icon")]
    public class MapActivity : Activity, IOnMapReadyCallback
    {
        private GoogleMap map;
        private LatLng defaultPosition = new LatLng(53.7715, -0.3674);

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Map);

            StartMap();
        }

        private void StartMap()
        {
            if (map == null)
            {
                FragmentManager.FindFragmentById<MapFragment>(Resource.Id.map).GetMapAsync(this);
            }
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            BuildMap(googleMap);

            SetCamera();

            CreateUserMarker();
        }

        private void BuildMap(GoogleMap googleMap)
        {
            map = googleMap;
            map.MapType = GoogleMap.MapTypeTerrain;

            MapSettings();
        }

        private void MapSettings()
        {
            map.UiSettings.CompassEnabled = false;
            map.UiSettings.IndoorLevelPickerEnabled = false;
            map.UiSettings.MapToolbarEnabled = false;
            map.UiSettings.MyLocationButtonEnabled = false;
            map.UiSettings.RotateGesturesEnabled = false;
            map.UiSettings.ScrollGesturesEnabled = false;
            map.UiSettings.TiltGesturesEnabled = false;
            map.UiSettings.ZoomControlsEnabled = false;
            map.UiSettings.ZoomGesturesEnabled = false;
        }

        private void SetCamera()
        {
            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();

            builder.Target(defaultPosition);
            builder.Zoom(18);
            builder.Bearing(0);
            builder.Tilt(21);

            CameraPosition initialCameraPosition = builder.Build();
            CameraUpdate initialCameraUpdate = CameraUpdateFactory.NewCameraPosition(initialCameraPosition);

            map.MoveCamera(initialCameraUpdate);
        }

        private void CreateUserMarker()
        {
            MarkerOptions userMarker = new MarkerOptions();

            userMarker.SetPosition(defaultPosition);
            userMarker.SetTitle(Resources.GetString(Resource.String.Username));
            userMarker.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueRed));

            map.AddMarker(userMarker);
        }
    }
}

