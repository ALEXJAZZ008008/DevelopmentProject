using Android.Gms.Maps.Model;
using System.Collections.Generic;

namespace CaptureTheCampus.Game
{
    public class Player
    {
        volatile public Marker marker;
        volatile public int score;
        volatile public LatLng currentPosition;
        volatile public LinkedList<LatLng> vertices;
        volatile public LinkedListNode<LatLng> verticesNode;
        volatile public Polyline polyline;
        volatile public bool drawingBool, positionBool, deathBool;
    }
}