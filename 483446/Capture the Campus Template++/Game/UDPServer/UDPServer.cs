using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace UDPServer
{
    public class UDPServer
    {
        public static void Main()
        {
            const int broadcastPort = 11000;

            UdpClient broadcaster = new UdpClient();
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Broadcast, broadcastPort);

            try
            {
                while (true)
                {
                    byte[] bytes = Encoding.ASCII.GetBytes("Capture the Campus!");

#if DEBUG
                    Console.WriteLine("Sending broadcast to " + groupEP.ToString() + ": " + Encoding.ASCII.GetString(bytes, 0, bytes.Length));
#endif

                    broadcaster.Send(bytes, bytes.Length, groupEP);

                    Thread.Sleep(1000);
                }

            }
            catch (Exception e)
            {
#if DEBUG
                Console.WriteLine("ERROR: " + e.ToString());
#endif
            }
            finally
            {
                broadcaster.Close();
            }
        }
    }
}