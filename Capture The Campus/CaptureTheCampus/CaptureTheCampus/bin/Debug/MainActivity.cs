using Android.App;
using Android.OS;
using Android.Content;

namespace CaptureTheCampus
{
    [Activity(Label = "@string/ApplicationName", MainLauncher = true, Icon = "@drawable/icon")]

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