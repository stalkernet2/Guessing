using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guessing.Extentions
{
    internal static class Extentions
    {
        internal static Color Noise(this Color value, int noisePower)
        {
            var random = new Random();
            var randNum = random.Next(0, noisePower);

            var color = new int[3];

            color[0] = CheckValue(value.R + randNum);
            color[1] = CheckValue(value.G + randNum);
            color[2] = CheckValue(value.B + randNum);

            return Color.FromArgb(color[0], color[1], color[2]);
        }

        internal static Color[,] ToArrayColor(this Bitmap target)
        {
            var output = new Color[target.Width, target.Height];

            for (int i = 0; i < target.Height; i++)
                for (int j = 0; j < target.Width; j++)
                    output[j, i] = target.GetPixel(j, i);

            return output;
        }

        private static int CheckValue(int value)
        {
            value = value > 255 ? 255 : value;
            return value < 0 ? 0 : value;
        }
    }
}
