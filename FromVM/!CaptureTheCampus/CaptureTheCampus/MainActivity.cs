using Android.App;
using Android.Content;
using Android.OS;

namespace CaptureTheCampus
{
    [Activity(Label = "@string/MainActivityName", MainLauncher = true, Icon = "@drawable/Icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);
            GoToMapActivity();
        }

        private void GoToMapActivity()
        {
            Intent intent = new Intent(this, typeof(MapActivity));
            StartActivity(intent);
        }
    }
}

