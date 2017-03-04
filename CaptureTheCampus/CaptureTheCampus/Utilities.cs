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

        public Utilities(Context context) : base(context)
        {
            Log.Info("Position", "Position built");

            gameActivity = (GameActivity)context;
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
            if (CheckPosition() && gameActivity.path.vertices.Count >= 1)
            {
                UpdatePath();
            }
            else
            {
                ResetPath();
            }
        }

        private bool CheckPosition()
        {
            // Get the angle between the point and the
            // first and last vertices.
            int max_point = gameActivity.playArea.polygon.Points.Count - 1;
            double total_angle = GetAngle(gameActivity.playArea.polygon.Points[max_point].Latitude, gameActivity.playArea.polygon.Points[max_point].Longitude, gameActivity.position.Latitude, gameActivity.position.Longitude, gameActivity.playArea.polygon.Points[0].Latitude, gameActivity.playArea.polygon.Points[0].Longitude);

            // Add the angles from the point
            // to each other pair of vertices.
            for (int i = 0; i < max_point; i++)
            {
                total_angle += GetAngle(gameActivity.playArea.polygon.Points[i].Latitude, gameActivity.playArea.polygon.Points[i].Longitude, gameActivity.position.Latitude, gameActivity.position.Longitude, gameActivity.playArea.polygon.Points[i + 1].Latitude, gameActivity.playArea.polygon.Points[i + 1].Longitude);
            }

            // The total angle should be 2 * PI or -2 * PI if
            // the point is in the polygon and close to zero
            // if the point is outside the polygon.
            return (Math.Abs(total_angle) > 0.000001);
        }

        private double GetAngle(double Ax, double Ay, double Bx, double By, double Cx, double Cy)
        {
            // Get the dot product.
            double dot_product = DotProduct(Ax, Ay, Bx, By, Cx, Cy);

            // Get the cross product.
            double cross_product = CrossProductLength(Ax, Ay, Bx, By, Cx, Cy);

            // Calculate the angle.
            return Math.Atan2(cross_product, dot_product);
        }

        private double DotProduct(double Ax, double Ay, double Bx, double By, double Cx, double Cy)
        {
            // Get the vectors' coordinates.
            double BAx = Ax - Bx;
            double BAy = Ay - By;
            double BCx = Cx - Bx;
            double BCy = Cy - By;

            // Calculate the dot product.
            return (BAx * BCx + BAy * BCy);
        }

        private double CrossProductLength(double Ax, double Ay, double Bx, double By, double Cx, double Cy)
        {
            // Get the vectors' coordinates.
            double BAx = Ax - Bx;
            double BAy = Ay - By;
            double BCx = Cx - Bx;
            double BCy = Cy - By;

            // Calculate the Z coordinate of the cross product.
            return (BAx * BCy - BAy * BCx);
        }

        private void UpdatePath()
        {
            gameActivity.path.vertices.Add(gameActivity.position);

            if (gameActivity.path.drawing == false)
            {
                for (int i = 1; i < gameActivity.playArea.polygon.Points.Count; i++)
                {
                    if (doIntersect(gameActivity.path.vertices[0], gameActivity.path.vertices[1], gameActivity.playArea.polygon.Points[i - 1], gameActivity.playArea.polygon.Points[i]))
                    {
                        gameActivity.path.vertices[0] = LineIntersectionPoint(gameActivity.path.vertices[0], gameActivity.path.vertices[1], gameActivity.playArea.polygon.Points[i - 1], gameActivity.playArea.polygon.Points[i]);
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

                for (int i = 1; i < gameActivity.playArea.polygon.Points.Count; i++)
                {
                    if (doIntersect(gameActivity.path.vertices[gameActivity.path.vertices.Count - 2], gameActivity.path.vertices[gameActivity.path.vertices.Count - 1], gameActivity.playArea.polygon.Points[i - 1], gameActivity.playArea.polygon.Points[i]))
                    {
                        gameActivity.path.vertices[gameActivity.path.vertices.Count - 1] = LineIntersectionPoint(gameActivity.path.vertices[gameActivity.path.vertices.Count - 2], gameActivity.path.vertices[gameActivity.path.vertices.Count - 1], gameActivity.playArea.polygon.Points[i - 1], gameActivity.playArea.polygon.Points[i]);
                    }
                }

                SetPolyline(gameActivity.path.vertices);

                gameActivity.path.drawing = false;

                gameActivity.path.vertices.Clear();

                gameActivity.path.vertices.Add(gameActivity.position);
            }
        }

        // The main function that returns true if line segment 'p1q1'
        // and 'p2q2' intersect.
        private bool doIntersect(LatLng p1, LatLng q1, LatLng p2, LatLng q2)
        {
            // Find the four orientations needed for general and
            // special cases
            int o1 = orientation(p1, q1, p2);
            int o2 = orientation(p1, q1, q2);
            int o3 = orientation(p2, q2, p1);
            int o4 = orientation(p2, q2, q1);

            // General case
            if (o1 != o2 && o3 != o4)
                return true;

            // Special Cases
            // p1, q1 and p2 are colinear and p2 lies on segment p1q1
            if (o1 == 0 && onSegment(p1, p2, q1)) return true;

            // p1, q1 and p2 are colinear and q2 lies on segment p1q1
            if (o2 == 0 && onSegment(p1, q2, q1)) return true;

            // p2, q2 and p1 are colinear and p1 lies on segment p2q2
            if (o3 == 0 && onSegment(p2, p1, q2)) return true;

            // p2, q2 and q1 are colinear and q1 lies on segment p2q2
            if (o4 == 0 && onSegment(p2, q1, q2)) return true;

            return false; // Doesn't fall in any of the above cases
        }

        // To find orientation of ordered triplet (p, q, r).
        // The function returns following values
        // 0 --> p, q and r are colinear
        // 1 --> Clockwise
        // 2 --> Counterclockwise
        private int orientation(LatLng p, LatLng q, LatLng r)
        {
            // See http://www.geeksforgeeks.org/orientation-3-ordered-points/
            // for details of below formula.
            double val = (q.Longitude - p.Longitude) * (r.Latitude - q.Latitude) - (q.Latitude - p.Latitude) * (r.Longitude - q.Longitude);

            if (val == 0)
            {
                return 0;  // colinear
            }

            return (val > 0) ? 1 : 2; // clock or counterclock wise
        }

        // Given three colinear points p, q, r, the function checks if
        // point q lies on line segment 'pr'
        private bool onSegment(LatLng p, LatLng q, LatLng r)
        {
            if (q.Latitude <= Math.Max(p.Latitude, r.Latitude) && q.Latitude >= Math.Min(p.Latitude, r.Latitude) && q.Longitude <= Math.Max(p.Longitude, r.Longitude) && q.Longitude >= Math.Min(p.Longitude, r.Longitude))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        LatLng LineIntersectionPoint(LatLng ps1, LatLng pe1, LatLng ps2, LatLng pe2)
        {
            // Get A,B,C of first line - points : ps1 to pe1
            double A1 = pe1.Longitude - ps1.Longitude;
            double B1 = ps1.Latitude - pe1.Latitude;
            double C1 = A1 * ps1.Latitude + B1 * ps1.Longitude;

            // Get A,B,C of second line - points : ps2 to pe2
            double A2 = pe2.Longitude - ps2.Longitude;
            double B2 = ps2.Latitude - pe2.Latitude;
            double C2 = A2 * ps2.Latitude + B2 * ps2.Longitude;

            // Get delta and check if the lines are parallel
            double delta = A1 * B2 - A2 * B1;

            if (delta == 0)
            {
                throw new Exception("Lines are parallel");
            }

            // now return the Vector2 intersection point
            return new LatLng((B2 * C1 - B1 * C2) / delta, (A1 * C2 - A2 * C1) / delta);
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

        public void SetPolygon(List<LatLng> position)
        {
            Log.Debug("SetPolygon", "Setting polygon positions");

            if (gameActivity.playArea.polygon == null)
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
                gameActivity.playArea.polygon.Points = position;
            }
        }

        private void BuildPolygon(PolygonOptions polygon)
        {
            Log.Debug("BuildPolygon", "Building polygon");

            gameActivity.playArea.polygon = gameActivity.map.AddPolygon(polygon);
        }

        internal bool UpdateRotation(SensorEvent e)
        {
            float alpha = 0.97f;

            if(e.Sensor.Type == SensorType.Accelerometer)
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