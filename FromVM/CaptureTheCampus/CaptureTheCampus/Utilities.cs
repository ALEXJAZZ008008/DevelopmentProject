using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Hardware;
using Android.Locations;
using Android.Util;
using System;
using System.Collections.Generic;

namespace CaptureTheCampus
{
    public class Utilities : Activity
    {
        private GameActivity gameActivity;
        private Area area;
        private Maths maths;

        public Utilities(Context context)
        {
            Log.Info("Utilities", "Utilities built");

            gameActivity = (GameActivity)context;
            area = new Area(gameActivity, this);
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

            markerOptions.SetIcon(BitmapDescriptorFactory.DefaultMarker(Colour(gameActivity.markers.Count)));
            markerOptions.SetPosition(gameActivity.path.currentPosition);

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

        public void UpdateLocationInformation(Location location)
        {
            Log.Debug("Position", gameActivity.path.currentPosition.ToString());

            gameActivity.path.currentPosition = new LatLng(location.Latitude, location.Longitude);

            UpdateLocation();
            area.UpdatePaths();
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

            int[] builderSettingsInts = { 17, (int)gameActivity.azimuth, 21 };

            CameraPosition cameraPosition = CameraBuilder(builderSettingsInts).Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);

            gameActivity.map.MoveCamera(cameraUpdate);
        }

        private CameraPosition.Builder CameraBuilder(int[] builderSettingsInts)
        {
            Log.Debug("CameraBuilder", "Building Camera");

            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();

            builder.Target(gameActivity.path.currentPosition);
            builder.Zoom(builderSettingsInts[0]);
            builder.Bearing(builderSettingsInts[1]);
            builder.Tilt(builderSettingsInts[2]);

            return builder;
        }

        private void MoveMarker()
        {
            gameActivity.markers[gameActivity.markerNumber].Position = gameActivity.path.currentPosition;
        }

        public void SetPolyline(LinkedList<LatLng> vertices)
        {
            Log.Debug("SetPolyline", "Setting polyline positions");

            if (gameActivity.path.drawingBool == false)
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

                BuildPolyline(polyline);
            }
            else
            {
                gameActivity.path.polyline.Points = new List<LatLng>(vertices);
            }
        }

        private void BuildPolyline(PolylineOptions polyline)
        {
            Log.Debug("BuildPolyline", "Building polyline");

            gameActivity.path.polyline = gameActivity.map.AddPolyline(polyline);
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

            gameActivity.playArea.polygonsNode = gameActivity.playArea.polygons.AddLast(gameActivity.map.AddPolygon(polygon));

            if (gameActivity.playArea.playAreaDrawnBool == false)
            {
                gameActivity.initialArea = maths.PolygonArea(gameActivity.playArea.vertices);
                gameActivity.area = (int)((maths.PolygonArea(gameActivity.playArea.vertices) / gameActivity.initialArea) * 100);

                gameActivity.playArea.playAreaDrawnBool = true;
            }
        }

        public void SetCircle()
        {
            Log.Debug("SetCircle", "Setting circle positions");

            CircleOptions circleOptions = new CircleOptions();

            circleOptions.InvokeCenter(maths.FindCentroid());
            circleOptions.InvokeRadius(maths.ShortestLineSegment(gameActivity.playArea.vertices) * 5000);

            BuildCircle(circleOptions);
        }

        private void BuildCircle(CircleOptions circle)
        {
            Log.Debug("BuildCircle", "Building circle");

            gameActivity.circle = gameActivity.map.AddCircle(circle);
        }

        public bool UpdateRotation(SensorEvent e)
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