using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Hardware;
using Android.Util;

namespace CaptureTheCampus
{
    public class Rotation: View, ISensorEventListener
    {
        private GameActivity gameActivity;
        private Utilities utilities;

        public Rotation (Context context) : base (context)
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