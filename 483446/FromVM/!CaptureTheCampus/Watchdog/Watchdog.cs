using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Diagnostics;
using System.ComponentModel;

namespace Watchdog
{
    public class Watchdog
    {
        public void Output(bool Standalone)
        {
            Process TCPServer = new Process();

            int ERROR_FILE_NOT_FOUND = 2;
            int ERROR_ACCESS_DENIED = 5;

            try
            {
                // Get the path that stores user documents.
                string myDocumentsPath = Directory.GetCurrentDirectory();

                if (Standalone)
                {
#if DEBUG
                    string[] myDocumentsPathSplit = Regex.Split(myDocumentsPath, @"Watchdog\\bin\\Debug");

                    TCPServer.StartInfo.FileName = myDocumentsPathSplit[0] + @"TCPServer\\bin\\Debug\\TCPServer.exe";
#else
                    string[] myDocumentsPathSplit = Regex.Split(myDocumentsPath, @"Watchdog\\bin\\Release");
                    
                    TCPServer.StartInfo.FileName = myDocumentsPathSplit[0] + @"TCPServer\\bin\\Release\\TCPServer.exe";
#endif
                }
                else
                {
                    
                }

                TCPServer.Start();
            }
            catch (Win32Exception e)
            {
                if (e.NativeErrorCode == ERROR_FILE_NOT_FOUND)
                {
#if DEBUG
                    Console.WriteLine(e.Message + ". Check the path.");
#endif
                }
                else
                {
                    if (e.NativeErrorCode == ERROR_ACCESS_DENIED)
                    {
#if DEBUF
                        Console.WriteLine(e.Message + ". You do not have permission to access this file.");
#endif
                    }
                }
            }

            Input();

            TCPServer.Kill();
        }

        private static void Input()
        {
            Client.Client client = new Client.Client();

            client.Input(new string[] { "-t", "dateTime", (DateTime.Now + TimeSpan.FromMilliseconds(10000)).ToString() });

            DateTime serverDateTime = DateTime.Parse(Regex.Split(client.Input(new string[] { "-t", "dateTime" }), ": ")[1]);

            while (serverDateTime >= DateTime.Now)
            {
                Thread.Sleep(3000);

                try
                {
                    serverDateTime = DateTime.Parse(Regex.Split(client.Input(new string[] { "-t", "dateTime" }), ": ")[1]);
                }
                catch
                {

                }
            }
        }
    }
}