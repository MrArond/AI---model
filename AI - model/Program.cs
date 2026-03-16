using AI___model.InputAndOutput;
using Aspose.Imaging;
using Aspose.Imaging.FileFormats.Cdr.Objects;
using System;
using System.Data;
using System.Reflection;
using AI___model.PreProcessing.Readers;

class Program
{
    static void Main()
    {
        AI___model.Console.Console.Run();

        


        // string imagesPath = @"C:\Users\Jakub\source\repos\MrArond\AI---model\AI - model\Dataset\Images\t10k-images.idx3-ubyte";
        // string labelsPath = @"C:\Users\Jakub\source\repos\MrArond\AI---model\AI - model\Dataset\Images\t10k-labels.idx1-ubyte";
        //
        // int n = 10;
        //
        // var (images, rows, cols) = MnistReader.ReadImages(imagesPath, n);
        // var labels = MnistReader.ReadLabels(labelsPath, n);
        //
        // Console.WriteLine($"rows={rows}, cols={cols}");
        // Console.WriteLine($"images={images.Length}, labels={labels.Length}");
        // Console.WriteLine();
        //
        // for (int i = 0; i < n; i++)
        // {
        //     Console.WriteLine($"#{i}  label={labels[i]}");
        //     PrintImageAscii(images[i], rows, cols, threshold: 0.35f);
        //     Console.WriteLine(new string('-', 40));
        // }
        //
        // string PathObject = "C:\\Users\\Jakub\\source\\repos\\MrArond\\AI---model\\AI - model\\Dataset\\TestObject\\cos.png";
        // string PathToSecondOBject = "C:\\Users\\Jakub\\source\\repos\\MrArond\\AI---model\\AI - model\\Dataset\\TestObject\\CosTestowe1.png";
        // string NameObject = "CosTestowe1.png";
        // string NameObject2 = "CosTestowe2.png";
        // string PathToFolder = "C:\\Users\\Jakub\\source\\repos\\MrArond\\AI---model\\AI - model\\Dataset\\TestObject";
        //
        // ResizeGraphic.ResizeTo28x28AndSave(PathObject, PathToFolder, NameObject);
        // ChangeColorOfObject.ChangeColor(PathToSecondOBject, PathToFolder, NameObject2);
        //
        // Console.WriteLine("Koniec.");

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


/*
 * 1. Dane z MNIST
 * 2. Dane z png
 *  - iterowanie po folderze z png
 *  - zmiana rozmiaru do 28x28
 *  - zmiana kolorów do odcieni szarości
 *
 * foldery
 * \Input(nazwa podawana przy uruchomieniu programu)
 * \Input\Temp (tutaj będą zapisywane przetworzone obrazy)
 * \Input\Output (tutaj będą zapisywane wyniki działania programu)
 *
 * dla wielu obrazów raport w formie csv lub html z wyświetleniem obrazka z nazwą pliku i przewidywaną cyfrą
*/