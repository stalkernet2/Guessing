using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guessing.Curse
{
    public class Curse
    {
        public Color[,] Image(Size size, Bitmap image, int every)
        {
            Color[,] curseImage = new Color[size.Width, size.Height];

            var ghostyY = -1;
            for (int y = 0; y < image.Height; y += every)
            {
                var ghostyX = -1;
                ghostyY++;
                for (int x = 0; x < image.Width; x += every)
                {
                    ghostyX++;
                    curseImage[ghostyX, ghostyY] = image.GetPixel(x, y);
                }
            }
            return curseImage;
        }
    }
}
