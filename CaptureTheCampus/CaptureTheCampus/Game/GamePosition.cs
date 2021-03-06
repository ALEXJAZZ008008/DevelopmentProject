using Android.Content;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Util;
using Android.Views;

namespace CaptureTheCampus.Game
{
    public class GamePosition : View, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener, Android.Gms.Location.ILocationListener
    {
        volatile private GameActivity gameActivity;
        volatile private Utilities utilities;

        volatile private LocationRequest locRequest;

        public GamePosition(Context context) : base(context)
        {
            Log.Info("Position", "Position built");

            gameActivity = (GameActivity)context;
            utilities = new Utilities(gameActivity);

            // generate a location request that we will pass into a call for location updates
            locRequest = new LocationRequest();
        }

        public void OnConnected(Bundle connectionHint)
        {
            // This method is called when we connect to the LocationClient. We can start location updated directly form
            // here if desired, or we can do it in a lifecycle method, as shown above 

            // You must implement this to implement the IGooglePlayServicesClientConnectionCallbacks Interface
            Log.Info("LocationClient", "Now connected to client");

            SetupLocRequest();
        }

        private void SetupLocRequest()
        {
            // Setting location priority to PRIORITY_HIGH_ACCURACY (100)
            locRequest.SetPriority(100);

            // Setting interval between updates, in milliseconds
            // NOTE: the default FastestInterval is 1 second. If you want to receive location updates more than 
            // once a minute, you _must_ also change the FastestInterval to be less than or equal to your Interval
            locRequest.SetFastestInterval(1000);
            locRequest.SetInterval(1000);

            Log.Debug("LocationRequest", "Request priority set to status code {0}, interval set to {1} ms", locRequest.Priority.ToString(), locRequest.Interval.ToString());

            // pass in a location request and LocationListener
            LocationServices.FusedLocationApi.RequestLocationUpdates(gameActivity.googleAPIClient, locRequest, this);
            // In OnLocationChanged (below), we will make calls to update the UI
            // with the new location data
        }

        public void OnDisconnected()
        {
            // This method is called when we disconnect from the LocationClient.

            // You must implement this to implement the IGooglePlayServicesClientConnectionCallbacks Interface
            Log.Info("LocationClient", "Now disconnected from client");
        }

        public void OnConnectionSuspended(int cause)
        {
            Log.Info("LocationClient", "Connection suspended");
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            // This method is used to handle connection issues with the Google Play Services Client (LocationClient). 
            // You can check if the connection has a resolution (bundle.HasResolution) and attempt to resolve it

            // You must implement this to implement the IGooglePlayServicesClientOnConnectionFailedListener Interface
            Log.Info("LocationClient", "Connection failed, attempting to reach google play services");
        }

        public void OnLocationChanged(Location location)
        {
            if (location.Latitude != gameActivity.playerArray[gameActivity.playerPosition].currentPosition.Latitude || location.Longitude != gameActivity.playerArray[gameActivity.playerPosition].currentPosition.Longitude)
            {
                // This method returns changes in the user's location if they've been requested

                // You must implement this to implement the Android.Gms.Locations.ILocationListener Interface
                Log.Debug("LocationClient", "going to UpdateLocation");

                utilities.UpdateLocationInformation(gameActivity.playerPosition, new LatLng(location.Latitude, location.Longitude));
            }
        }
    }
}