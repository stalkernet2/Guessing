using Guessing.Extentions;

namespace Guessing
{
    public class Guesser : IDisposable
    {
        private readonly Size _newSize;

        private readonly int _scale;
        private readonly int _noisePower;

        private readonly Color[,] _colorMap;

        private readonly Color[,] _outputBitmap = new Color[0, 0];

        public Guesser(Bitmap image, int scale = 1, int noisePower = 1)
        {
            _newSize = new Size(image.Width * scale, image.Height * scale);
            _outputBitmap = new Color[_newSize.Width, _newSize.Height];

            _scale = scale;
            _colorMap = image.ToArrayColor();
            _noisePower = noisePower;
        }

        public void SolidMethod()
        {
            for (int y = 0; y < _newSize.Height; y += _scale)   // расстановка пикселей
                for (int x = 0; x < _newSize.Width; x += _scale)
                    _outputBitmap[x, y] = _colorMap[x / _scale, y / _scale];


            for (int y = 0; y < _newSize.Height; y += _scale)   // угадывание по x
            {
                for (int x = 0; x < _newSize.Width; x += _scale)
                {
                    if (x + _scale < _newSize.Width)
                        for (int ghostyX = 1; ghostyX <= _scale; ghostyX++)
                            _outputBitmap[x + ghostyX, y] = ColorDiffuse(_outputBitmap[x + (ghostyX - 1), y], _outputBitmap[x + _scale, y]).Noise(_noisePower);

                    else
                        break;
                }
            }

            for (int x = 0; x < _newSize.Width; x++)            // угадывание по y
            {
                for (int y = 0; y < _newSize.Height; y += _scale)
                {
                    if (y + _scale < _newSize.Height)
                        for (int ghostyY = 1; ghostyY <= _scale; ghostyY++)
                            _outputBitmap[x, y + ghostyY] = ColorDiffuse(_outputBitmap[x, y + (ghostyY - 1)], _outputBitmap[x, y + _scale]).Noise(_noisePower);

                    else
                        break;
                }
            }
        }

        public void NeightborMethod()
        {
            for (int y = 0; y < _newSize.Width - _scale; y += _scale)
            {
                for (int x = 0; x < _newSize.Height - _scale; x += _scale)
                {
                    for (int j = 0; j < _scale - 1; j++)
                    {
                        var anonX = x + 1;
                        var anonY = y + 1;
                        var invAnonX = x + _scale;
                        var invAnonY = y + _scale;

                        for (int h = 0; h < _scale - 1; h++)
                        {
                            _outputBitmap[anonX, anonY++] = SumNeighbors(_outputBitmap, anonX, anonY);
                            _outputBitmap[anonX++, anonY] = SumNeighbors(_outputBitmap, anonX, anonY);
                            _outputBitmap[anonX, invAnonY--] = SumNeighbors(_outputBitmap, anonX, anonY);
                            _outputBitmap[invAnonX--, anonY] = SumNeighbors(_outputBitmap, anonX, anonY);
                        }
                    }
                }
            }
        }

        public void MakeABase()
        {
            for (int y = 0; y < _newSize.Height; y += _scale)   // расстановка пикселей
                for (int x = 0; x < _newSize.Width; x += _scale)
                    _outputBitmap[x, y] = _colorMap[x / _scale, y / _scale];
        }

        public void DefaultMethod()
        {
            for (int y = 0; y < _newSize.Height; y += _scale)   // угадывание по x
            {
                for (int x = 0; x < _newSize.Width; x += _scale)
                {
                    if (x + _scale < _newSize.Width)
                        for (int ghostyX = 1; ghostyX <= _scale; ghostyX++)
                            _outputBitmap[x + ghostyX, y] = ColorDiffuse(_outputBitmap[x + (ghostyX - 1), y], _outputBitmap[x + _scale, y]).Noise(_noisePower);

                    else
                        break;
                }
            }

            for (int x = 0; x < _newSize.Width; x += _scale)    // угадывание по y
            {
                for (int y = 0; y < _newSize.Height; y += _scale)
                {
                    if (y + _scale < _newSize.Height)
                        for (int ghostyY = 1; ghostyY <= _scale; ghostyY++)
                            _outputBitmap[x, y + ghostyY] = ColorDiffuse(_outputBitmap[x, y + (ghostyY - 1)], _outputBitmap[x, y + _scale]).Noise(_noisePower);

                    else
                        break;
                }
            }
        }

        public Bitmap Draw()
        {
            var output = new Bitmap(_newSize.Width, _newSize.Height);

            for (int y = 0; y < output.Height; y++)
                for (int x = 0; x < output.Width; x++)
                    output.SetPixel(x, y, _outputBitmap[x, y]);
            
            return output;
        }
        
        private static Color ColorDiffuse(Color left, Color right)
        {
            var outR = (int)((left.R + right.R) / 2f);
            var outG = (int)((left.G + right.G) / 2f);
            var outB = (int)((left.B + right.B) / 2f);

            return Color.FromArgb(outR, outG, outB);
        }

        private static Color SumNeighbors(Color[,] colorMap, int x, int y)
        {
            var neighbours = new Color[4];
            int value = 0;

            if (colorMap[x + 1, y].A != 0)
                neighbours[value++] = colorMap[x + 1, y];
            if (colorMap[x - 1, y].A != 0)
                neighbours[value++] = colorMap[x - 1, y];
            if (colorMap[x, y + 1].A != 0)
                neighbours[value++] = colorMap[x, y + 1];
            if (colorMap[x, y - 1].A != 0)
                neighbours[value++] = colorMap[x, y + 1];

            var (a, r, g, b) = (0, 0, 0, 0);

            for (int i = 0; i < value; i++)
            {
                a += neighbours[i].A;
                r += neighbours[i].R;
                g += neighbours[i].G;
                b += neighbours[i].B;
            }

            if (a == 0)
                return Color.Black;

            return Color.FromArgb(a / value, r / value, g / value, b / value);
        }

        //private Color[,] coolficha(Color[,] cursedImage)
        //{                          
        //    Color[,] guessedImage = new Color[_image.Width, _image.Height];       !! раснос пикселей
        //        Color[,] cursedImage = CurseImage();

        //            for (int y = 0; y<_image.Height; y += _every) // расстановка пикселей
        //            {
        //                for (int x = 0; x<_image.Width; x += _every)
        //                {
        //                    guessedImage[x, y] = cursedImage[x / _every, y / _every];
        //                }
        //}                                                                         !!


        //    for (int y = 0; y < _image.Height; y += _every)                       !! если сделать Y основу получится прикольный эффект светящихся икср
        //    {
        //        for (int x = 0; x < _image.Width; x += _every)
        //        {
        //            if (x + _every < _image.Width)
        //            {
        //                for (int ghostyX = 1; ghostyX <= _every; ghostyX++)
        //                {
        //                    guessedImage[x + ghostyX, y] = ColorDiffuse(guessedImage[x + (ghostyX - 1), y], guessedImage[x + ((_every + 1) - ghostyX), y]);
        //                }
        //            }
        //            else
        //                break;
        //        }
        //    }

        //    private Color ColorDiffuse(Color left, Color right)
        //    {
        //        Random rand = new Random();

        //        int outR = (int)((left.R + right.R) / 2f);
        //        int outG = (int)((left.G + right.G) / 2f);
        //        int outB = (int)((left.B + right.B) / 2f);

        //        return Color.FromArgb(outR, outG, outB);
        //    }                                                                     !!


        //    return guessedImage;
        //}

        public void Dispose()
        {
            GC.Collect();
        }
    }
}