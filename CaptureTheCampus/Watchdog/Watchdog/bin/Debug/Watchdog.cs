using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Diagnostics;
using System.ComponentModel;

namespace Watchdog
{
    class Watchdog
    {
        public static void Main()
        {
            Client client = new Client();
            Process UDPServer = new Process();
            Process TCPServer = new Process();

            int ERROR_FILE_NOT_FOUND = 2;
            int ERROR_ACCESS_DENIED = 5;

            try
            {
                // Get the path that stores user documents.
                string myDocumentsPath = Directory.GetCurrentDirectory();

#if DEBUG
                string[] myDocumentsPathSplit = Regex.Split(myDocumentsPath, @"Watchdog\\Watchdog\\bin\\Debug");

                UDPServer.StartInfo.FileName = myDocumentsPathSplit[0] + @"UDPServer\\UDPServer\\bin\\Debug\\UDPServer.exe";
                TCPServer.StartInfo.FileName = myDocumentsPathSplit[0] + @"TCPServer\\TCPServer\\bin\\Debug\\TCPServer.exe";
#else
                string[] myDocumentsPathSplit = Regex.Split(myDocumentsPath, @"Watchdog\\Watchdog\\bin\\Release");
                
                UDPServer.StartInfo.FileName = myDocumentsPathSplit[0] + @"UDPServer\\UDPServer\\bin\\Release\\UDPServer.exe";
                TCPServer.StartInfo.FileName = myDocumentsPathSplit[0] + @"TCPServer\\TCPServer\\bin\\Release\\TCPServer.exe";
#endif

                UDPServer.Start();
                TCPServer.Start();
            }
            catch (Win32Exception e)
            {
                if (e.NativeErrorCode == ERROR_FILE_NOT_FOUND)
                {
                    Console.WriteLine(e.Message + ". Check the path.");
                }
                else
                {
                    if (e.NativeErrorCode == ERROR_ACCESS_DENIED)
                    {
                        Console.WriteLine(e.Message + ". You do not have permission to access this file.");
                    }
                }
            }

            client.Input(new string[] { "dateTime", (DateTime.Now + TimeSpan.FromMilliseconds(10000)).ToString() });

            DateTime serverDateTime = DateTime.Parse(Regex.Split(client.Input(new string[] { "dateTime" }), ": ")[1]);

            while (serverDateTime >= DateTime.Now)
            {
                Thread.Sleep(3000);

                try
                {
                    serverDateTime = DateTime.Parse(Regex.Split(client.Input(new string[] { "dateTime" }), ": ")[1]);
                }
                catch
                {

                }
            }

            UDPServer.Kill();
            TCPServer.Kill();
        }
    }
}
