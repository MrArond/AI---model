using System;
using System.IO;
using AI___model;

public class MnistReader
{
    // public static (float[][] images, int rows, int cols) ReadImages(string path, int maxcount)
    // {
    //     using var br = new BinaryReader(File.OpenRead(path));
    //     int magic = ReadInt32BigEndian(br);
    //     if (magic != 2051) throw new Exception("Error");
    //     int count = ReadInt32BigEndian(br);
    //     int rows = ReadInt32BigEndian(br);
    //     int cols = ReadInt32BigEndian(br);
    //
    //     int n = Math.Min(count, maxcount);
    //     float[][] images = new float[n][];
    //
    //     for (int i = 0; i < n; i++)
    //     {
    //         images[i] = new float[rows * cols];
    //         for (int j = 0; j < rows * cols; j++)
    //         {
    //             byte pixel = br.ReadByte();   
    //             images[i][j] = pixel / 255f;  
    //         }
    //     }
    //
    //     return (images, rows, cols);
    // }



    public List<byte[]> ReadDataSet()
    {
        using var br = new BinaryReader(File.OpenRead(Path.Combine(Config.MnistPath, Config.MnistFile)));
        var magic = ReadInt32BigEndian(br);
        if (magic != 2051) throw new Exception("Error");
        var count = ReadInt32BigEndian(br);
        //var rows = ReadInt32BigEndian(br);
        //var cols = ReadInt32BigEndian(br);
        const int size = 28;

        var bc = new AI___model.PreProcessing.Converters.BitConverter();
        
        var images = new List<byte[]>();

        var bits = new bool[size*size];
        
        for (var i = 0; i < count; i++)
        {
            
            for (var j = 0; j < size * size; j++)
            {
                var pixel = br.ReadByte();
                if (pixel > Config.Threshold)
                {
                    bits[j] = true;
                }
                else
                {
                    bits[j] = false;
                }
            }

            var bytes = bc.GetBytes(bits);
            images.Add(bytes);
        }

        return images;
    }






    // public static int[] ReadLabels(string path, int maxCount)
    // {
    //     using var br = new BinaryReader(File.OpenRead(path));
    //
    //     int magic = ReadInt32BigEndian(br);
    //     if (magic != 2049)
    //         throw new Exception("Error");
    //
    //     int count = ReadInt32BigEndian(br);
    //
    //     int n = Math.Min(count, maxCount);
    //
    //     int[] labels = new int[n];
    //
    //     for (int i = 0; i < n; i++)
    //     {
    //         labels[i] = br.ReadByte();
    //     }
    //
    //     return labels;
    // }
    private int ReadInt32BigEndian(BinaryReader br)
    {
        byte[] b = br.ReadBytes(4);
        if (b.Length < 4) throw new EndOfStreamException();

        return (b[0] << 24) | (b[1] << 16) | (b[2] << 8) | b[3];
    }
}
