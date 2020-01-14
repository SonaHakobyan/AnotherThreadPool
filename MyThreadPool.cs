using System;
using System.Collections.Generic;
using System.Threading;

namespace AnotherThreadPool
{
    /// <summary>
    /// Custom Thread Pool implementation
    /// </summary>
    public class MyThreadPool
    {
        /// <summary>
        /// Queue of user work items and states
        /// </summary>
        private Queue<Tuple<Action<object>, object>> workItems;

        /// <summary>
        /// Mutex for mutual exclusion
        /// </summary>
        private Mutex mutex;

        /// <summary>
        /// Semaphore for counting the number of full spaces in the queue
        /// </summary>
        private Semaphore full;

        /// <summary>
        /// Collection of worker threads
        /// </summary>
        private List<Thread> workers;

        /// <summary>
        /// Background worker thread
        /// </summary>
        private Thread backgroundWorker;

        /// <summary>
        /// Maximum number of worker threads in the thread pool
        /// </summary>
        private int maxThreads;

        /// <summary>
        /// Initializes a new instance of the MyThreadPool class
        /// </summary>
        public MyThreadPool()
        {
            // Initialize static fields

            this.workItems = new Queue<Tuple<Action<object>, object>>();
            this.full = new Semaphore(0, int.MaxValue);
            this.mutex = new Mutex();
            this.workers = new List<Thread>();
            this.maxThreads = 100;

            // Initialize worker threads
            for (int i = 0; i < maxThreads / 10; i++)
            {
                // Create a new thread 
                var worker = new Thread(ExecuteWorkItem);

                // Add to workers collection 
                workers.Add(worker);

                // Start thread
                worker.Start();
            }

            // Initialize the background worker thread
            backgroundWorker = new Thread(BalanceThreads);

            // Start the background worker thread
            backgroundWorker.Start();
        }

        /// <summary>
        /// Creates some new threads if needed
        /// </summary>
        private void BalanceThreads()
        {
            while (true)
            {
                // Current workers count
                var totalWorkers = workers.Count;

                // No more threads are allowed
                if (totalWorkers >= maxThreads) return;

                // Current work items count
                var totalWorkItems = workItems.Count;

                // Create some new threads if needed
                if (totalWorkItems / totalWorkers >= 10)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        // Create a new thread
                        var worker = new Thread(ExecuteWorkItem);

                        // Add to workers list
                        workers.Add(worker);

                        // Start the thread
                        worker.Start();
                    }
                }

                // Sleep 10 sec
                Thread.Sleep(10000);
            }
        }

        /// <summary>
        /// Function that wait until there is some work, then get and do it
        /// </summary>
        private void ExecuteWorkItem()
        {
            while (true)
            {
                // Down full semaphore
                full.WaitOne();

                // Lock queue
                mutex.WaitOne();

                // Dequeue work item
                var tuple = workItems.Dequeue();

                // Unlock queue
                mutex.ReleaseMutex();

                // Execute the action
                tuple.Item1?.Invoke(tuple.Item2);
            }
        }

        /// <summary>
        ///  Queues a method for execution, and specifies an object containing data to be
        ///  used by the method. The method executes when a thread pool thread becomes available
        /// </summary>
        /// <param name="action">The method to execute</param>
        /// <param name="state">An object containing data to be used by the method.</param>
        public void QueueUserWorkItem(Action<object> action, object state = null)
        {
            // Return in case of null action
            if (action == null) return;

            // Lock queue
            mutex.WaitOne();

            // Enqueue action and its state
            workItems.Enqueue(new Tuple<Action<object>, object>(action, state));

            // Unlock queue
            mutex.ReleaseMutex();

            // Up full semaphore
            full.Release();
        }
    }
}

