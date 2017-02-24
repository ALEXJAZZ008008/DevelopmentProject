using System;

namespace TCPServer
{
    class Logging
    {
        //This allows external access to the private log method
        public void ToLog(string loggingOutput, string ipAddress)
        {
            string logMessage = ipAddress + " - - " + DateTime.Now.ToString("'['dd'/'MM'/'yyyy':'HH':'mm':'ss zz00']'") + " \"" + loggingOutput + "\"" + "\r\n";

            Log(logMessage);

#if DEBUG
            //This outputs the log message to the screen
            Console.Write(logMessage);
#endif
        }

        private void Log(string logMessage)
        {
            TCPServer.textWriter.Write(logMessage);
            TCPServer.textWriter.Flush();
        }
    }
}