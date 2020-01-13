using System;
using System.Threading;

namespace AnotherThreadPool
{
    class Program
    {
        public static void DoSomeWork(Object param)
        {
            Thread.Sleep(Convert.ToInt32(param) * 1000);
            Console.WriteLine($"Calculations done for {param}!");
        }

        static void Main(string[] args)
        {
            var random = new Random();

            for (int i = 0; i < 100; i++)
            {
                var num = random.Next(1, 5);

                MyThreadPool.QueueUserWorkItem(DoSomeWork, num);
            }
        }
    }
}
