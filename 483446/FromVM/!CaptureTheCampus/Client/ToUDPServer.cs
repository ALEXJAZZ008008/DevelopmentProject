using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    class ToUDPServer
    {
        public string Input()
        {
            const int listnerPort = 11000;
            string toOutput = "ERROR: incorrect key";
            string received = string.Empty;

            try
            {
                using (UdpClient listener = new UdpClient(listnerPort))
                {
                    IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listnerPort);

#if DEBUG
                    Console.Write("Waiting for broadcast... ");
#endif

                    listener.Client.ReceiveTimeout = 3000;
                    byte[] bytes = listener.Receive(ref groupEP);
                    received = Encoding.ASCII.GetString(bytes, 0, bytes.Length);

#if DEBUG
                    Console.Write("Received broadcast from " + groupEP.ToString().Split(':')[0] + ": " + received + ": ");
#endif

                    if (received == "Capture the Campus!")
                    {
                        toOutput = groupEP.ToString().Split(':')[0];
                    }
                }
            }
            catch (Exception ex)
            {
                //This prints to the screen an error message
                toOutput = "ERROR: " + ex.ToString();
            }

            return toOutput;
        }
    }
}
