using AI___model.PreProcessing.Readers;
using AI___model.TempFilesHandlers;

namespace AI___model.Console.Views;

public class PreProcessingView : IConsoleView
{
    public void ShowOnAppear()
    {
        var size = System.Console.WindowHeight;
        
        System.Console.WriteLine("Welcome to Digit Recognizer!\n");
        System.Console.WriteLine("This is preprocessing view.\n Here you can prepare your data for training and testing the model.\n");
        
        
        System.Console.WriteLine("1. Prepare data from raw images");
        System.Console.WriteLine("2. Test model with MNIST data");
        System.Console.WriteLine("3. Load data from temp files");

        System.Console.CursorTop = size - 1;
        
        System.Console.Write("> ");
    }

    public void HandleInput(string input)
    {
        switch (input)
        {
            case "1":
                System.Console.WriteLine("Preparing data from raw images...");
                PrepareDataFromRawImages();
                break;
            case "2":
                System.Console.WriteLine("Converting data from MNIST dataset...");
                var ok = Mnist();
                break;
            case "3":
                System.Console.WriteLine("Loading data from temp files...");
                break;
            default:
                System.Console.WriteLine("Invalid input");
                break;
        }
    }


    private bool Mnist()
    {
        var mr = new MnistReader();
        
        var bytes = mr.ReadDataSet();
        System.Console.WriteLine($"Read {bytes.Count} images from MNIST dataset. \n Saving to temp files...");
        
        var bih = new BinaryImagesHandler();
        
        var ok = bih.Save(Path.Combine(Config.MnistPath, Config.TempPath), "mnist.bin", bytes);
        Console.ChangeView(new MainView());
        
        return ok;
    }
    
    private void PrepareDataFromRawImages()
    {
        var size = System.Console.WindowHeight;
        System.Console.Clear();
        System.Console.WriteLine("Enter path to raw images:");
        
        System.Console.CursorTop = size - 1;
        
        System.Console.Write("> ");
        
        var path = System.Console.ReadLine();
        
        var ir = new ImagesReader();
        var data = ir.ReadData(path);
        System.Console.WriteLine($"Read {data.Count} images from raw images. \n Saving to temp files...");
        
        var bih = new BinaryImagesHandler();
        
        var ok = bih.Save(Path.Combine(path, Config.TempPath), "images.bin", data);
        Console.ChangeView(new MainView());
    }
}