using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;

namespace CaptureTheCampus
{
    [Activity(Label = "@string/SearchActivityLabel", Icon = "@drawable/icon")]
    public class SearchActivity : Activity
    {
        private string searchType;

        private string text;

        private Host host;
        private Join join;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Log.Debug("OnCreate", "OnCreate called, initializing views...");

            // Set our view from the "menu" layout resource
            SetContentView(Resource.Layout.Search);

            searchType = Intent.GetStringExtra("searchType");

            if(searchType == "Host")
            {
                host = new Host();

                text = "Host";
            }
            else
            {
                if (searchType == "Join")
                {
                    join = new Join();

                    text = "Join";
                }
            }
        }

        private void GoToSetActivity(string gameType)
        {
            Log.Debug("GoToSetActivity", "GoToSetActivity called, going to SetActivity...");

            Intent intent = new Intent(this, typeof(SetActivity));
            intent.PutExtra("gameType", gameType);
            StartActivity(intent);

            Finish();
        }
    }
}