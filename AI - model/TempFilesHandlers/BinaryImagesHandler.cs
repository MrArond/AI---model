namespace AI___model.TempFilesHandlers;

public class BinaryImagesHandler
{
    public bool Save(string path, string name, List<byte[]> bytes)
    {
        try
        {
            Directory.CreateDirectory(path);
            using var bw = new BinaryWriter(File.OpenWrite(Path.Combine(path, name)));
            foreach (var byteArray in bytes)
            {
                bw.Write(byteArray);
            }
            return true;
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e);
            return false;
        }
    }
    
    public List<bool[,]> Load(string name)
    {
        return null;
    }
}