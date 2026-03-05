using System;

class Program
{
    static void Main()
    {
        string imagesPath = @"C:\Users\jakub\source\repos\AI - model\AI - model\Dataset\Images\t10k-images.idx3-ubyte";
        string labelsPath = @"C:\Users\jakub\source\repos\AI - model\AI - model\Dataset\Images\t10k-labels.idx1-ubyte";

        int n = 10;

        var (images, rows, cols) = MnistReader.ReadImages(imagesPath, n);
        var labels = MnistReader.ReadLabels(labelsPath, n);

        Console.WriteLine($"rows={rows}, cols={cols}");
        Console.WriteLine($"images={images.Length}, labels={labels.Length}");
        Console.WriteLine();

        for (int i = 0; i < n; i++)
        {
            Console.WriteLine($"#{i}  label={labels[i]}");
            PrintImageAscii(images[i], rows, cols, threshold: 0.35f);
            Console.WriteLine(new string('-', 40));
        }

        Console.WriteLine("Koniec.");
        Console.ReadKey();
    }

    static void PrintImageAscii(float[] img, int rows, int cols, float threshold)
    {
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                float v = img[r * cols + c]; 
                Console.Write(v > threshold ? '#' : ' ');
            }
            Console.WriteLine();
        }
    }
}