using Android.App;
using Android.Widget;
using Android.OS;

namespace CaptureTheCampus
{
    [Activity(Label = "CaptureTheCampus", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Client client = new Client();

            // Set our view from the "main" layout resource
            // SetContentView (Resource.Layout.Main);
        }
    }
}

