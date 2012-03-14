using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bespoke.Common
{
    public static class RandomHelper
    {
        /// <summary>
        /// Static constructor.
        /// </summary>
        static RandomHelper()
        {
            sRandom = new Random();
        }

        public static int Next(int minValue, int maxValue)
        {
            return sRandom.Next(minValue, maxValue);
        }

        public static double NextDouble(double minValue, double maxValue)
        {
            return NextDouble(sRandom, minValue, maxValue);
        }

        public static double NextDouble(Random random, double minValue, double maxValue)
        {
            if (minValue > maxValue)
            {
                throw new ArgumentOutOfRangeException("minValue");
            }

            return minValue + (random.NextDouble() * (maxValue - minValue));
        }

        public static bool RandomBoolean()
        {
            return (sRandom.NextDouble() > 0.5);
        }

        private static Random sRandom;
    }
}
