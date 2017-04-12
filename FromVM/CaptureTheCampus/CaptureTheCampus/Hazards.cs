using Android.Content;
using Android.Gms.Maps.Model;
using Android.Util;
using System;
using System.Collections.Generic;

namespace CaptureTheCampus
{
    public class Hazards
    {
        private GameActivity gameActivity;
        private Utilities utilities;
        private Maths maths;

        volatile private Circle circle;
        private double velocity;
        private int degrees;

        public Hazards(Context context)
        {
            Log.Info("Area", "Area built");

            gameActivity = (GameActivity)context;
            utilities = new Utilities(gameActivity);
            maths = new Maths(gameActivity);

            velocity = 1d / (111d / (10d / (60d * 60d)));
            degrees = 0;

            SetDegrees();
        }

        public void SetCircle(Circle inCircle)
        {
            circle = inCircle;
        }

        public void SetDegrees()
        {
            Random random = new Random();

            degrees = random.Next(0, 361);
        }

        public void RunHazards(LinkedList<LatLng> vertices)
        {
            gameActivity.RunOnUiThread(() => CheckPosition(vertices));
        }

        private void CheckPosition(LinkedList<LatLng> vertices)
        {
            LatLng position = circle.Center;
            double radius = circle.Radius;

            if (!maths.PointInPolygon(vertices, position))
            {
                gameActivity.circle.Center = utilities.FindCirclePosition(vertices, radius);

                SetCircle(gameActivity.circle);
                SetDegrees();
            }
            else
            {
                UpdatePosition();

                if (utilities.CircleIntersectPolygon(vertices, circle.Center, radius))
                {
                    gameActivity.circle.Center = position;

                    SetCircle(gameActivity.circle);
                }
            }
        }

        private void UpdatePosition()
        {
            double radians = DegreesToRadians();

            circle.Center = new LatLng(circle.Center.Latitude + (Math.Cos(radians) * velocity), circle.Center.Longitude + (Math.Sin(radians) * velocity));

            gameActivity.circle.Center = circle.Center;

            UpdateUnitVector();
        }

        private double DegreesToRadians()
        {
            return (degrees - 180) * (Math.PI / 180);
        }

        private void UpdateUnitVector()
        {
            Random random = new Random();

            int acceleration = random.Next(0, 121) - 60;

            int temporaryDegrees = degrees + acceleration;

            if(temporaryDegrees > 360)
            {
                degrees = acceleration - (360 - temporaryDegrees);
            }
            else
            {
                if (temporaryDegrees < 0)
                {
                    degrees = 360 - (acceleration - temporaryDegrees);
                }
                else
                {
                    degrees = temporaryDegrees;
                }
            }
        }
    }
}