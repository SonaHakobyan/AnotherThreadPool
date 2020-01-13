using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AnotherThreadPool
{
    /// <summary>
    /// Custom Thread Pool implementation
    /// </summary>
    public static class MyThreadPool
    {
        /// <summary>
        /// Queue of user work items
        /// Each item is a tuple of Callback function and State
        /// </summary>
        private static Queue<Tuple<WaitCallback, object>> workItems;

        /// <summary>
        /// AutoResetEvent for mutual exclusion
        /// </summary>
        private static AutoResetEvent mutex;

        /// <summary>
        /// Semaphore for counting the number of full spaces in the queue
        /// </summary>
        private static Semaphore full;

        /// <summary>
        /// Collection of worker threads
        /// </summary>
        private static List<Thread> workers;

        /// <summary>
        /// Background worker thread
        /// </summary>
        private static Thread backgroundWorker;

        /// <summary>
        /// Maximum number of worker threads in the thread pool
        /// </summary>
        private static int maxThreads;

        /// <summary>
        /// Static ctor for MyThreadPool
        /// </summary>
        static MyThreadPool()
        {
            // Initialize static fields

            workItems = new Queue<Tuple<WaitCallback, object>>();
            full = new Semaphore(0, int.MaxValue);
            mutex = new AutoResetEvent(true);
            workers = new List<Thread>();
            maxThreads = 100;

            // Initialize worker threads
            for (int i = 0; i < maxThreads / 10; i++)
            {
                // Create a new thread 
                var worker = new Thread(ThreadFunc);

                // Add to workers collection 
                workers.Add(worker);

                // Start thread
                worker.Start();
            }

            // IInitialize the background worker thread and start
            backgroundWorker = new Thread(BackgroundFunction);
            backgroundWorker.Start();
        }

        /// <summary>
        /// Thread Function that wait until there is work, then get and do it
        /// </summary>
        private static void ThreadFunc()
        {
            while (true)
            {
                // Down full semaphore
                full.WaitOne();

                // Lock queue
                mutex.Reset();

                // Dequeue work item
                var tuple = workItems.Dequeue();

                // Unlock queue
                mutex.Set();

                // Invoke callback 
                tuple.Item1?.Invoke(tuple.Item2);
            }
        }

        /// <summary>
        /// Checks and creates some new threds if needed
        /// </summary>
        private static void BackgroundFunction()
        {
            while (true)
            {
                // Current workers count
                var totalWorkers = workers.Count;

                if (totalWorkers >= maxThreads) return;

                // Current work items count
                var totalWorkItems = workItems.Count;

                // Creates some new threds if needed
                if (totalWorkItems / totalWorkers >= 10)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        // Create a new thread
                        var thread = new Thread(ThreadFunc);

                        // Add to workers list
                        workers.Add(thread);

                        // Start the thread
                        thread.Start();
                    }
                }

                // Sleep 10 sec
                Thread.Sleep(10000);
            }
        }

        /// <summary>
        ///  Queues a method for execution, and specifies an object containing data to be
        ///  used by the method. The method executes when a thread pool thread becomes available
        /// </summary>
        /// <param name="callBack">The method to execute</param>
        /// <param name="state">An object containing data to be used by the method.</param>
        public static void QueueUserWorkItem(WaitCallback callBack, object state = null)
        {
            // Do nothing in case of null callback
            if (callBack == null) return;

            // Lock queue
            mutex.Reset();

            // Enqueue callback and its state
            workItems.Enqueue(new Tuple<WaitCallback, object>(callBack, state));

            // Unlock queue
            mutex.Set();

            // Up full semaphore
            full.Release();
        }
    }
}

