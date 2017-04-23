using Android.Content;
using Android.Hardware;
using Android.Runtime;
using Android.Util;
using Android.Views;

namespace CaptureTheCampus.Game
{
    public class Rotation : View, ISensorEventListener
    {
        volatile private GameActivity gameActivity;
        volatile private Utilities utilities;

        public Rotation(Context context) : base(context)
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
            if (utilities.UpdateRotation(gameActivity.playerPosition, e))
            {
                Log.Info("OnSensorChanged", "Sensor changed");
            }
        }
    }
}