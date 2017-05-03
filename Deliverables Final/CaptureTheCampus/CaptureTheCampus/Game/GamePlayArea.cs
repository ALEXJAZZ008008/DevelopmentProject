using Android.Gms.Maps.Model;
using System.Collections.Generic;

namespace CaptureTheCampus.Game
{
    public class GamePlayArea
    {
        volatile public LinkedList<LatLng> vertices;
        volatile public LinkedListNode<LatLng> verticesNode;
        volatile public LinkedList<Polygon> polygons;
        volatile public LinkedListNode<Polygon> polygonsNode;
        volatile public bool playAreaDrawnBool;
    }
}