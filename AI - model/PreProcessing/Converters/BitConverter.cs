namespace AI___model.PreProcessing.Converters;

public class BitConverter
{
    public byte[] GetBytes(bool[] bits)
    {
        var byteCount = (bits.Length + 7) / 8;
        var bytes = new byte[byteCount];

        for (var i = 0; i < bits.Length; i++)
        {
            if (bits[i])
            {
                bytes[i / 8] |= (byte)(1 << (i % 8));
            }
        }

        return bytes;
    }

    public bool[] GetBits(byte[] bytes)
    {
        return (bool[])bytes.Clone();
    }
}