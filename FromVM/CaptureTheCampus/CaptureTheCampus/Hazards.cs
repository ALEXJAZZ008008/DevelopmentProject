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
        private int attempts;

        public Hazards(Context context)
        {
            Log.Info("Area", "Area built");

            gameActivity = (GameActivity)context;
            utilities = new Utilities(gameActivity);
            maths = new Maths(gameActivity);

            velocity = 1d / (111.3d / (10d / (60d * 60d)));
            degrees = 0;

            SetDegrees();

            attempts = 0;
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

        public void RunHazards(LinkedList<LatLng> polygonVertices, LinkedList<LatLng> pathVertices)
        {
            gameActivity.RunOnUiThread(() => CheckPosition(polygonVertices, pathVertices));
        }

        private void CheckPosition(LinkedList<LatLng> polygonVertices, LinkedList<LatLng> pathVertices)
        {
            LatLng position = circle.Center;
            double radius = circle.Radius;

            if (!maths.PointInPolygon(polygonVertices, position))
            {
                gameActivity.circle.Center = utilities.FindCirclePosition(polygonVertices, radius);

                SetCircle(gameActivity.circle);
                SetDegrees();
            }
            else
            {
                LatLng[] interceptionVertex;

                UpdatePosition(pathVertices, radius);

                if (utilities.CircleIntersectPolygon(polygonVertices, circle.Center, radius, out interceptionVertex))
                {
                    gameActivity.circle.Center = position;

                    SetCircle(gameActivity.circle);

                    ReverseUnitVector(interceptionVertex);

                    if (attempts >= 10)
                    {
                        gameActivity.circle.Center = utilities.FindCirclePosition(polygonVertices, radius);

                        SetCircle(gameActivity.circle);
                        SetDegrees();
                    }
                    else
                    {
                        attempts++;
                    }
                }
                else
                {
                    attempts = 0;
                }
            }
        }

        private void UpdatePosition(LinkedList<LatLng> pathVertices, double radius)
        {
            LatLng degreesUnitVector = maths.DegreesToUnitVector(degrees);

            circle.Center = new LatLng(circle.Center.Latitude + (degreesUnitVector.Latitude * velocity), circle.Center.Longitude + (degreesUnitVector.Longitude * velocity));

            gameActivity.circle.Center = circle.Center;

            CheckPlayerInterception(pathVertices, radius);

            UpdateUnitVector();
        }

        private void CheckPlayerInterception(LinkedList<LatLng> pathVertices, double radius)
        {
            LatLng[] interceptionVertex;

            if (pathVertices.First != null && pathVertices.First.Next != null)
            {
                if (utilities.CircleIntersectPolygon(pathVertices, circle.Center, radius, out interceptionVertex))
                {
                    ;
                }
            }
        }

        private void UpdateUnitVector()
        {
            Random random = new Random();

            int acceleration = random.Next(0, 91) - 45;

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

        private void ReverseUnitVector(LatLng[] interceptionVertex)
        {
            LatLng interceptionNormalVector = new LatLng(interceptionVertex[1].Longitude - interceptionVertex[0].Longitude, -(interceptionVertex[1].Latitude - interceptionVertex[0].Latitude));

            double interceptionNormalVectorMagnitude = Math.Sqrt(Math.Pow(interceptionNormalVector.Latitude, 2) + Math.Pow(interceptionNormalVector.Longitude, 2));
            interceptionNormalVector = new LatLng(Math.Abs(interceptionNormalVector.Latitude / interceptionNormalVectorMagnitude) * -1, Math.Abs(interceptionNormalVector.Longitude / interceptionNormalVectorMagnitude) * -1);

            if(interceptionNormalVector.Latitude == 0)
            {
                interceptionNormalVector.Latitude = 1;
            }

            if (interceptionNormalVector.Longitude == 0)
            {
                interceptionNormalVector.Longitude = 1;
            }

            LatLng degreesUnitVector = maths.DegreesToUnitVector(degrees);
            degreesUnitVector = new LatLng(degreesUnitVector.Latitude * interceptionNormalVector.Latitude, degreesUnitVector.Longitude * interceptionNormalVector.Longitude);

            double degreesUnitVectorMagnitude = Math.Sqrt(Math.Pow(degreesUnitVector.Latitude, 2) + Math.Pow(degreesUnitVector.Longitude, 2));
            degreesUnitVector = new LatLng(degreesUnitVector.Latitude / degreesUnitVectorMagnitude, degreesUnitVector.Longitude / degreesUnitVectorMagnitude);

            degrees = maths.UnitVectorToDegrees(degreesUnitVector);
        }
    }
}