using System;

namespace Tests
{
    class ToTests
    {
        static void Main(string[] args)
        {
            TestSelection();
        }

        private static void TestSelection()
        {
            bool notValidResponse = false;
            string[] tests = { "Threading" };
            int switchVariable;

            Console.WriteLine("Tests...");
            Console.WriteLine();

            for (int i = 0; i < tests.Length; i++)
            {
                Console.WriteLine((i + 1).ToString() + ": " + tests[i]);
            }

            do
            {
                Console.WriteLine();
                Console.Write("Please Select a test number: ");

                string testNumberString = Console.ReadLine();

                if (int.TryParse(testNumberString.Trim(), out switchVariable))
                {
                    switch ((switchVariable - 1))
                    {
                        case 0:

                            ThreadingTest threadingTest = new ThreadingTest();

                            threadingTest.Input();

                            break;
                    }
                }
                else
                {
                    Console.WriteLine();
                    Console.Write("Please enter a valid response");

                    notValidResponse = true;
                }
            } while (notValidResponse);
        }
    }
}
