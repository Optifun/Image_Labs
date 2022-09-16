﻿using System;

namespace Labs.Core.Scheme
{
    public record struct HLSA
    {
        public enum Channel
        {
            Undefined = 0,
            Alpha = 1,
            Hue = 2,
            Lightness = 4,
            Saturation = 8,
            HLS = 14,
            All = 15
        }

        /// <summary>
        /// Hue (0, 360)
        /// </summary>
        public double H;

        /// <summary>
        /// Lightness (0, 1)
        /// </summary>
        public double L;

        /// <summary>
        /// Saturation (0, 1)
        /// </summary>
        public double S;

        /// <summary>
        /// Alpha (0, 1)
        /// </summary>
        public double A;

        public HLSA(double h, double l, double s, double a = 1)
        {
            H = h;
            L = l;
            S = s;
            A = a;
        }

        public ARGB ToARGB()
        {
            if (S == 0)
            {
                // ахроматический цвет (шкала серого)
                var v = (byte) Math.Round(L * 255.0);
                return new ARGB(v, v, v, (byte) (A * 255));
            }

            double q = (L < 0.5) ? (L * (1.0 + S)) : (L + S - (L * S));
            double p = (2.0 * L) - q;

            double Hk = H / 360.0;
            double[] T = new double[3];
            T[0] = Hk + (1.0 / 3.0); // Tr
            T[1] = Hk; // Tb
            T[2] = Hk - (1.0 / 3.0); // Tg

            for (int a = 0; a < 3; a++)
            {
                if (T[a] < 0) T[a] += 1.0;
                if (T[a] > 1) T[a] -= 1.0;

                if ((T[a] * 6) < 1)
                {
                    T[a] = p + ((q - p) * 6.0 * T[a]);
                }
                else if ((T[a] * 2.0) < 1) //(1.0/6.0)<=T[a] && T[a]<0.5
                {
                    T[a] = q;
                }
                else if ((T[a] * 3.0) < 2) // 0.5<=T[a] && T[a]<(2.0/3.0)
                {
                    T[a] = p + (q - p) * ((2.0 / 3.0) - T[a]) * 6.0;
                }
                else T[a] = p;
            }

            var r = (byte) Math.Round(T[0] * 255.0);
            var g = (byte) Math.Round(T[1] * 255.0);
            var b = (byte) Math.Round(T[2] * 255.0);
            return new ARGB(r, g, b, (byte) (A * 255));
        }
    }
}