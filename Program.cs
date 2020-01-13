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
            //Console.WriteLine($"Calculations done by thread {Thread.CurrentThread.ManagedThreadId}!");
        }

        static void Main(string[] args)
        {
            var random = new Random();

            for (int i = 0; i < 1000000; i++)
            {
                var num = random.Next(1, 5);

                MyThreadPool.QueueUserWorkItem(DoSomeWork, num);
                //MyThreadPool.QueueUserWorkItem(x => Program.DoSomeWork(num));
                //MyThreadPool.QueueUserWorkItem(x => Console.WriteLine("Result: " + x), num);
            }


        }
    }
}
