using System;
using System.IO;

namespace TCPServer
{
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
            streamWriter.Flush();
            streamWriter.Close();

#if DEBUG
            //This outputs the log message to the screen
            Console.Write(TCPServer.logMessage);
#endif

            TCPServer.logMessage = string.Empty;
        }
    }
}