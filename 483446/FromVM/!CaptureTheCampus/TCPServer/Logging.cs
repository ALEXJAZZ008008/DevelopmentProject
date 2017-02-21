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
            TCPServer.loggingMutex.WaitOne();

            //This adds the correct log message to a string
            TCPServer.logMessage = TCPServer.logMessage + ipAddress + " - - " + DateTime.Now.ToString("'['dd'/'MM'/'yyyy':'HH':'mm':'ss zz00']'") + " \"" + loggingOutput + "\"" + "\r\n";

            //This allowes another thread to enter the previous code
            TCPServer.loggingMutex.ReleaseMutex();
        }

        public void Log()
        {
            while (true)
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

                //This locks this part of the code univerally
                TCPServer.loggingMutex.WaitOne();

                TCPServer.logMessage = string.Empty;

                //This allowes another thread to enter the previous code
                TCPServer.loggingMutex.ReleaseMutex();
            }
        }
    }
}