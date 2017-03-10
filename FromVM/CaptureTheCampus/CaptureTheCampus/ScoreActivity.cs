using Android.App;
using Android.OS;
using Android.Util;
using Android.Widget;

namespace CaptureTheCampus
{
    [Activity(Label = "@string/ScoreActivityLabel", Icon = "@drawable/icon")]
    public class ScoreActivity : Activity
    {
        private int score;
        private TextView scoreTextView;

        private Button MainMenuButton;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Log.Debug("OnCreate", "OnCreate called, initializing views...");

            // Set our view from the "menu" layout resource
            SetContentView(Resource.Layout.Score);

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
            GetScore();

            scoreTextView = (TextView)FindViewById(Resource.Id.Score);
            scoreTextView.Text = "Score: " + score.ToString();

            MainMenuButton = FindViewById<Button>(Resource.Id.MainMenuButton);
        }

        private void GetScore()
        {
            score = Intent.GetIntExtra("score", 0);
        }

        private void StartButtonEventHandlers()
        {
            MainMenuButton.Click += (sender, e) =>
            {
                ReturnToMainMenu();
            };
        }

        private void StopButtonEventHandlers()
        {
            MainMenuButton.Click -= (sender, e) =>
            {
                ReturnToMainMenu();
            };
        }

        private void ReturnToMainMenu()
        {
            Finish();
        }
    }
}