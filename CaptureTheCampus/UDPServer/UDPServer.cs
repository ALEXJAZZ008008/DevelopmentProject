using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace UDPServer
{
    public class UDPServer
    {
        public void Input(CancellationToken cancellationToken)
        {
            const int broadcastPort = 1026;

            while (true)
            {
                // Poll on this property if you have to do
                // other cleanup before throwing.
                if (cancellationToken.IsCancellationRequested)
                {
                    // Clean up here, then...
                    cancellationToken.ThrowIfCancellationRequested();
                }

                UdpClient broadcaster = new UdpClient();
                IPEndPoint groupEP = new IPEndPoint(IPAddress.Broadcast, broadcastPort);

                try
                {
                    byte[] bytes = Encoding.ASCII.GetBytes("Capture the Campus!");

#if DEBUG
                    Console.WriteLine("Sending broadcast to " + groupEP.ToString() + ": " + Encoding.ASCII.GetString(bytes, 0, bytes.Length));
#endif

                    broadcaster.Send(bytes, bytes.Length, groupEP);
                }
                catch (Exception ex)
                {
#if DEBUG
                    //This prints to the screen an error message
                    Console.WriteLine("ERROR: " + ex.ToString());
#endif
                }
                finally
                {
                    broadcaster.Close();
                }


                Thread.Sleep(1000);
            }
        }
    }
}