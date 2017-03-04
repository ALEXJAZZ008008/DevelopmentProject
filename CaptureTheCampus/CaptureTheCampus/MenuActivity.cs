using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Widget;

namespace CaptureTheCampus
{
    [Activity(Label = "@string/MenuActivityLabel", Icon = "@drawable/icon")]
    public class MenuActivity : Activity
    {
        private Button singlePlayerButton, hostMultiplayerButton, joinMultiplayerButton;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Log.Debug("OnCreate", "OnCreate called, initializing views...");

            // Set our view from the "menu" layout resource
            SetContentView(Resource.Layout.Menu);

            Initialise();
        }

        protected override void OnResume()
        {
            base.OnResume();
            Log.Debug("OnResume", "OnResume called, starting event handlers...");

            StartButtonEventHandlers();
        }

        protected override void OnPause()
        {
            Log.Debug("OnPause", "OnPause called, stopping event handlers");

            StopButtonEventHandlers();

            base.OnPause();
        }

        private void Initialise()
        {
            singlePlayerButton = FindViewById<Button>(Resource.Id.SinglePlayerButton);
            hostMultiplayerButton = FindViewById<Button>(Resource.Id.HostMultiplayerButton);
            joinMultiplayerButton = FindViewById<Button>(Resource.Id.JoinMultiplayerButton);
        }

        private void StartButtonEventHandlers()
        {
            singlePlayerButton.Click += (sender, e) =>
            {
                GoToSetActivity();
            };

            hostMultiplayerButton.Click += (sender, e) =>
            {
                GoToSearchActivity("Host");
            };

            joinMultiplayerButton.Click += (sender, e) =>
            {
                GoToSearchActivity("Join");
            };
        }

        private void StopButtonEventHandlers()
        {
            singlePlayerButton.Click -= (sender, e) =>
            {
                GoToSetActivity();
            };

            hostMultiplayerButton.Click -= (sender, e) =>
            {
                GoToSearchActivity("Host");
            };

            joinMultiplayerButton.Click -= (sender, e) =>
            {
                GoToSearchActivity("Join");
            };
        }

        private void GoToSetActivity()
        {
            Log.Debug("GoToSetActivity", "GoToSetActivity called, going to SetActivity...");

            Intent intent = new Intent(this, typeof(SetActivity));
            intent.PutExtra("gameType", "Single Player");
            StartActivity(intent);
        }

        private void GoToSearchActivity(string searchType)
        {
            Log.Debug("GoToSearchActivity", "GoToSearchActivity called, going to SearchActivity...");

            Intent intent = new Intent(this, typeof(SearchActivity));
            intent.PutExtra("searchType", searchType);
            StartActivity(intent);
        }
    }
}