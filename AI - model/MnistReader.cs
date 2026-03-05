using System;
using System.IO;

public class MnistReader
{
    public static (float[][] images, int rows, int cols) ReadImages(string path, int maxcount)
    {
        using var br = new BinaryReader(File.OpenRead(path));
        int magic = ReadInt32BigEndian(br);
        if (magic != 2051) throw new Exception("Error");
        int count = ReadInt32BigEndian(br);
        int rows = ReadInt32BigEndian(br);
        int cols = ReadInt32BigEndian(br);

        int n = Math.Min(count, maxcount);
        float[][] images = new float[n][];

        for (int i = 0; i < n; i++)
        {
            images[i] = new float[rows * cols];
            for (int j = 0; j < rows * cols; j++)
            {
                byte pixel = br.ReadByte();   
                images[i][j] = pixel / 255f;  
            }
        }

        return (images, rows, cols);
    }
    public static int[] ReadLabels(string path, int maxCount)
    {
        using var br = new BinaryReader(File.OpenRead(path));

        int magic = ReadInt32BigEndian(br);
        if (magic != 2049)
            throw new Exception("Error");

        int count = ReadInt32BigEndian(br);

        int n = Math.Min(count, maxCount);

        int[] labels = new int[n];

        for (int i = 0; i < n; i++)
        {
            labels[i] = br.ReadByte();
        }

        return labels;
    }
    static int ReadInt32BigEndian(BinaryReader br)
    {
        byte[] b = br.ReadBytes(4);
        if (b.Length < 4) throw new EndOfStreamException();

        return (b[0] << 24) | (b[1] << 16) | (b[2] << 8) | b[3];
    }
}
