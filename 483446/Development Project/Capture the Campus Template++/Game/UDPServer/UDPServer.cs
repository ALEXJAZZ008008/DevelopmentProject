using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UDPServer
{
    public class UDPServer
    {
        public static void Main()
        {
            const int listenPort = 11000;

            UdpClient listener = new UdpClient(listenPort);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);

            try
            {
                while (true)
                {
                    Console.WriteLine("Waiting for broadcast");

                    byte[] bytes = listener.Receive(ref groupEP);

                    Console.WriteLine("Received broadcast from " + groupEP.ToString() + ": " + Encoding.ASCII.GetString(bytes, 0, bytes.Length) + "\r\n");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: " + e.ToString());
            }
            finally
            {
                listener.Close();
            }
        }
    }
}