using Android.Content;
using Android.Gms.Maps.Model;
using Android.Util;
using System;
using System.Collections.Generic;

namespace CaptureTheCampus.Game
{
    public class Hazards
    {
        volatile private GameActivity gameActivity;
        volatile private Utilities utilities;

        volatile private Circle circle;
        volatile private float velocity;
        volatile private int degrees, attempts;

        public Hazards(Context context)
        {
            Log.Info("Area", "Area built");

            gameActivity = (GameActivity)context;
            utilities = new Utilities(gameActivity);

            velocity = 1f / (gameActivity.latLngToKM / (10f / (60f * 60f)));
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

        public void RunHazards(LinkedList<LatLng> polygonVertices)
        {
            if (gameActivity.gameType == "Single Player" || gameActivity.gameType == "Host")
            {
                gameActivity.RunOnUiThread(() => CheckPosition(polygonVertices));
            }
            else
            {
                if(gameActivity.gameType == "Join")
                {
                    gameActivity.RunOnUiThread(() => CheckPlayerInterception(circle.Radius));
                }
            }
        }

        private void CheckPosition(LinkedList<LatLng> polygonVertices)
        {
            LatLng position = circle.Center;
            double radius = circle.Radius;

            if (!Static.Maths.PointInPolygon(polygonVertices, position))
            {
                gameActivity.circle.Center = utilities.FindCirclePosition(polygonVertices, radius);

                SetCircle(gameActivity.circle);
                SetDegrees();
            }
            else
            {
                LatLng[] interceptionVertex;

                UpdatePosition(radius);

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

        private void UpdatePosition(double radius)
        {
            LatLng degreesUnitVector = Static.Maths.DegreesToUnitVector(degrees);

            circle.Center = new LatLng(circle.Center.Latitude + (degreesUnitVector.Latitude * velocity), circle.Center.Longitude + (degreesUnitVector.Longitude * velocity));

            gameActivity.circle.Center = circle.Center;

            CheckPlayerInterception(radius);

            UpdateUnitVector();
        }

        private void CheckPlayerInterception(double radius)
        {
            LinkedList<LatLng> pathVertices = new LinkedList<LatLng>(); ;

            bool deathBool;

            LatLng[] interceptionVertex;

            for (int i = 0; i < gameActivity.numberOfPlayers; i++)
            {
                gameActivity.CopyVertices(pathVertices, gameActivity.playerArray[i].vertices);
                gameActivity.CopyBool(out deathBool, gameActivity.playerArray[i].deathBool);

                if (pathVertices.First != null && pathVertices.First.Next != null)
                {
                    if (utilities.CircleIntersectPolygon(pathVertices, circle.Center, radius, out interceptionVertex) && !deathBool)
                    {
                        gameActivity.KillPlayer(i);
                    }
                }
            }
        }

        private void UpdateUnitVector()
        {
            Random random = new Random();

            int acceleration = random.Next(0, 91) - 45;

            int temporaryDegrees = degrees + acceleration;

            if (temporaryDegrees > 360)
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

            if (interceptionNormalVector.Latitude == 0)
            {
                interceptionNormalVector.Latitude = 1;
            }

            if (interceptionNormalVector.Longitude == 0)
            {
                interceptionNormalVector.Longitude = 1;
            }

            LatLng degreesUnitVector = Static.Maths.DegreesToUnitVector(degrees);
            degreesUnitVector = new LatLng(degreesUnitVector.Latitude * interceptionNormalVector.Latitude, degreesUnitVector.Longitude * interceptionNormalVector.Longitude);

            double degreesUnitVectorMagnitude = Math.Sqrt(Math.Pow(degreesUnitVector.Latitude, 2) + Math.Pow(degreesUnitVector.Longitude, 2));
            degreesUnitVector = new LatLng(degreesUnitVector.Latitude / degreesUnitVectorMagnitude, degreesUnitVector.Longitude / degreesUnitVectorMagnitude);

            degrees = Static.Maths.UnitVectorToDegrees(degreesUnitVector);
        }
    }
}