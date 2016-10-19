using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{

    public class RandomSettings
    {

        public int Seed
        {
            get { return seed; }
        }

        public long NumGenerated
        {
            get { return numGenerated; }
        }

        private readonly int seed;
        private readonly long numGenerated;

        public RandomSettings(int seed, long numGenerated)
        {
            this.seed = seed;
            this.numGenerated = numGenerated;
        }
    }

    public class RandomNumber
    {
        private static Random generator;
        private static int seed;
        private static long numGenerated;

        public static RandomSettings Settings
        {
            get
            {
                return new RandomSettings(seed, numGenerated);
            }

            set
            {
                Resume(value.Seed, value.NumGenerated);
            }
        }

        static RandomNumber()
        {
            Reset();
        }

        public static double Random()
        {
            numGenerated++;
            return generator.NextDouble();
        }

        public static float RandomF()
        {
            return (float)Random();
        }

        public static double Random(double min, double max)
        {
            return min + Random() * (max - min);
        }

        public static int Random(int min, int max)
        {
            return (int)Random((double)min, (double)max);
        }

        public static float RandomF(float min, float max)
        {
            if (min.Equals(float.NegativeInfinity))
                min = float.MinValue;
            if (min.Equals(float.PositiveInfinity))
                min = float.MaxValue;
            if (max.Equals(float.NegativeInfinity))
                max = float.MinValue;
            if (max.Equals(float.PositiveInfinity))
                max = float.MaxValue;
            return min + RandomF() * (max - min);
        }

        public static double GausRandomClamped()
        {
            return Math.Max(-1.0, Math.Min(1.0, GausRandom()));
        }

        public static double GausRandom()
        {
            double x1, x2, w, y1, y2;
            do
            {
                x1 = 2.0 * Random() - 1.0;
                x2 = 2.0 * Random() - 1.0;
                w = x1 * x1 + x2 * x2;
            } while (w >= 1.0);

            w = Math.Sqrt((-2.0 * Math.Log(w, Math.E)) / w);
            y1 = x1 * w;
            y2 = x2 * w;
            //Console.WriteLine("y1 = {0:f}, y2 = {1:f}", y1, y2);
            return Random() > 0.5 ? y2 : y1;
        }

        public static int GetSeed()
        {
            return seed;
        }

        public static void SetSeed(int newSeed)
        {
            seed = newSeed;
            generator = new Random(seed);
            numGenerated = 0;
        }

        public static void Reset()
        {
            SetSeed(Math.Abs((int)DateTime.Now.Ticks));
        }


        public static void Resume(RandomSettings randomSettings)
        {
            Resume(randomSettings.Seed, randomSettings.NumGenerated);
        }

        public static void Resume(int newSeed, long newNumGenerated)
        {
            if (seed != newSeed || numGenerated != newNumGenerated)
            {
                SetSeed(newSeed);
                for (long i = 0; i < newNumGenerated; ++i)
                {
                    Random();
                }
            }
        }

        public static Random GetGenerator()
        {
            return generator;
        }

        public static void SetGenerator(Random r)
        {
            generator = r;
        }

        /// <param name="maxValue">The exclusive upper bound</param>
        public static int Next(int maxValue)
        {
            if (generator == null)
                Reset();
            return generator.Next(maxValue);
        }

        /// <summary>
        /// Returns a random number within a specified range
        /// </summary>
        /// <param name="minValue">The inclusive lower bound</param>
        /// <param name="maxValue">The exclusive upper bound</param>
        public static int Next(int minValue, int maxValue)
        {
            if (generator == null)
                Reset();
            return generator.Next(minValue, maxValue);
        }

        public static double LookupRandom(double[] randomTable, int x, int y)
        {
            const int PRIME = 31;
            int result = PRIME * (PRIME + x) + y;
            return randomTable[result % randomTable.Length];
        }

        public static void FillRandomTable(double[] randomTable)
        {
            for (int i = 0; i < randomTable.Length; i++)
            {
                randomTable[i] = Random();
            }
        }

        public static double[] CreateRandomTable()
        {
            double[] randomTable = new double[1457];
            FillRandomTable(randomTable);
            return randomTable;
        }
    }
}
