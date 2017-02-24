using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace TCPServer
{
    public class TCPServer
    {
        //This is the dictionary used to store the usernames and associated locations
        public static ConcurrentDictionary<string, string> serverDictionary = new ConcurrentDictionary<string, string>();
        private static Dictionary<string, Task> taskDictionary = new Dictionary<string, Task>();

        //This is the default file path to the server backup and log
        private static string logPath = "serverLog";
        private static string logPathDirectory = Directory.GetCurrentDirectory() + @"\" + logPath + @"Directory\";
        public static TextWriter textWriter = TextWriter.Synchronized(File.AppendText(TCPServer.logPath));

        public void Input(CancellationToken ct)
        {
            //This initializes files for backups etc
            OnServerStartup();

            Loop(ct);
        }

        private static void OnServerStartup()
        {
            DateTime dateTime = DateTime.MinValue;

            if (!Directory.Exists(logPathDirectory))
            {
                Directory.CreateDirectory(logPathDirectory);
            }
            else
            {
                string[] currentDirectory = Directory.GetFiles(logPathDirectory);

                //If the dictionary backup file exists it is loaded here
                if (currentDirectory != null && currentDirectory.Length != 0)
                {
                    long[] currentDirectoryDateTime = new long[currentDirectory.Length];

                    for (int i = 0; i < currentDirectory.Length; i++)
                    {
                        currentDirectory[i] = currentDirectory[i].Substring(currentDirectory[i].IndexOf(logPath) + logPath.Length);
                        currentDirectory[i] = currentDirectory[i].Substring(currentDirectory[i].IndexOf(logPath) + logPath.Length + 1).Split('.')[0];

                        currentDirectoryDateTime[i] = long.Parse(currentDirectory[i]);
                    }

                    Array.Sort(currentDirectoryDateTime);

                    //This initializes a new reader for the dictionary backup file
                    using (StreamReader streamReader = new StreamReader(logPathDirectory + logPath + " " + currentDirectoryDateTime[currentDirectoryDateTime.Length - 1] + ".txt"))
                    {

                        List<string> dictionaryItems = new List<string>();

                        //This continues while there are lines to read
                        while ((!streamReader.EndOfStream) && (streamReader.Peek() != -1))
                        {
                            //This reads the next line from the dictionary backup file
                            string nextLine = streamReader.ReadLine();

                            //This splits the username from the location
                            nextLine = nextLine.Substring(nextLine.IndexOf('"') + 1);

                            string[] toDictionary = Regex.Split(nextLine, "\"");
                            toDictionary[1] = toDictionary[0].Substring(toDictionary[0].IndexOf(' ') + 1);
                            toDictionary[0] = toDictionary[0].Split(' ')[0];

                            if (toDictionary[0] == "ERROR:")
                            {
                                continue;
                            }

                            if (toDictionary[0] != toDictionary[1])
                            {
                                bool inDictionary = false;

                                if (dictionaryItems.Count != 0)
                                {
                                    for (int i = 0; i < dictionaryItems.Count; i += 2)
                                    {
                                        if (dictionaryItems[i] == toDictionary[0])
                                        {
                                            inDictionary = true;

                                            dictionaryItems[i + 1] = toDictionary[1];

                                            break;
                                        }
                                    }
                                }

                                if (!inDictionary)
                                {
                                    dictionaryItems.Add(toDictionary[0]);
                                    dictionaryItems.Add(toDictionary[1]);
                                }
                            }
                        }

                        for (int i = 0; i < dictionaryItems.Count; i += 2)
                        {
                            //This addes all the necessary information to the dictionary
                            serverDictionary.TryAdd(dictionaryItems[i], dictionaryItems[i + 1]);
                        }
                    }
                }
            }

            //This creates the dictionary backup file if it does not exist
            logPath = logPathDirectory + logPath + " " + DateTime.Now.Ticks.ToString() + ".txt";
            File.Create(logPath).Close();
        }

        private static void Loop(CancellationToken ct)
        {
            //This starts a listener
            TcpListener listener = new TcpListener(IPAddress.Any, 43);
            listener.Start();

            #region To Threads

            try
            {
                //This is the main loop of the program
                while (true)
                {
                    // Poll on this property if you have to do
                    // other cleanup before throwing.
                    if (ct.IsCancellationRequested)
                    {
                        // Clean up here, then...
                        ct.ThrowIfCancellationRequested();
                    }

                    //This creates new sockets
                    Socket socket = listener.AcceptSocket();

                    //This calls the thread method
                    Tasks(socket);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                //This prints to the screen an error message
                Console.WriteLine("ERROR: " + ex.ToString());
#endif
            }

            #endregion
        }

        private static void Tasks(Socket socket)
        {
            //This initializes a new instance of the Update class
            Update update = new Update();

            string taskName = "Task" + taskDictionary.Count;

            Task task = new Task(() => update.Initialisation(socket));

            taskDictionary.Add(taskName, task);

            task.Start();
        }
    }
}