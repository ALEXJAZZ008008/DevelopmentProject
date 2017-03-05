using Android.Content;
using Android.Views;
using Android.Util;
using Android.Locations;
using Android.Gms.Maps.Model;
using Android.Gms.Maps;
using System.Collections.Generic;
using System;
using Android.Hardware;

namespace CaptureTheCampus
{
    public class Utilities : View
    {
        private GameActivity gameActivity;
        private Maths maths;

        public Utilities(Context context) : base(context)
        {
            Log.Info("Utilities", "Utilities built");

            gameActivity = (GameActivity)context;
            maths = new Maths(gameActivity);
        }

        public void BuildMap(GoogleMap googleMap, bool[] mapSettingsBools)
        {
            Log.Debug("BuildMap", "Building map");

            googleMap.MapType = GoogleMap.MapTypeTerrain;
            MapSettings(googleMap, mapSettingsBools);

            gameActivity.map = googleMap;
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

            gameActivity.markers.Add(gameActivity.map.AddMarker(BuildMarker()));
        }

        private MarkerOptions BuildMarker()
        {
            Log.Debug("BuildMarker", "Building marker");

            MarkerOptions markerOptions = new MarkerOptions();

            markerOptions.SetPosition(gameActivity.position);

            return markerOptions;
        }

        public void UpdateLocationInformation(Location location)
        {
            Log.Debug("Position", gameActivity.position.ToString());

            gameActivity.position = new LatLng(location.Latitude, location.Longitude);

            UpdateLocation();
            UpdatePaths();
        }

        private void UpdateLocation()
        {
            Log.Debug("LocationClient", "Location updated");

            SetCamera();
            MoveMarker();
        }

        private void SetCamera()
        {
            Log.Debug("SetCamera", "Setting camera position");

            int[] builderSettingsInts = { 0, (int)gameActivity.azimuth, 21 };

            CameraPosition cameraPosition = CameraBuilder(builderSettingsInts).Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);

            gameActivity.map.MoveCamera(cameraUpdate);
        }

        private CameraPosition.Builder CameraBuilder(int[] builderSettingsInts)
        {
            Log.Debug("CameraBuilder", "Building Camera");

            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();

            builder.Target(gameActivity.position);
            builder.Zoom(builderSettingsInts[0]);
            builder.Bearing(builderSettingsInts[1]);
            builder.Tilt(builderSettingsInts[2]);

            return builder;
        }

        private void MoveMarker()
        {
            gameActivity.markers[0].Position = gameActivity.position;
        }

        private void UpdatePaths()
        {
            if (maths.CheckPosition() && gameActivity.path.vertices.Count >= 1)
            {
                UpdatePath();
            }
            else
            {
                ResetPath();
            }
        }

        private void UpdatePath()
        {
            gameActivity.path.vertices.Add(gameActivity.position);

            if (gameActivity.path.drawing == false)
            {
                for (int i = 1; i < gameActivity.playArea.polygon[0].Points.Count; i++)
                {
                    if (maths.doIntersect(gameActivity.path.vertices[0], gameActivity.path.vertices[1], gameActivity.playArea.polygon[0].Points[i - 1], gameActivity.playArea.polygon[0].Points[i]))
                    {
                        gameActivity.path.vertices[0] = maths.LineIntersectionPoint(gameActivity.path.vertices[0], gameActivity.path.vertices[1], gameActivity.playArea.polygon[0].Points[i - 1], gameActivity.playArea.polygon[0].Points[i]);

                        break;
                    }
                }

                SetPolyline(gameActivity.path.vertices);

                gameActivity.path.drawing = true;
            }
            else
            {
                SetPolyline(gameActivity.path.vertices);
            }
        }

        private void ResetPath()
        {
            if (gameActivity.path.drawing == false)
            {
                gameActivity.path.vertices.Clear();

                gameActivity.path.vertices.Add(gameActivity.position);
            }
            else
            {
                gameActivity.path.vertices.Add(gameActivity.position);

                for (int i = 1; i < gameActivity.playArea.polygon[0].Points.Count; i++)
                {
                    if (maths.doIntersect(gameActivity.path.vertices[gameActivity.path.vertices.Count - 2], gameActivity.path.vertices[gameActivity.path.vertices.Count - 1], gameActivity.playArea.polygon[0].Points[i - 1], gameActivity.playArea.polygon[0].Points[i]))
                    {
                        gameActivity.path.vertices[gameActivity.path.vertices.Count - 1] = maths.LineIntersectionPoint(gameActivity.path.vertices[gameActivity.path.vertices.Count - 2], gameActivity.path.vertices[gameActivity.path.vertices.Count - 1], gameActivity.playArea.polygon[0].Points[i - 1], gameActivity.playArea.polygon[0].Points[i]);

                        break;
                    }
                }

                SetPolyline(gameActivity.path.vertices);

                BuildArea();

                gameActivity.path.polylines[gameActivity.path.polylines.Count - 1].Remove();

                gameActivity.path.drawing = false;

                gameActivity.path.vertices.Clear();

                gameActivity.path.vertices.Add(gameActivity.position);
            }
        }

