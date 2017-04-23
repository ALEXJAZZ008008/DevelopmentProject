using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Hardware;
using Android.Util;
using System;
using System.Collections.Generic;

namespace CaptureTheCampus.Game
{
    public class Utilities
    {
        volatile private GameActivity gameActivity;
        volatile private Area area;

        public Utilities(Context context)
        {
            Log.Info("Utilities", "Utilities built");

            gameActivity = (GameActivity)context;
            area = new Area(gameActivity, this);
        }

        public void BuildMap(GoogleMap googleMap, bool[] mapSettingsBools)
        {
            Log.Debug("BuildMap", "Building map");

            googleMap.MapType = GoogleMap.MapTypeTerrain;
            MapSettings(googleMap, mapSettingsBools);

            gameActivity.googleMap = googleMap;
        }

        public void MapSettings(GoogleMap googleMap, bool[] mapSettingsBools)
        {
            Log.Debug("MapSettings", "Setting map UI restrictions");

            googleMap.UiSettings.CompassEnabled = mapSettingsBools[0];
            googleMap.UiSettings.IndoorLevelPickerEnabled = mapSettingsBools[1];
            googleMap.UiSettings.MapToolbarEnabled = mapSettingsBools[2];
            googleMap.UiSettings.MyLocationButtonEnabled = mapSettingsBools[3];
            googleMap.UiSettings.RotateGesturesEnabled = mapSettingsBools[4];
            googleMap.UiSettings.ScrollGesturesEnabled = mapSettingsBools[5];
            googleMap.UiSettings.TiltGesturesEnabled = mapSettingsBools[6];
            googleMap.UiSettings.ZoomControlsEnabled = mapSettingsBools[7];
            googleMap.UiSettings.ZoomGesturesEnabled = mapSettingsBools[8];
        }

        public void SetMarker()
        {
            Log.Debug("SetMarker", "Setting marker position");

            for (int i = 0; i < gameActivity.numberOfPlayers; i++)
            {
                gameActivity.playerArray[i].marker = gameActivity.googleMap.AddMarker(BuildMarker(i));
            }
        }

        private MarkerOptions BuildMarker(int playerPosition)
        {
            Log.Debug("BuildMarker", "Building marker");

            MarkerOptions markerOptions = new MarkerOptions();

            markerOptions.SetIcon(BitmapDescriptorFactory.DefaultMarker(Colour(playerPosition)));
            markerOptions.SetPosition(gameActivity.playerArray[playerPosition].currentPosition);

            return markerOptions;
        }

        public float Colour(int count)
        {
            while (true)
            {
                switch (count)
                {
                    case 0:
                        return BitmapDescriptorFactory.HueAzure;

                    case 1:
                        return BitmapDescriptorFactory.HueBlue;

                    case 2:
                        return BitmapDescriptorFactory.HueCyan;

                    case 3:
                        return BitmapDescriptorFactory.HueGreen;

                    case 4:
                        return BitmapDescriptorFactory.HueMagenta;

                    case 5:
                        return BitmapDescriptorFactory.HueOrange;

                    case 6:
                        return BitmapDescriptorFactory.HueRed;

                    case 7:
                        return BitmapDescriptorFactory.HueRose;

                    case 8:
                        return BitmapDescriptorFactory.HueViolet;

                    case 9:
                        return BitmapDescriptorFactory.HueYellow;

                    default:
                        count = count - 10;
                        break;
                }
            }
        }

        public void UpdateLocationInformation(int playerPosition, LatLng position)
        {
            Log.Debug("Position", gameActivity.playerArray[playerPosition].currentPosition.ToString());

            gameActivity.playerArray[playerPosition].currentPosition = new LatLng(position.Latitude, position.Longitude);

            UpdateLocation(playerPosition);
            area.UpdatePaths(playerPosition);
        }

