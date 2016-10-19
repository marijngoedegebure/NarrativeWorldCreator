using System;
using System.Collections.Generic;

using System.Text;

namespace Common.Util
{
    public class PerlinNoise
    {
        private double initFrequency = 1.0;
        private double initAmplitude = 1.0;
        private double persistence = 0.65;
        private int octaves = 4;

        /// <summary>
        /// Initial frequency.
        /// </summary>
        /// 
        /// <remarks><para>The property sets initial frequency of the first octave. Frequencies for
        /// next octaves are calculated using the next equation:<br />
        /// frequency<sub>i</sub> = <see cref="InitFrequency"/> * 2<sup>i</sup>,
        /// where i = [0, <see cref="Octaves"/>).
        /// </para>
        /// 
        /// <para>Default value is set to <b>1</b>.</para>
        /// </remarks>
        /// 
        public double InitFrequency
        {
            get { return initFrequency; }
            set { initFrequency = value; }
        }

        /// <summary>
        /// Initial amplitude.
        /// </summary>
        /// 
        /// <remarks><para>The property sets initial amplitude of the first octave. Amplitudes for
        /// next octaves are calculated using the next equation:<br />
        /// amplitude<sub>i</sub> = <see cref="InitAmplitude"/> * <see cref="Persistence"/><sup>i</sup>,
        /// where i = [0, <see cref="Octaves"/>).
        /// </para>
        /// 
        /// <para>Default value is set to <b>1</b>.</para>
        /// </remarks>
        ///
        public double InitAmplitude
        {
            get { return initAmplitude; }
            set { initAmplitude = value; }
        }

        /// <summary>
        /// Persistence value.
        /// </summary>
        ///
        /// <remarks><para>The property sets so called persistence value, which controls the way
        /// how <see cref="InitAmplitude">amplitude</see> is calculated for each octave comprising
        /// the Perlin noise function.</para>
        /// 
        /// <para>Default value is set to <b>0.65</b>.</para>
        /// </remarks>
        ///
        public double Persistence
        {
            get { return persistence; }
            set { persistence = value; }
        }

        /// <summary>
        /// Number of octaves, [1, 32].
        /// </summary>
        /// 
        /// <remarks><para>The property sets the number of noise functions, which sum up the resulting
        /// Perlin noise function.</para>
        /// 
        /// <para>Default value is set to <b>4</b>.</para>
        /// </remarks>
        /// 
        public int Octaves
        {
            get { return octaves; }
            set { octaves = System.Math.Max(1, System.Math.Min(32, value)); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PerlinNoise"/> class.
        /// </summary>
        /// 
        public PerlinNoise() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PerlinNoise"/> class.
        /// </summary>
        /// 
        /// <param name="octaves">Number of octaves (see <see cref="Octaves"/> property).</param>
        /// <param name="persistence">Persistence value (see <see cref="Persistence"/> property).</param>
        /// 
        public PerlinNoise(int octaves, double persistence)
        {
            this.octaves = octaves;
            this.persistence = persistence;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="PerlinNoise"/> class.
        /// </summary>
        /// 
        /// <param name="octaves">Number of octaves (see <see cref="Octaves"/> property).</param>
        /// <param name="persistence">Persistence value (see <see cref="Persistence"/> property).</param>
        /// <param name="initFrequency">Initial frequency (see <see cref="InitFrequency"/> property).</param>
        /// <param name="initAmplitude">Initial amplitude (see <see cref="InitAmplitude"/> property).</param>
        /// 
        public PerlinNoise(int octaves, double persistence, double initFrequency, double initAmplitude)
        {
            this.octaves = octaves;
            this.persistence = persistence;
            this.initFrequency = initFrequency;
            this.initAmplitude = initAmplitude;
        }

        /// <summary>
        /// 1-D Perlin noise function.
        /// </summary>
        /// 
        /// <param name="x">x value.</param>
        /// 
        /// <returns>Returns function's value at point <paramref name="x"/>.</returns>
        /// 
        public double Function(double x)
        {
            double frequency = initFrequency;
            double amplitude = initAmplitude;
            double sum = 0;

            // octaves
            for (int i = 0; i < octaves; i++)
            {
                sum += SmoothedNoise(x * frequency) * amplitude;

                frequency *= 2;
                amplitude *= persistence;
            }
            return sum;
        }

        /// <summary>
        /// 2-D Perlin noise function.
        /// </summary>
        /// 
        /// <param name="x">x value.</param>
        /// <param name="y">y value.</param>
        /// 
        /// <returns>Returns function's value at point (<paramref name="x"/>, <paramref name="y"/>).</returns>
        /// 
        public double Function2D(double x, double y)
        {
            double frequency = initFrequency;
            double amplitude = initAmplitude;
            double sum = 0;

            // octaves
            for (int i = 0; i < octaves; i++)
            {
                sum += SmoothedNoise(x * frequency, y * frequency) * amplitude;

                frequency *= 2;
                amplitude *= persistence;
            }
            return sum;
        }


        /// <summary>
        /// Ordinary noise function
        /// </summary>
        private double Noise(int x)
        {
            int n = (x << 13) ^ x;

            return (1.0 - ((n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824.0);
        }
        private double Noise(int x, int y)
        {
            int n = x + y * 57;
            n = (n << 13) ^ n;

            return (1.0 - ((n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824.0);
        }


        /// <summary>
        /// Smoothed noise.
        /// </summary>
        private double SmoothedNoise(double x)
        {
            int xInt = (int)x;
            double xFrac = x - xInt;

            return CosineInterpolate(Noise(xInt), Noise(xInt + 1), xFrac);
        }
        private double SmoothedNoise(double x, double y)
        {
            int xInt = (int)x;
            int yInt = (int)y;
            double xFrac = x - xInt;
            double yFrac = y - yInt;

            // get four noise values
            double x0y0 = Noise(xInt, yInt);
            double x1y0 = Noise(xInt + 1, yInt);
            double x0y1 = Noise(xInt, yInt + 1);
            double x1y1 = Noise(xInt + 1, yInt + 1);

            // x interpolation
            //double v1 = FadeInterpolation(x0y0, x1y0, xFrac);
            double v1 = CosineInterpolate(x0y0, x1y0, xFrac);
            double v2 = CosineInterpolate(x0y1, x1y1, xFrac);
            //double v2 = FadeInterpolation(x0y1, x1y1, xFrac);
            // y interpolation
            return CosineInterpolate(v1, v2, yFrac);
            //return FadeInterpolation(v1, v2, yFrac);
        }

        private double FadeInterpolation(double v1, double v2, double frac)
        {
            return v1 + (v2 - v1) * fade(frac);
        }

        double fade(double t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        /// <summary>
        /// Cosine interpolation.
        /// </summary>
        private double CosineInterpolate(double x1, double x2, double a)
        {
            double f = (1 - Math.Cos(a * Math.PI)) * 0.5;

            return x1 * (1 - f) + x2 * f;
        }
    }
}
