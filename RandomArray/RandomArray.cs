using KTreeDataTypes;
using System;

namespace RandomArray
{
    /// <summary>
    /// A static class to generate random arrays of numbers and objects.
    /// </summary>
    public static class RandomArray
    {
        /// <summary>
        /// Random object to generate pseudo-random numbers.
        /// </summary>
        private static readonly Random random = new();

        /// <summary>
        /// Generates a specified length array of random 1-D point objects in a specified range.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random numbers generated.</param>
        /// <param name="maxValue">The inclusive upper bound of the random numbers generated.</param>
        /// <param name="numValues">The number of random numbers to generate.</param>
        /// <returns>An array of random 1-D points.</returns>
        public static Point1D[] GenerateRandom1DArray(int minValue, int maxValue, int numValues)
        {
            // Generate pool of numbers to select from
            int[] randomPool = GenerateRandomIntArray(minValue, maxValue, numValues);

            Point1D[] random1D = new Point1D[numValues];

            // Create objects from first numValues in pool
            for (int i = 0; i < numValues; i++)
            {
                random1D[i] = new Point1D(randomPool[i]);
            }

            return random1D;
        }

        /// <summary>
        /// Generates a specified length array of random 2-D point objects in a specified range.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random numbers generated.</param>
        /// <param name="maxValue">The inclusive upper bound of the random numbers generated.</param>
        /// <param name="numValues">The number of random numbers to generate.</param>
        /// <returns>An array of random 2-D points.</returns>
        public static Point2D[] GenerateRandom2DArray(int minValue, int maxValue, int numValues)
        {
            // Generate pools of numbers to select from
            int[] randomXPool = GenerateRandomIntArray(minValue, maxValue, numValues);
            int[] randomYPool = GenerateRandomIntArray(minValue, maxValue, numValues);

            Point2D[] random2D = new Point2D[numValues];

            // Create objects from first numValues in pools
            for (int i = 0; i < numValues; i++)
            {
                random2D[i] = new Point2D(randomXPool[i], randomYPool[i]);
            }

            return random2D;
        }

        /// <summary>
        /// Generates a specified length array of random integers in a specified range.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random numbers generated.</param>
        /// <param name="maxValue">
        /// The inclusive upper bound of the random numbers generated. maxValue must be greater than or equal to minValue.
        /// </param>
        /// <param name="numValues">
        /// The number of random numbers to generate. numValues must be greater than zero and less than maxValue - minValue.
        /// </param>
        /// <returns>An array of random integers.</returns>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public static int[] GenerateRandomIntArray(int minValue, int maxValue, int numValues)
        {
            // Check arguments
            if (minValue > maxValue)
            {
                string message = string.Format("Cannot be greater than {0}.", nameof(maxValue));
                throw new ArgumentOutOfRangeException(nameof(minValue), message);
            }
            if (numValues < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(numValues), "Cannot be less than one.");
            }
            if (numValues > (maxValue - minValue) + 1)
            {
                string message = string.Format("Cannot be greater than ({0} - {1}) + 1.", nameof(maxValue), nameof(minValue));
                throw new ArgumentOutOfRangeException(nameof(numValues), message);
            }

            int[] randomPool = new int[maxValue - minValue + 1];

            // Generate pool of numbers to select from
            for (int i = 0; i < randomPool.Length; i++)
            {
                randomPool[i] = minValue + i;
            }

            int numUnshuffled = randomPool.Length;

            // Shuffle pool to randomise
            while (numUnshuffled > 1)
            {
                int randomIndex = random.Next(numUnshuffled--);
                (randomPool[randomIndex], randomPool[numUnshuffled]) = (randomPool[numUnshuffled], randomPool[randomIndex]);
            }

            int[] randomSelection = new int[numValues];

            // Select first numValues in pool
            for (int i = 0; i < numValues; i++)
            {
                randomSelection[i] = randomPool[i];
            }

            return randomSelection;
        }
    }
}
