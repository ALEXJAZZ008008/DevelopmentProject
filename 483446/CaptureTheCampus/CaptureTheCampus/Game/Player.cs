using Android.Gms.Maps.Model;
using System.Collections.Generic;

namespace CaptureTheCampus.Game
{
    public class Player
    {
        public Marker marker;
        public int score;
        public LatLng currentPosition;
        public LinkedList<LatLng> vertices;
        public LinkedListNode<LatLng> verticesNode;
        public Polyline polyline;
        public bool drawingBool, positionBool, deathBool;
    }
}