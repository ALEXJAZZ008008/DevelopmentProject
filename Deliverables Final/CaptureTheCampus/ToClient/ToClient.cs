using System;

namespace ToClient
{
    class ToClient
    {
        private static void Main()
        {
            Loop();
        }

        private static void Loop()
        {
            while (true)
            {
                Client.Client client = new Client.Client();

#if DEBUG
                string[] args = Console.ReadLine().Split(new char[] { ' ' });
                Console.WriteLine(client.Input(args));
#endif
            }
        }
    }
}