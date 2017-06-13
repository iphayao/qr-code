using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace qr_code
{
    class Program
    {
        static void Main(string[] args)
        {
            const int MODULE_SIZE_START = 21;
            const int MODULE_STEP_COUNT = 4;

            int version = 7;

            // Calculate group of version
            int group = (version % 7 == 0) ? version / 7 : (version - version % 7) / 7;
            // Calculate no. of module/ size (A)
            int modules_size = (version - 1) * MODULE_STEP_COUNT + MODULE_SIZE_START;
            // Calculate function pattern module (B)
            int modules_pattern = ((modules_size - 8 * 2 - 5 * group) * 2) + ((int)(Math.Pow((group + 1), 2) + group * 2) * 25) + ((int)(Math.Pow(8, 2) * 3)) - ((version > 1) ? 0 : 25);

            int ms = modules_size;
            int[,] patterns = new int[modules_size, modules_size];
            int[,] modules = new int[modules_size, modules_size];

            // Position pattern
            for (int i = 0; i < modules_size; i++)
            {
                for (int j = 0; j < modules_size; j++)
                {
                    if (i == 0 || i == 6)
                    {
                        patterns[i, j] = (j < 7 || j >= ms - 7) ? 1 : 0;
                    }
                    else if (i == ms - 7 || i == ms - 1)
                    {
                        patterns[i, j] = (j < 7) ? 1 : 0;
                    }
                    else if (i == 1 || i == 5)
                    {
                        patterns[i, j] = ((j == 0 || j == 6) || (j == ms - 7 || j == ms - 1)) ? 1 : 0;
                    }
                    else if (i == ms - 6 || i == ms - 2)
                    {
                        patterns[i, j] = (j == 0 || j == 6) ? 1 : 0;
                    }
                    else if (i >= 2 && i <= 4)
                    {
                        patterns[i, j] = ((j == 0 || j == 6 || (j >= 2 && j <= 4)) || (j == ms - 7 || j == ms - 1 || (j >= ms - 5 && j <= ms - 3))) ? 1 : 0;
                    }
                    else if (i >= ms - 5 && i <= ms - 3)
                    {
                        patterns[i, j] = (j == 0 || j == 6 || (j >= 2 && j <= 4)) ? 1 : 0;
                    }
                }
            }

            // Alignment pattern
            int aligment = ((((modules_size - 6) - 6) - 1) / (group + 1));//(version + 1) * 2;
            for (int i = 6; i < modules_size; i += aligment)
            {
                for (int j = 6; j < modules_size; j += aligment)
                {
                    if (patterns[i, j] != 1)
                    {
                        for (int ii = -2; ii <= 2; ii++)
                        {
                            for (int jj = -2; jj <= 2; jj++)
                            {
                                patterns[i - ii, j - jj] = ((Math.Abs(ii) == 2) || (Math.Abs(jj) == 2)) ? 1 : 0;
                            }
                        }
                        patterns[i, j] = 1;
                    }
                }
            }

            // Timing pattern
            for (int i = 6; i < modules_size - 8; i += 2)
            {
                if (i == 6)
                {
                    for (int j = 8; j < modules_size - 8; j += 2)
                    {
                        patterns[i, j] = 1;
                    }
                }
                patterns[i, 6] = 1;
            }

            // End created pattern

            // Copy patterns to modules
            for (int i = 0; i < modules_size; i++)
            {
                for(int j = 0; j < modules_size; j++)
                {
                    modules[i, j] = patterns[i, j];
                }
            }

            int x, y;
            int m, n;
            m = 0;
            n = 0;
            int scale = 10;

            Bitmap img = new Bitmap(modules_size * scale, modules_size * scale);

            for (x = 0; x < modules_size * scale; x++)
            {
                for (y = 0, n = 0; y < modules_size * scale; y++)
                {
                    //int rgb = 255;
                    img.SetPixel(x, y, (modules[n, m] == 1) ? Color.Black : Color.White);
                    if ((y + 1) % scale == 0) n++;
                }
                if ((x + 1) % scale == 0) m++;
            }

            img.Save("qrcode.jpg", ImageFormat.Jpeg);

        }
    }
}
