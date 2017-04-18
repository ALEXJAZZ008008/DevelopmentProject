using Android.Gms.Maps.Model;
using System.Collections.Generic;
using System.Linq;

namespace CaptureTheCampus
{
    public static class Serialise
    {
        public static string SerialiseLatLngLinkedList(LinkedList<LatLng> linkedList)
        {
            string outString = string.Empty;

            for(int i = 0; i < linkedList.Count; i++)
            {
                outString = SerialiseString(SerialiseLatLng(linkedList.ElementAt(i))) + outString;
            }

            return outString;
        }

        public static string SerialiseLatLng(LatLng latLng)
        {
            return SerialiseString(latLng.Latitude.ToString()) + latLng.Longitude.ToString();
        }

        public static string SerialiseString(string inString)
        {
            return inString + ",";
        }

        public static LinkedList<LatLng> DeserialiseLatLngLinkedList(string inString)
        {
            LinkedList<LatLng> linkedList = new LinkedList<LatLng>();

            while(inString != string.Empty)
            {
                linkedList.AddLast(DeserialiseLatLng(inString, out inString));
            }

            return linkedList;
        }

        public static LatLng DeserialiseLatLng(string inString, out string outString)
        {
            LatLng latLng = new LatLng(double.Parse(DeserialiseString(inString, out inString)), double.Parse(DeserialiseString(inString, out inString)));

            outString = inString;

            return latLng;
        }

        public static string DeserialiseString(string inString, out string outString)
        {
            string[] stringArray = inString.Split(new char[] { ',' }, 2);

            outString = stringArray[1];

            return stringArray[0];
        }
    }
}