using Aspose.Imaging.FileFormats.Bmp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace AI___model.InputAndOutput
{
    internal static class ChangeColorOfObject
    {
        public static void ChangeColor(string path, string outputPath, string ObjectBack)
        {
            Bitmap img = new Bitmap(path);
            Bitmap result = new Bitmap(img.Width, img.Height);

            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    Color pixel = img.GetPixel(x, y);

                    int gray = (pixel.R + pixel.G + pixel.B) / 3;

                    int bw = gray > 128 ? 0 : 1;

                    Color newColor = bw == 1 ? Color.White : Color.Black;

                    result.SetPixel(x, y, newColor);
                }
            }

            result.Save(Path.Combine(outputPath, ObjectBack));
        }
    }
}