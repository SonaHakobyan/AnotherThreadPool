namespace AnotherThreadPool
{
    public class Fibonacci
    {
        /// <summary>
        /// Recursive method that calculates the Nth Fibonacci number
        /// </summary>
        /// <param name="n">The number</param>
        /// <returns></returns>
        public int Calculate(int n)
        {
            // Base step of recursion
            if (n <= 1)
            {
                return n;
            }

            // Add the previous two Fibonacci numbers
            return Calculate(n - 1) + Calculate(n - 2);
        }
    }
}
