using System;

namespace Client
{
    class ToClient
    {
        public static void Main()
        {
            Loop();
        }

        private static void Loop()
        {
            while (true)
            {
                Client client = new Client();

#if DEBUG
                string[] args = Console.ReadLine().Split(new char[] { ' ' });
                Console.WriteLine(client.Input(args));
#endif
            }
        }
    }
}