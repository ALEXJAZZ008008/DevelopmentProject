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

    class Update
    {
        //This creates a new instance of the Logging class
        Logging logging = new Logging();

        public void Initialisation(Socket socket)
        {
            //This opens a new network stream on the given socket
            NetworkStream networkStream = new NetworkStream(socket);

            //This acquires the IP address of the host
            string ipAddress = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();

            #region ToUpdateDictionary

            try
            {
                //This calls the update protocol method
                ToUpdateDictionary(networkStream, ipAddress);
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
                //This cleans up before exiting
                networkStream.Close();

                socket.Close();
            }

            #endregion
        }

        private void ToUpdateDictionary(NetworkStream networkStream, string ipAddress)
        {
            //These are the variables used to pass data too and from the server
            StreamWriter streamWriter = new StreamWriter(networkStream);
            StreamReader streamReader = new StreamReader(networkStream);

            //This ensures that if the client hangs for too long the server doens't wait for a responce
            networkStream.ReadTimeout = 3000;

            //This reads the next line from the client
            string nextLine = streamReader.ReadLine();

            //This ensures that if the client hangs for too long the server doens't wait for a responce
            networkStream.WriteTimeout = 3000;

            //This calls the update dictionary class
            UpdateDictionary(streamWriter, streamReader, nextLine, ipAddress);
        }

        private void UpdateDictionary(StreamWriter streamWriter, StreamReader streamReader, string nextLine, string ipAddress)
        {
            //These variables are used to hold output stings
            string output = string.Empty;
            string loggingOutput = string.Empty;

            //This splits the input into a array containing the username and location
            string[] nextLineSectionsWhoIs = nextLine.Split(new char[] { ' ' }, 2);

            #region Request

            //This checks to see if the array contains both a username and location
            if (nextLineSectionsWhoIs.Length == 1)
            {
                //This checks to see if the dictionary already containes an instance of the given username
                if (TCPServer.dictionary.ContainsKey(nextLine))
                {
                    //This adds the username to a string
                    string username = TCPServer.dictionary[nextLine];

                    //This adds the username to a string
                    output = username + "\r\n";
                    loggingOutput = nextLine + " " + username;
                }
                else
                {
                    //This outputs an error if the dictionary does not contain the username
                    output = "ERROR: no entries found" + "\r\n";
                    loggingOutput = "ERROR: no entries found";
                }
            }

            #region Update

            else
            {
                //This checks to see if the array contains both a username and location
                if (nextLineSectionsWhoIs.Length == 2)
                {
                    //This checks to see if the dictionary already containes an instance of the given username
                    if (TCPServer.dictionary.ContainsKey(nextLineSectionsWhoIs[0]))
                    {
                        //This adds the location to the dictionary
                        TCPServer.dictionary[nextLineSectionsWhoIs[0]] = nextLineSectionsWhoIs[1];
                    }
                    else
                    {
                        //This adds the username and location to the dictionary
                        TCPServer.dictionary.Add(nextLineSectionsWhoIs[0], nextLineSectionsWhoIs[1]);
                    }

                    //This is an output to a string
                    output = "OK" + "\r\n";
                    loggingOutput = nextLineSectionsWhoIs[0] + " " + nextLineSectionsWhoIs[1];
                }
                else
                {
                    output = "ERROR: arguments invalid" + "\r\n";
                    loggingOutput = "ERROR: arguments invalid" + "\r\n";
                }
            }

            #endregion

            #endregion

            streamWriter.WriteLine(output);
            streamWriter.Close();

            //This runs the logging scripts
            logging.ToLog(loggingOutput, ipAddress);
        }
    }

    class Logging
    {
        //This allows external access to the private log method
        public void ToLog(string loggingOutput, string ipAddress)
        {
            //This locks this part of the code univerally
            TCPServer.loggingMessageMutex.WaitOne();

            //This adds the correct log message to a string
            TCPServer.logMessage = TCPServer.logMessage + ipAddress + " - - " + DateTime.Now.ToString("'['dd'/'MM'/'yyyy':'HH':'mm':'ss zz00']'") + " \"" + loggingOutput + "\"" + "\r\n";

            //This allowes another thread to enter the previous code
            TCPServer.loggingMessageMutex.ReleaseMutex();

            if (DateTime.Now > TCPServer.dateTime)
            {
                //This locks this part of the code univerally
                TCPServer.loggingMutex.WaitOne();

                try
                {
                    Log(loggingOutput, ipAddress);
                }
                catch
                {
                    TCPServer.loggingMutex.ReleaseMutex();
                }

                TCPServer.dateTime = DateTime.Now + TimeSpan.FromMilliseconds(1000);

                //This allowes another thread to enter the previous code
                TCPServer.loggingMutex.ReleaseMutex();
            }
        }

        private void Log(string loggingOutput, string ipAddress)
        {
            //This creates if needed a file that is then written to with the log message
            StreamWriter streamWriter = File.AppendText(TCPServer.logPath);

            //This writes the message
            streamWriter.Write(TCPServer.logMessage);
            streamWriter.Close();

#if DEBUG
            //This outputs the log message to the screen
            Console.Write(TCPServer.logMessage);
#endif

            TCPServer.logMessage = string.Empty;
        }
    }
}