        private void UpdateLocation(int playerPosition)
        {
            Log.Debug("LocationClient", "Location updated");

            if (!gameActivity.cameraInitiallySet)
            {
                SetCamera(playerPosition);

                gameActivity.cameraInitiallySet = true;
            }

            MoveMarker(playerPosition);
        }

        public void SetCamera(int playerPosition)
        {
            if (playerPosition == gameActivity.playerPosition)
            {
                Log.Debug("SetCamera", "Setting camera position");


                int[] builderSettingsInts;

                if (!gameActivity.cameraInitiallySet)
                {
                    builderSettingsInts = new int[] { 17, (int)gameActivity.azimuth, 21 };
                }
                else
                {
                    builderSettingsInts = new int[] { (int)gameActivity.googleMap.CameraPosition.Zoom, (int)gameActivity.azimuth, 21 };
                }

                CameraPosition cameraPosition = CameraBuilder(playerPosition, builderSettingsInts).Build();
                CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);

                gameActivity.googleMap.MoveCamera(cameraUpdate);
            }
        }

        private CameraPosition.Builder CameraBuilder(int playerPosition, int[] builderSettingsInts)
        {
            Log.Debug("CameraBuilder", "Building Camera");

            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();

            builder.Target(gameActivity.playerArray[playerPosition].currentPosition);
            builder.Zoom(builderSettingsInts[0]);
            builder.Bearing(builderSettingsInts[1]);
            builder.Tilt(builderSettingsInts[2]);

            return builder;
        }

        private void MoveMarker(int playerPosition)
        {
            gameActivity.playerArray[playerPosition].marker.Position = gameActivity.playerArray[playerPosition].currentPosition;
        }

        public void SetPolyline(int playerPosition, LinkedList<LatLng> vertices)
        {
            Log.Debug("SetPolyline", "Setting polyline positions");

            if (gameActivity.playerArray[playerPosition].drawingBool == false)
            {
                PolylineOptions polyline = new PolylineOptions();
                LinkedListNode<LatLng> verticesNode = vertices.First;

                while (true)
                {
                    polyline.Add(verticesNode.Value);

                    if (verticesNode.Next != null)
                    {
                        verticesNode = verticesNode.Next;
                    }
                    else
                    {
                        break;
                    }
                }

                BuildPolyline(playerPosition, polyline);
            }
            else
            {
                gameActivity.playerArray[playerPosition].polyline.Points = new List<LatLng>(vertices);
            }
        }

        private void BuildPolyline(int playerPosition, PolylineOptions polyline)
        {
            Log.Debug("BuildPolyline", "Building polyline");

            gameActivity.playerArray[playerPosition].polyline = gameActivity.googleMap.AddPolyline(polyline);
        }

        public void SetPolygon(LinkedList<LatLng> vertices)
        {
            Log.Debug("SetPolygon", "Setting polygon positions");

            PolygonOptions polygon = new PolygonOptions();

            LinkedListNode<LatLng> verticesNode = vertices.First;

            while (true)
            {
                polygon.Add(verticesNode.Value);

                if (verticesNode.Next != null)
                {
                    verticesNode = verticesNode.Next;
                }
                else
                {
                    break;
                }
            }

            BuildPolygon(polygon);
        }

        private void BuildPolygon(PolygonOptions polygon)
        {
            Log.Debug("BuildPolygon", "Building polygon");

            gameActivity.gamePlayArea.polygonsNode = gameActivity.gamePlayArea.polygons.AddLast(gameActivity.googleMap.AddPolygon(polygon));

            if (gameActivity.gamePlayArea.playAreaDrawnBool == false)
            {
                gameActivity.initialArea = (float)Static.Maths.PolygonArea(gameActivity.gamePlayArea.vertices);
                gameActivity.area = (int)((Static.Maths.PolygonArea(gameActivity.gamePlayArea.vertices) / gameActivity.initialArea) * 100);

                gameActivity.gamePlayArea.playAreaDrawnBool = true;
            }
        }

