using Android.App;
using Android.Content;
using Android.Hardware;
using Android.Runtime;
using Android.Util;

namespace CaptureTheCampus
{
    public class Rotation: Activity, ISensorEventListener
    {
        private GameActivity gameActivity;
        private Utilities utilities;

        public Rotation (Context context)
        {
            Log.Info("Rotation", "Rotation built");

            gameActivity = (GameActivity)context;
            utilities = new Utilities(gameActivity);
        }

        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
            Log.Info("OnAccuracyChanged", "Accuracy changed");
        }

        public void OnSensorChanged(SensorEvent e)
        {
            if(utilities.UpdateRotation(e))
            {
                Log.Info("OnSensorChanged", "Sensor changed");
            }
        }
    }
}