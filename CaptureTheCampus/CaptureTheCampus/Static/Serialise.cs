using Android.Gms.Maps.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CaptureTheCampus.Static
{
    public static class Serialise
    {
        public static string SerialiseLatLngLinkedList(LinkedList<LatLng> linkedList)
        {
            string outString = string.Empty;

            for(int i = 0; i < linkedList.Count; i++)
            {
                outString += SerialiseString(SerialiseLatLng(linkedList.ElementAt(i)));
            }

            return outString.TrimEnd(',');
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
            try
            {
            LinkedList<LatLng> linkedList = new LinkedList<LatLng>();

            while(inString != string.Empty)
            {
                linkedList.AddLast(DeserialiseLatLng(inString, out inString));
            }

            return linkedList;
        }
            catch (Exception ex)
            {
#if DEBUG
                //This prints to the screen an error message
                Console.WriteLine("ERROR: " + ex.ToString());
#endif

                //This prints to the screen an error message
                return new LinkedList<LatLng>();
            }
}

        public static LatLng DeserialiseLatLng(string inString, out string outString)
        {
            try
            {
                LatLng latLng = new LatLng(double.Parse(DeserialiseString(inString, out inString)), double.Parse(DeserialiseString(inString, out inString)));

                outString = inString;

                return latLng;
            }
            catch (Exception ex)
            {
                outString = "ERROR: " + ex.ToString();

#if DEBUG
                //This prints to the screen an error message
                Console.WriteLine("ERROR: " + ex.ToString());
#endif

                //This prints to the screen an error message
                return new LatLng(0, 0);
            }
}

        public static string DeserialiseString(string inString, out string outString)
        {
            try
            {
                string[] stringArray = inString.Split(new char[] { ',' }, 2);

                if (stringArray.Length > 1)
                {
                    outString = stringArray[1];
                }
                else
                {
                    outString = string.Empty;
                }

                return stringArray[0];
            }
            catch (Exception ex)
            {
                outString = "ERROR: " + ex.ToString();

#if DEBUG
                //This prints to the screen an error message
                Console.WriteLine("ERROR: " + ex.ToString());
#endif

                //This prints to the screen an error message
                return outString;
            }
        }
    }
}