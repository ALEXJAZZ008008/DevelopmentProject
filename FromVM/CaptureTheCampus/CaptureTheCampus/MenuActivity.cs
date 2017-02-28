using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Widget;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CaptureTheCampus
{
    [Activity(Label = "@string/MenuActivityLabel", Icon = "@drawable/icon")]
    public class MenuActivity : Activity
    {
        private Dictionary<string, Task> taskDictionary;
        private Task task;

        private Button SinglePlayerButton, HostMultiplayerButton, JoinMultiplayerButton;

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

            StartTask(StartButtonEventHandlers());
        }

        protected override void OnPause()
        {
            Log.Debug("OnPause", "OnPause called, stopping event handlers");

            StartTask(StopButtonEventHandlers());

            base.OnPause();
        }

        private void Initialise()
        {
            taskDictionary = new Dictionary<string, Task>();

            SinglePlayerButton = FindViewById<Button>(Resource.Id.SinglePlayerButton);
            HostMultiplayerButton = FindViewById<Button>(Resource.Id.HostMultiplayerButton);
            JoinMultiplayerButton = FindViewById<Button>(Resource.Id.JoinMultiplayerButton);
        }

        private void StartTask(Action method)
        {
            Log.Debug("StartTask", "Starting task");

            string taskName = "Task" + taskDictionary.Count;

            task = new Task(() => method());

            taskDictionary.Add(taskName, task);

            task.Start();
        }

        private Action StartButtonEventHandlers()
        {
            SinglePlayerButton.Click += (sender, e) =>
            {
                GoToSetActivity();
            };

            HostMultiplayerButton.Click += (sender, e) =>
            {
                GoToSearchActivity("Host");
            };

            JoinMultiplayerButton.Click += (sender, e) =>
            {
                GoToSearchActivity("Join");
            };

            return null;
        }

        private Action StopButtonEventHandlers()
        {
            SinglePlayerButton.Click -= (sender, e) =>
            {
                GoToSetActivity();
            };

            HostMultiplayerButton.Click -= (sender, e) =>
            {
                GoToSearchActivity("Host");
            };

            JoinMultiplayerButton.Click -= (sender, e) =>
            {
                GoToSearchActivity("Join");
            };

            return null;
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