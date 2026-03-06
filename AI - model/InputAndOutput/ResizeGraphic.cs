using Aspose.Imaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using Color = System.Drawing.Color;
using Graphics = System.Drawing.Graphics;

namespace AI___model.InputAndOutput
{

    internal static class ResizeGraphic
    {
        public static void ResizeTo28x28AndSave(string inputPath, string outputPath, string name)
        {
            Bitmap original = new Bitmap(inputPath);
            Bitmap result = new Bitmap(28, 28);

            using (Graphics g = Graphics.FromImage(result))
            {
                g.Clear(Color.White);
                g.InterpolationMode = (System.Drawing.Drawing2D.InterpolationMode)InterpolationMode.HighQualityBicubic;

                float scaleX = 28f / original.Width;
                float scaleY = 28f / original.Height;
                float scale = Math.Min(scaleX, scaleY);

                int newWidth = (int)(original.Width * scale);
                int newHeight = (int)(original.Height * scale);

                int posX = (28 - newWidth) / 2;
                int posY = (28 - newHeight) / 2;

                g.DrawImage(original, posX, posY, newWidth, newHeight);
            }

            result.Save(Path.Combine(outputPath, name));
        }
    }
}
