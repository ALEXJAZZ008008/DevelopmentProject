using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TCPServer
{
    class TCPServer
    {
        //This is the dictionary used to store the usernames and associated locations
        public static Dictionary<string, string> dictionary = new Dictionary<string, string>();

        //This is the default file path to the server backup and log
        public static string logPath = "serverLog";
        public static string logPathDirectory = Directory.GetCurrentDirectory() + @"\" + logPath + @"Directory\";

        public static DateTime dateTime;
        public static Mutex loggingMutex = new Mutex();
        public static Mutex loggingMessageMutex = new Mutex();
        public static string logMessage = string.Empty;

        private static void Main()
        {
            //This initializes files for backups etc
            OnServerStartup();

            Loop();
        }

        private static void OnServerStartup()
        {
            dateTime = DateTime.MinValue;

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
                    StreamReader streamReader = new StreamReader(logPathDirectory + logPath + " " + currentDirectoryDateTime[currentDirectoryDateTime.Length - 1] + ".txt");

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

                    //This closes the stream reader
                    streamReader.Close();

                    for (int i = 0; i < dictionaryItems.Count; i += 2)
                    {
                        //This addes all the necessary information to the dictionary
                        dictionary.Add(dictionaryItems[i], dictionaryItems[i + 1]);
                    }
                }
            }

            //This creates the dictionary backup file if it does not exist
            logPath = logPathDirectory + logPath + " " + DateTime.Now.Ticks.ToString() + ".txt";
            File.Create(logPath).Close();
        }

        private static void Loop()
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
                    //This creates new sockets
                    Socket socket = listener.AcceptSocket();

                    //This calls the thread method
                    Threads(socket);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                //This prints to the screen an error message
                Console.WriteLine("ERROR: " + ex.ToString());
#endif

                try
                {
                    loggingMutex.ReleaseMutex();
                    loggingMessageMutex.ReleaseMutex();
                }
                catch
                {

                }
            }

            #endregion
        }

        private static void Threads(Socket socket)
        {
            //This initializes a new instance of the Update class
            Update update = new Update();

            //This initializes a new thread with a max memory size of 250Kb and a loction to work of Initialisation in the Update class
            Thread thread = new Thread(() => update.Initialisation(socket), 1);

            //This starts a new thread
            thread.Start();
        }
    }
}