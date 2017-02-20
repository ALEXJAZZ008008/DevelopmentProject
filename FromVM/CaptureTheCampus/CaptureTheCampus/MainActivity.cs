using Android.App;
using Android.Content;
using Android.OS;

namespace CaptureTheCampus
{
    [Activity(Label = "@string/MainActivityLabel", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);

            GoToMapActivity();
        }

        private void GoToMapActivity()
        {
            Intent intent = new Intent(this, typeof(MapActivity));
            StartActivity(intent);
        }
    }
}

