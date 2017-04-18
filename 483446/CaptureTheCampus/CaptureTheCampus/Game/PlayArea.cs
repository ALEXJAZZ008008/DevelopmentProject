using Android.Gms.Maps.Model;
using System.Collections.Generic;

namespace CaptureTheCampus
{
    public class PlayArea
    {
        volatile public LinkedList<LatLng> vertices;
        public LinkedListNode<LatLng> verticesNode;
        public LinkedList<Polygon> polygons;
        public LinkedListNode<Polygon> polygonsNode;
        public bool playAreaDrawnBool;
    }
}