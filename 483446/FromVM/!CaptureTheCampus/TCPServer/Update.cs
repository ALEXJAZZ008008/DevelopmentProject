using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace TCPServer
{
    class Update
    {
        public void Initialisation(Socket socket)
        {
            //This opens a new network stream on the given socket
            using (NetworkStream networkStream = new NetworkStream(socket))
            {
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
                    networkStream.Flush();

                    socket.Close();
                }
            }

            #endregion
        }

        private void ToUpdateDictionary(NetworkStream networkStream, string ipAddress)
        {
            //These are the variables used to pass data too and from the server
            using (StreamWriter streamWriter = new StreamWriter(networkStream))
            using (StreamReader streamReader = new StreamReader(networkStream))
            {
                //This ensures that if the client hangs for too long the server doens't wait for a responce
                networkStream.ReadTimeout = 3000;

                //This reads the next line from the client
                string nextLine = streamReader.ReadLine();

                //This ensures that if the client hangs for too long the server doens't wait for a responce
                networkStream.WriteTimeout = 3000;

                //This calls the update dictionary class
                UpdateDictionary(streamWriter, streamReader, nextLine, ipAddress);

                streamWriter.Flush();
            }
        }

        private void UpdateDictionary(StreamWriter streamWriter, StreamReader streamReader, string nextLine, string ipAddress)
        {
            //These variables are used to hold output stings
            string output = string.Empty;
            string loggingOutput = string.Empty;

            //This splits the input into a array containing the username and location
            string[] nextLineSections = nextLine.Split(new char[] { ' ' }, 2);

            #region Request

            //This checks to see if the array contains both a username and location
            if (nextLineSections.Length == 1)
            {
                //This checks to see if the dictionary already containes an instance of the given username
                if (TCPServer.serverDictionary.ContainsKey(nextLine))
                {
                    //This adds the username to a string
                    string username = TCPServer.serverDictionary[nextLine];

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
                if (nextLineSections.Length == 2)
                {
                    //This checks to see if the dictionary already containes an instance of the given username
                    if (TCPServer.serverDictionary.ContainsKey(nextLineSections[0]))
                    {
                        //This adds the location to the dictionary
                        TCPServer.serverDictionary[nextLineSections[0]] = nextLineSections[1];
                    }
                    else
                    {
                        //This adds the username and location to the dictionary
                        TCPServer.serverDictionary.TryAdd(nextLineSections[0], nextLineSections[1]);
                    }

                    //This is an output to a string
                    output = "OK" + "\r\n";
                    loggingOutput = nextLineSections[0] + " " + nextLineSections[1];
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

            Logging logging = new Logging();

            logging.ToLog(loggingOutput, ipAddress);
        }
    }
}