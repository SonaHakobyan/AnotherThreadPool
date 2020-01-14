using System;

namespace AnotherThreadPool
{
    class Program
    {
        /// <summary>
        /// The entry point of the program
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // Random number generator
            var random = new Random();

            var fibonacci = new Fibonacci();

            // Thread Pool instance
            var threadPool = new MyThreadPool();

            for (int i = 0; i < 100000; i++)
            {
                // Generate a rundom number
                var n = random.Next(1, 30);

                // Local instance vor index
                int j = i; 

                // Queue User Work Item
                threadPool.QueueUserWorkItem(x => 
                {
                    // Calculate the number
                    var result = fibonacci.Calculate(n);

                    // Print on console
                    Console.WriteLine($"{j}: {n}th Fibbonacci Number is {result}");
                });
            }
        }
    }
}
