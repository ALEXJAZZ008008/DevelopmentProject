using Android.Content;
using Android.Gms.Maps.Model;
using Android.Util;

namespace CaptureTheCampus
{
    public class Hazards
    {
        private GameActivity gameActivity;
        private Utilities utilities;
        private Maths maths;

        private Circle circle;

        public Hazards(Context context, Circle inCircle)
        {
            Log.Info("Area", "Area built");

            gameActivity = (GameActivity)context;
            utilities = new Utilities(gameActivity);
            maths = new Maths(gameActivity);

            circle = inCircle;
        }

        public void RunHazards()
        {

        }
    }
}