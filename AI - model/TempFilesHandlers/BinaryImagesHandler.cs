namespace AI___model.TempFilesHandlers;

public class BinaryImagesHandler
{
    public bool Save(string path, List<byte[]> bytes)
    {
        try
        {
            using var bw = new BinaryWriter(File.OpenWrite(path));
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