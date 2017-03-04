using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.OS;
using Android.Util;
using Android.Widget;

namespace CaptureTheCampus
{
    [Activity(Label = "@string/MainActivityLabel", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private bool isGooglePlayServicesInstalled;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Log.Debug("OnCreate", "OnCreate called, initializing views...");

            // Set our view from the "Main" layout resource
            SetContentView (Resource.Layout.Main);

            isGooglePlayServicesInstalled = IsGooglePlayServicesInstalled();

            if (isGooglePlayServicesInstalled)
            {
                GoToMenuActivity();
            }
            else
            {
                Log.Error("OnCreate", "Google Play Services is not installed");
                Toast.MakeText(this, "Google Play Services is not installed", ToastLength.Long).Show();
            }

            Finish();
        }

        private bool IsGooglePlayServicesInstalled()
        {
            int queryResult = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);

            if (queryResult == ConnectionResult.Success)
            {
                Log.Info("MainActivity", "Google Play Services is installed on this device.");

                return true;
            }

            if (GoogleApiAvailability.Instance.IsUserResolvableError(queryResult))
            {
                // Show error dialog to let user debug google play services

                string errorString = GoogleApiAvailability.Instance.GetErrorString(queryResult);
                Log.Error("MainActivity", "There is a problem with Google Play Services on this device: {0} - {1}", queryResult, errorString);
            }

            return false;
        }

        private void GoToMenuActivity()
        {
            Log.Debug("GoToMenuActivity", "GoToMenuActivity called, going to MenuActivity...");

            Intent intent = new Intent(this, typeof(MenuActivity));
            StartActivity(intent);
        }
    }
}

