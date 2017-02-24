using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tests
{
    internal class ThreadingTest
    {
        internal void Input()
        {
            bool whileContinue = true;
            bool notValidResponse1 = false;
            int threads;

            Client.Client client = new Client.Client();
            TCPServer.TCPServer tcpServer = new TCPServer.TCPServer();
            CancellationToken ct = new CancellationToken();

            Task serverTask = new Task(() => tcpServer.Input(ct));
            serverTask.Start();

            do
            {
                Console.WriteLine();
                Console.Write("Please enter an initial number of threads: ");

                string testNumberString = Console.ReadLine();

                if (int.TryParse(testNumberString.Trim(), out threads))
                {
                    while (whileContinue)
                    {
                        Console.WriteLine();
                        Console.Write("Number of threads: " + threads);
                        Console.WriteLine();
                        
                        Parallel.For(0, threads, index => client.Input(new string[] { "-t", "username", "location" }));

                        Thread.Sleep(1000);

                        Console.WriteLine();
                        Console.Write("Would you like to continue Y/N: ");

                        bool notValidResponse2 = false;

                        do
                        {
                            string yn = Console.ReadLine().ToUpper();

                            if (yn == "Y")
                            {
                                threads++;
                            }
                            else
                            {
                                if (yn == "N")
                                {
                                    whileContinue = false;
                                }
                                else
                                {
                                    Console.WriteLine();
                                    Console.Write("Please enter a valid response");

                                    notValidResponse2 = true;
                                }
                            }
                        } while (notValidResponse2);
                    }
                }
                else
                {
                    Console.WriteLine();
                    Console.Write("Please enter a valid response");

                    notValidResponse1 = true;
                }
            } while (notValidResponse1);
        }
    }
}