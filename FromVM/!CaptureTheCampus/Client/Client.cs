using System;

namespace Client
{
    public class Client
    {
        //This pair of variables holds the IP address of the server and the port of the server globally for easy access
        public static string ipAddress = "localhost";
        public static int port = 43;

        //These are used to store the variables that will bypassed from the client to the server
        public static string username;
        public static string location;

        public string Input(string[] args)
        {
            //These are used to store variables used to verify the arguments given to the client
            int noOfArgs = 0;
            bool argBool = false;
            char serverType = 'N';

            //This calls the method used to verify the arguments given
            FromUser(args, ref noOfArgs, ref argBool, ref serverType);

            //If the arguments given are valid this calls the method which updates the server
            if (argBool && serverType != 'N')
            {
                if (serverType == 'u')
                {
                    ToUDPServer toUDPServer = new ToUDPServer();

                    return toUDPServer.Input();
                }
                else
                {
                    ToTCPServer toTCPServer = new ToTCPServer();

                    return toTCPServer.Input(args, ref noOfArgs);
                }
            }
            else
            {
                return "ERROR: Invalid Arguments.";
            }
        }

        private static void FromUser(string[] args, ref int noOfArgs, ref bool argBool, ref char serverType)
        {
            //If there are no arguments the program doesn't bother to execute any further code
            if (args.Length != 0)
            {
                //These are used to track the progress through the assignment process and to pass the correct arguments to the correct variables
                int i = 0;
                bool usernameBool = false;
                bool locationBool = false;

                //This is used to exit the program if the arguments given are invalid
                argBool = true;

                //This ensures the correct number of arguments are initialized
                while (i < args.Length)
                {
                    switch (args[i])
                    {
                        #region U

                        //This is triggered when the port is to be changed
                        case "-u":
                            serverType = 'u';

                            //This tracks the number of arguments
                            noOfArgs++;

                            //This is used to break out of the enclosing while loop
                            i++;

                            //This breaks out of the case
                            break;

                        #endregion

                        #region T

                        //This is triggered when the port is to be changed
                        case "-t":
                            serverType = 't';

                            //This tracks the number of arguments
                            noOfArgs++;

                            //This is used to break out of the enclosing while loop
                            i++;

                            //This breaks out of the case
                            break;

                        #endregion

                        #region I

                        //This is triggured when the IP address is to be changed
                        case "-i":

                            //This ensures that there is an IP address to set
                            if (i + 2 <= args.Length)
                            {
                                try
                                {
                                    //This sets the IP address
                                    ipAddress = args[i + 1];

                                    //This tracks the number of arguments
                                    noOfArgs = noOfArgs + 2;
                                }
                                catch (Exception ex)
                                {
#if DEBUG
                                    Console.WriteLine("ERROR: " + ex.ToString());
#endif

                                    //This tracks the number of arguments
                                    noOfArgs++;
                                }
                            }
                            else
                            {
                                //This tracks the number of arguments
                                noOfArgs++;
                            }

                            //This is used to break out of the enclosing while loop
                            i = i + 2;

                            //This breaks out of the case
                            break;

                        #endregion

                        #region P

                        //This is triggered when the port is to be changed
                        case "-p":

                            //This ensures that there is an IP address to set
                            if (i + 2 <= args.Length)
                            {
                                try
                                {
                                    //This sets the port
                                    port = int.Parse(args[i + 1]);

                                    //This tracks the number of arguments
                                    noOfArgs = noOfArgs + 2;
                                }
                                catch (Exception ex)
                                {
#if DEBUG
                                    Console.WriteLine("ERROR: " + ex.ToString());
#endif

                                    //This tracks the number of arguments
                                    noOfArgs++;
                                }
                            }
                            else
                            {
                                //This tracks the number of arguments
                                noOfArgs++;
                            }

                            //This is used to break out of the enclosing while loop
                            i = i + 2;

                            //This breaks out of the case
                            break;

                        #endregion

                        #region Default

                        //This is the default case to be called whenever the argument doesn't match one of the other cases
                        default:

                            //This tests to see if a user name has already been set
                            if (usernameBool == true)
                            {
                                //This sets the location
                                location = args[i];

                                //This ensures the program does not crash by executing the following lines on a empty string
                                if (location != string.Empty)
                                {
                                    //This removes all black space from the beginning and end of the location before putting it into an array of its characters
                                    char[] locationCharArray = location.Trim().ToCharArray();

                                    //if the location begins with a / the location is made invalid so as not to interfere with future code
                                    if ((locationCharArray[0] == '/'))
                                    {
                                        argBool = false;

#if DEBUG
                                        Console.WriteLine("ERROR: Invalid arguments.");
#endif

                                        //This breaks out of the case
                                        break;
                                    }
                                }

                                //If the location is already set that must mean that an invalid set of arguments has been entered as the last argument will always be the location thus this portion of code will exit the program under these circumstances
                                if (locationBool == true)
                                {
                                    argBool = false;

#if DEBUG
                                    Console.WriteLine("ERROR: Invalid arguments.");
#endif

                                    //This breaks out of the case
                                    break;
                                }
                                else
                                {
                                    //This is a boolean to track the status of the location
                                    locationBool = true;
                                }
                            }
                            else
                            {
                                //This sets the user name
                                username = args[i];

                                //This is a boolean to track the status of the user name
                                usernameBool = true;
                            }

                            //This is used to break out of the enclosing while loop
                            i++;

                            //This breaks out of the case
                            break;

                            #endregion
                    }

                    //This breaks out of the while loop if the arguments given are invalid
                    if (!argBool)
                    {
                        break;
                    }
                }
            }
            else
            {
#if DEBUG
                //This prints to the screen an error message
                Console.WriteLine("ERROR: No arguments given.");
#endif
            }
        }
    }
}