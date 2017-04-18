using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Util;

namespace CaptureTheCampus
{
    public class SetPosition : Activity, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener, Android.Gms.Location.ILocationListener
    {
        private SetActivity setActivity;

        private LocationRequest locRequest;

        public SetPosition(Context context)
        {
            Log.Info("Position", "Position built");

            setActivity = (SetActivity)context;

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
            LocationServices.FusedLocationApi.RequestLocationUpdates(setActivity.apiClient, locRequest, this);
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
            if (setActivity.positionBool == false)
            {
                // This method returns changes in the user's location if they've been requested

                // You must implement this to implement the Android.Gms.Locations.ILocationListener Interface
                Log.Debug("LocationClient", "going to UpdateLocation");

                UpdateLocationInformation(location);
            }
            else
            {
                setActivity.StopUpdatePosition();
            }
        }

        public void UpdateLocationInformation(Location location)
        {
            Log.Debug("Position", location.Latitude.ToString() + "," + location.Longitude.ToString());

            LatLng position = new LatLng(location.Latitude, location.Longitude);

            UpdateLocation(position);
        }

        private void UpdateLocation(LatLng position)
        {
            Log.Debug("LocationClient", "Location updated");

            SetCamera(position);
        }

        private void SetCamera(LatLng position)
        {
            Log.Debug("SetCamera", "Setting camera position");

            int[] builderSettingsInts = { 15, 0, 0 };

            CameraPosition cameraPosition = CameraBuilder(position, builderSettingsInts).Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);

            setActivity.map.MoveCamera(cameraUpdate);

            setActivity.positionBool = true;
        }

        private CameraPosition.Builder CameraBuilder(LatLng position, int[] builderSettingsInts)
        {
            Log.Debug("CameraBuilder", "Building Camera");

            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();

            builder.Target(position);
            builder.Zoom(builderSettingsInts[0]);
            builder.Bearing(builderSettingsInts[1]);
            builder.Tilt(builderSettingsInts[2]);

            return builder;
        }
    }
}