        private void BuildArea()
        {
            List<int> intersections = new List<int>();

            LatLng firstExtendedPosition = maths.ExtendLineSegment(gameActivity.path.vertices[0], gameActivity.path.vertices[gameActivity.path.vertices.Count - 1]);
            LatLng secondExtendedPosition = maths.ExtendLineSegment(gameActivity.path.vertices[gameActivity.path.vertices.Count - 1], gameActivity.path.vertices[0]);

            int[] firstLineSegment = new int[2];
            int[] secondLineSegment = new int[2];

            for (int i = 1; i < gameActivity.playArea.polygon[0].Points.Count; i++)
            {
                if (maths.doIntersect(firstExtendedPosition, secondExtendedPosition, gameActivity.playArea.polygon[0].Points[i - 1], gameActivity.playArea.polygon[0].Points[i]))
                {
                    intersections.Add(i - 1);
                    intersections.Add(i);
                }
            }

            firstLineSegment[0] = intersections[0];
            firstLineSegment[1] = intersections[1];

            secondLineSegment[0] = intersections[intersections.Count - 2];
            secondLineSegment[1] = intersections[intersections.Count - 1];

            BuildAreas(firstLineSegment, secondLineSegment);
        }

        private void BuildAreas(int[] firstLineSegment, int[] secondLineSegment)
        {
            List<LatLng> firstPolygon = new List<LatLng>();
            List<LatLng> secondPolygon = new List<LatLng>();

            for (int i = 0; i < gameActivity.path.polylines[gameActivity.path.polylines.Count - 1].Points.Count; i++)
            {
                firstPolygon.Add(gameActivity.path.polylines[gameActivity.path.polylines.Count - 1].Points[i]);
                secondPolygon.Add(gameActivity.path.polylines[gameActivity.path.polylines.Count - 1].Points[i]);
            }

            for (int i = firstLineSegment[1]; i < secondLineSegment[0] + 1; i++)
            {
                firstPolygon.Add(gameActivity.playArea.polygon[0].Points[i]);
            }

            SetPolygon(gameActivity.playArea.polygon.Count, firstPolygon);

            if (firstLineSegment[0] == 0)
            {
                firstLineSegment[0] = gameActivity.playArea.polygon[0].Points.Count - 1;
            }

            if (secondLineSegment[1] == gameActivity.playArea.polygon[0].Points.Count - 1)
            {
                secondLineSegment[1] = 0;

                secondPolygon.Reverse();
            }

            for (int i = secondLineSegment[1]; i < firstLineSegment[0] + 1; i++)
            {
                secondPolygon.Add(gameActivity.playArea.polygon[0].Points[i]);
            }

            SetPolygon(gameActivity.playArea.polygon.Count, secondPolygon);

            TestAreas();
        }

        private void TestAreas()
        {
            double firstArea = maths.PolygonArea(gameActivity.playArea.polygon.Count - 2);
            double secondArea = maths.PolygonArea(gameActivity.playArea.polygon.Count - 1);

            if (secondArea <= firstArea)
            {
                AddSecondArea(firstArea, secondArea);
            }
            else
            {
                AddFirstArea(firstArea, secondArea);
            }
        }

        private void AddSecondArea(double firstArea, double secondArea)
        {
            gameActivity.playArea.polygon[0].Points = gameActivity.playArea.polygon[gameActivity.playArea.polygon.Count - 2].Points;

            gameActivity.playArea.polygon.Remove(gameActivity.playArea.polygon[gameActivity.playArea.polygon.Count - 2]);

            gameActivity.score += (int)((secondArea / (firstArea + secondArea)) * 100);
        }

        private void AddFirstArea(double firstArea, double secondArea)
        {
            gameActivity.playArea.polygon[0].Points = gameActivity.playArea.polygon[gameActivity.playArea.polygon.Count - 1].Points;

            gameActivity.playArea.polygon.Remove(gameActivity.playArea.polygon[gameActivity.playArea.polygon.Count - 1]);

            gameActivity.score += (int)((firstArea / (firstArea + secondArea)) * 100);
        }

        public void SetPolyline(List<LatLng> position)
        {
            Log.Debug("SetPolyline", "Setting polyline positions");

            if (gameActivity.path.drawing == false)
            {
                PolylineOptions polyline = new PolylineOptions();

                for (int i = 0; i < position.Count; i++)
                {
                    polyline.Add(position[i]);
                }

                BuildPolyline(polyline);
            }
            else
            {
                gameActivity.path.polylines[gameActivity.path.polylines.Count - 1].Points = position;
            }
        }

        private void BuildPolyline(PolylineOptions polyline)
        {
            Log.Debug("BuildPolyline", "Building polyline");

            gameActivity.path.polylines.Add(gameActivity.map.AddPolyline(polyline));
        }

        public void SetPolygon(int polygonNumber, List<LatLng> position)
        {
            Log.Debug("SetPolygon", "Setting polygon positions");

            if (gameActivity.playArea.polygon.Count < polygonNumber + 1)
            {
                PolygonOptions polygon = new PolygonOptions();

                for (int i = 0; i < position.Count; i++)
                {
                    polygon.Add(position[i]);
                }

                BuildPolygon(polygon);
            }
            else
            {
                gameActivity.playArea.polygon[polygonNumber].Points = position;
            }
        }

        private void BuildPolygon(PolygonOptions polygon)
        {
            Log.Debug("BuildPolygon", "Building polygon");

            gameActivity.playArea.polygon.Add(gameActivity.map.AddPolygon(polygon));
        }

        internal bool UpdateRotation(SensorEvent e)
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

                    SetCamera();

                    return success;
                }

                return success;
            }

            return false;
        }
    }
}