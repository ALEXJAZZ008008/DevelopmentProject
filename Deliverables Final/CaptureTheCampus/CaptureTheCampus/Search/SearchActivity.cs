using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using System;
using System.Threading;

namespace CaptureTheCampus.Search
{
    [Activity(Label = "@string/SearchActivityLabel", Icon = "@drawable/icon")]
    public class SearchActivity : Activity
    {
        volatile public string searchType;
        volatile public string ip;
        volatile public int playerPosition, numberOfPlayers;

        volatile private Host host;
        volatile private Join join;

        volatile public CancellationTokenSource cancelationTokenSource;

        volatile public TextView searchTextView;
        volatile public Button searchButton;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Log.Debug("OnCreate", "OnCreate called, initializing views...");

            // Set our view from the "menu" layout resource
            SetContentView(Resource.Layout.Search);

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
            searchType = Intent.GetStringExtra("searchType");
            ip = string.Empty;
            playerPosition = 0;
            numberOfPlayers = 0;

            searchTextView = (TextView)FindViewById(Resource.Id.Search);
            searchButton = FindViewById<Button>(Resource.Id.SearchButton);

            if (searchType == "Host")
            {
                host = new Host(this);
            }
            else
            {
                if (searchType == "Join")
                {
                    join = new Join(this);
                }
            }
        }

        private void StartButtonEventHandlers()
        {
            searchButton.Click += (sender, e) =>
            {
                ButtonEvent();
            };
        }

        private void StopButtonEventHandlers()
        {
            searchButton.Click -= (sender, e) =>
            {
                ButtonEvent();
            };
        }

        private void ButtonEvent()
        {
            if (searchType == "Host")
            {
                if (playerPosition > 0)
                {
                    GoToSetActivity(searchType);
                }
            }
            else
            {
                if (searchType == "Join")
                {
                    if (ip == string.Empty)
                    {
                        GoToTextEntry();
                    }
                }
            }
        }

        private void GoToSetActivity(string gameType)
        {
            Log.Debug("GoToSetActivity", "GoToSetActivity called, going to SetActivity...");

            Intent intent = new Intent(this, typeof(Set.SetActivity));
            intent.PutExtra("gameType", gameType);
            intent.PutExtra("numberOfPlayers", numberOfPlayers.ToString());
            intent.PutExtra("playerPosition", playerPosition.ToString());
            StartActivity(intent);

            cancelationTokenSource.Cancel();

            Finish();
        }

        private void GoToTextEntry()
        {
            EditText editText = new EditText(this);

            // Build the dialog.
            AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(this);

            // Create empty event handlers, we will override them manually instead of letting the builder handling the clicks.
            alertDialogBuilder.SetTitle("Please enter IP:");
            alertDialogBuilder.SetView(editText);
            alertDialogBuilder.SetCancelable(false);
            alertDialogBuilder.SetPositiveButton("Set IP", (EventHandler<DialogClickEventArgs>)null);
            alertDialogBuilder.SetNegativeButton("Back", (EventHandler<DialogClickEventArgs>)null);

            AlertDialog alertDialog = alertDialogBuilder.Create();

            // Show the dialog. This is important to do before accessing the buttons.
            alertDialog.Show();

            // Get the buttons.
            var positiveButton = alertDialog.GetButton((int)DialogButtonType.Positive);
            var negativeButton = alertDialog.GetButton((int)DialogButtonType.Negative);

            // Assign our handlers.
            positiveButton.Click += (sender, args) => PositiveButtonEvent(alertDialog, editText, editText);
            negativeButton.Click += (sender, args) => NegativeButtonEvent(alertDialog, editText);

            ShowKeyboard(editText);
        }

        public void PositiveButtonEvent(AlertDialog alertDialog, EditText editText, View view)
        {
            HideKeyboard(view);

            alertDialog.Dismiss();

            ip = editText.Text;
        }

        public void NegativeButtonEvent(AlertDialog alertDialog, View view)
        {
            HideKeyboard(view);

            alertDialog.Dismiss();
        }

        public void ShowKeyboard(View view)
        {
            view.RequestFocus();

            InputMethodManager inputMethodManager = Application.GetSystemService(InputMethodService) as InputMethodManager;
            inputMethodManager.ShowSoftInput(view, ShowFlags.Forced);
            inputMethodManager.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
        }

        public void HideKeyboard(View view)
        {
            InputMethodManager inputMethodManager = Application.GetSystemService(InputMethodService) as InputMethodManager;
            inputMethodManager.HideSoftInputFromWindow(view.WindowToken, HideSoftInputFlags.None);
        }

        public void GoToGameActivity(string gameType)
        {
            Log.Debug("GoToGameActivity", "GoToGameActivity called, going to GameActivity...");

            Intent intent = new Intent(this, typeof(Game.GameActivity));
            intent.PutExtra("gameType", gameType);
            intent.PutExtra("numberOfPlayers", numberOfPlayers.ToString());
            intent.PutExtra("playerPosition", playerPosition.ToString());
            intent.PutExtra("ip", ip);
            StartActivity(intent);

            cancelationTokenSource.Cancel();

            Finish();
        }
    }
}