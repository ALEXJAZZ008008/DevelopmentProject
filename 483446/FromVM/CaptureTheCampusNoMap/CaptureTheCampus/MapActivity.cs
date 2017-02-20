using Android.App;
using Android.OS;

namespace CaptureTheCampus
{
    [Activity(Label = "@string/MapActivityLabel", Icon = "@drawable/icon")]
    public class MapActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Map);
        }
    }
}

