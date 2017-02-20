using System;
using System.IO;
using System.Net.Sockets;

namespace Client
{
    class ToTCPServer
    {
        public string Input(string[] args, ref int noOfArgs)
        {
            //These variables are used to store messages to be sent to the server and screen
            string toInput = string.Empty;
            string toOutput = string.Empty;

            //This variable dictates if the output from the server needs to be read or not
            bool serverOutput = false;

            //If the number of arguments unaccounted for is one then there must only be arguments for a user name
            if (args.Length - noOfArgs == 1)
            {
                //This indicates that the server will return something of value
                serverOutput = true;

                //This addes things to be outputted to the screen
                toOutput = Client.username + ": ";

                //This sets the string to query the server
                toInput = Client.username;
            }

            //If the number of arguments unaccounted for is one then there must be arguments for a user name and a location
            else
            {
                if (args.Length - noOfArgs == 2)
                {
                    //This addes things to be outputted to the screen
                    toOutput = Client.username + ": " + Client.location;

                    //This sets the string to query the server
                    toInput = Client.username + " " + Client.location;
                }
                else
                {
                    toOutput = "ERROR: arguments invalid";
                }
            }

            try
            {
                if (toInput != string.Empty)
                {
                    #region Client

                    //This initializes a new Tcp client to handle the requests to the server
                    TcpClient client = new TcpClient();

                    client.Connect(Client.ipAddress, Client.port);

                    //These are the variables used to pass data too and from the server
                    StreamWriter streamWriter = new StreamWriter(client.GetStream());
                    StreamReader streamReader = new StreamReader(client.GetStream());

                    //This ensures that if the server hangs for too long the client doesn't wait for a response
                    client.SendTimeout = 3000;

                    //This sends the queries to the database
                    streamWriter.WriteLine(toInput);
                    streamWriter.Flush();

                    //This ensures that if the server hangs for too long the client doesn't wait for a response
                    client.ReceiveTimeout = 3000;

                    #endregion

                    #region ToScreenOutput

                    //This will skip the response code if the response from the server is not needed
                    if (serverOutput)
                    {
                        //This continues while there are lines to read
                        while ((client.Connected) && (!streamReader.EndOfStream) && (streamReader.Peek() != -1))
                        {
                            //This reads the next line from the server
                            string nextLine = streamReader.ReadLine();

                            //This adds it to the variable to be printed to the screen
                            toOutput = toOutput + nextLine;

                            //This is used to ensure a stack overflow is not caused
                            if (nextLine == null)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        //This ensures the client doesn't crash when the server is trying to force a response that is unneeded
                        while ((client.Connected) && (!streamReader.EndOfStream) && (streamReader.Peek() != -1))
                        {
                            streamReader.ReadLine();
                        }
                    }

                    #endregion
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
