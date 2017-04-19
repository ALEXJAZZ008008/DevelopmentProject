using Android.Gms.Maps.Model;
using System.Collections.Generic;

namespace CaptureTheCampus.Game
{
    public class GamePlayArea
    {
        volatile public LinkedList<LatLng> vertices;
        public LinkedListNode<LatLng> verticesNode;
        public LinkedList<Polygon> polygons;
        public LinkedListNode<Polygon> polygonsNode;
        public bool playAreaDrawnBool;
    }
}