        public void SetCircle()
        {
            Log.Debug("SetCircle", "Setting circle positions");

            CircleOptions circleOptions = new CircleOptions();

            double radius = Static.Maths.ShortestLineSegment(gameActivity.gamePlayArea.vertices) * 5000;

            circleOptions.InvokeCenter(FindCirclePosition(gameActivity.gamePlayArea.vertices, radius));
            circleOptions.InvokeRadius(radius);

            BuildCircle(circleOptions);
        }

        public LatLng FindCirclePosition(LinkedList<LatLng> vertices, double radius)
        {
            LatLng position;
            LatLng[] interceptionVertex;

            while (true)
            {
                position = Static.Maths.FindRandomPoint(gameActivity);

                if (Static.Maths.PointInPolygon(vertices, position) && !CircleIntersectPolygon(vertices, position, radius, out interceptionVertex))
                {
                    break;
                }
            }

            return position;
        }

        public bool CircleIntersectPolygon(LinkedList<LatLng> vertices, LatLng position, double inRadius, out LatLng[] interceptionVertex)
        {
            double radius = 1d / (gameActivity.latLngToKM / (inRadius / 1000d));

            interceptionVertex = new LatLng[2];
            LinkedListNode<LatLng> verticesNode = vertices.First.Next;

            while (true)
            {
                if (Static.Maths.CircleLineIntersect(position, verticesNode.Previous.Value, verticesNode.Value, radius))
                {
                    interceptionVertex[0] = verticesNode.Previous.Value;
                    interceptionVertex[1] = verticesNode.Value;

                    return true;
                }

                if (verticesNode.Next != null)
                {
                    verticesNode = verticesNode.Next;
                }
                else
                {
                    if (Static.Maths.CircleLineIntersect(position, vertices.First.Value, vertices.Last.Value, radius))
                    {
                        interceptionVertex[0] = vertices.First.Value;
                        interceptionVertex[1] = vertices.Last.Value;

                        return true;
                    }

                    return false;
                }
            }
        }

        private void BuildCircle(CircleOptions circle)
        {
            Log.Debug("BuildCircle", "Building circle");

            gameActivity.circle = gameActivity.googleMap.AddCircle(circle);

            gameActivity.circle.FillColor = Color.HSVToColor(new float[] { BitmapDescriptorFactory.HueRed, 1.0f, 1.0f });
        }

        public bool UpdateRotation(int playerPosition, SensorEvent e)
        {
            float alpha = 0.97f;

            if (e.Sensor.Type == SensorType.Accelerometer)
            {
                gameActivity.gravity[0] = alpha * gameActivity.gravity[0] + (1 - alpha) * e.Values[0];
                gameActivity.gravity[1] = alpha * gameActivity.gravity[1] + (1 - alpha) * e.Values[1];
                gameActivity.gravity[2] = alpha * gameActivity.gravity[2] + (1 - alpha) * e.Values[2];
            }

            if (e.Sensor.Type == SensorType.MagneticField)
            {
                gameActivity.geoMagnetic[0] = alpha * gameActivity.geoMagnetic[0] + (1 - alpha) * e.Values[0];
                gameActivity.geoMagnetic[1] = alpha * gameActivity.geoMagnetic[1] + (1 - alpha) * e.Values[1];
                gameActivity.geoMagnetic[2] = alpha * gameActivity.geoMagnetic[2] + (1 - alpha) * e.Values[2];
            }

            if (gameActivity.gravity != null && gameActivity.geoMagnetic != null)
            {
                float[] R = new float[9];
                float[] I = new float[9];

                bool success = SensorManager.GetRotationMatrix(R, I, gameActivity.gravity, gameActivity.geoMagnetic);

                if (success)
                {
                    float[] orientation = new float[3];

                    SensorManager.GetOrientation(R, orientation);

                    gameActivity.azimuth = (float)(orientation[0] * (180 / Math.PI));
                    gameActivity.azimuth = (gameActivity.azimuth + 360) % 360;

                    SetCamera(playerPosition);

                    return success;
                }

                return success;
            }

            return false;
        }
    }
}