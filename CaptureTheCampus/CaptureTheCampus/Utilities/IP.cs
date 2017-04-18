using System.Net;

namespace CaptureTheCampus
{
    public static class IP
    {
        public static string GetIP()
        {
            //Getting the IP Address of the device from Android.
            IPAddress[] addresses = Dns.GetHostAddresses(Dns.GetHostName());

            if (addresses != null && addresses[0] != null)
            {
                return addresses[0].ToString();
            }

            return "";
        }
    }
}