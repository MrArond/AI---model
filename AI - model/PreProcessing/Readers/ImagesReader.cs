using System.Drawing;
using System.Drawing.Drawing2D;

namespace AI___model.PreProcessing.Readers;

public class ImagesReader
{
    public List<byte[]> ReadData(string path)
    {
        var bc = new AI___model.PreProcessing.Converters.BitConverter();
        var data = new List<byte[]>();
        var files = Directory.GetFiles(path);
        foreach (var file in files)
        {
            if (!file.Contains(".png"))
                continue;
            var resized = ResizeImage(file);
            var bw = ConvertToBlackAndWhite(resized);

            var bits = new bool[28 * 28];
            for (int y = 0; y < bw.Height; y++)
            {
                for (int x = 0; x < bw.Width; x++)
                {
                    var pixel = bw.GetPixel(x, y);
                    var gray = (pixel.R + pixel.G + pixel.B) / 3;
                    var ibw = gray > 128;
                    bits[y * 28 + x] = ibw;
                }
            }
            var bytes = bc.GetBytes(bits);
            data.Add(bytes);
        }

        return data;
    }
    
    private Bitmap ResizeImage(string path)
    {
        var original = new Bitmap(path);
        var result = new Bitmap(28, 28);

        using var g = Graphics.FromImage(result);
        g.Clear(Color.White);
        g.InterpolationMode = (System.Drawing.Drawing2D.InterpolationMode)InterpolationMode.HighQualityBicubic;

        var scaleX = 28f / original.Width;
        var scaleY = 28f / original.Height;
        var scale = Math.Min(scaleX, scaleY);

        var newWidth = (int)(original.Width * scale);
        var newHeight = (int)(original.Height * scale);

        var posX = (28 - newWidth) / 2;
        var posY = (28 - newHeight) / 2;

        g.DrawImage(original, posX, posY, newWidth, newHeight);

        return result;
    }
    
    private Bitmap ConvertToBlackAndWhite(Bitmap img)
    {
        var result = new Bitmap(img.Width, img.Height);

        for (int y = 0; y < img.Height; y++)
        {
            for (int x = 0; x < img.Width; x++)
            {
                var pixel = img.GetPixel(x, y);
                var gray = (pixel.R + pixel.G + pixel.B) / 3;
                var bw = gray > 128 ? 0 : 1;
                var newColor = bw == 1 ? Color.White : Color.Black;
                result.SetPixel(x, y, newColor);
            }
        }

        return result;
    }
}