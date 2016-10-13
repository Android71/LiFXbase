using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiFXbase
{
    public static class Utils
    {
        public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Converts HSL to HSB
        /// </summary>
        /// <param name="h" Hue, must be in [0.0, 360.0].></param>
        /// <param name="s" Saturation, must be in [0.0, 1.0].></param>
        /// <param name="l" Lightness, must be in [0.0, 1.0].></param>
        /// <returns>
        /// double[0] - Hue
        /// double[1] - Saturation
        /// double[2] - Brightness
        /// </returns>
        public static double[] HSL2HSB(double h, double s, double l)
        {
            double[] hsb = new double[3];

            // Hue
            hsb[0] = h;

            // Brightness
            //B = 2L + Shsl(1−| 2L−1 |) / 2
            hsb[2] = (2.0 * l + s * (1.0 - Math.Abs(2.0 * l - 1.0))) / 2.0;

            // Saturation
            //Shsb = 2(B−L)/B
            hsb[1] = 2.0 * (hsb[2] - l) / hsb[2];

            return hsb;
        }
    }
}
