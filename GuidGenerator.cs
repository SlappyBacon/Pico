using System.Text;

namespace Pico.Guid;

public static class GuidGenerator
{
    static object _lock = new object();

    const string DefaultSamples = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";



    static byte _size;
    public static byte Size
    {
        get { return _size; }
        set
        {
            //Size must be greater/equal to 4 characters
            if (value < 4) value = 4;
            if (value == Size) return;
            _size = value;
            ConstructBuilder();
        }
    }

    static StringBuilder _builder;
    static StringBuilder Builder => _builder;

    static GuidGenerator()
    {
        Size = 32;
    }

    static void ConstructBuilder()
    {
        lock (_lock)
        {
            _builder = new StringBuilder(Size);
        }
    }

    public static string Next(string sampleString = null)
    {
        lock (_lock)
        {
            //Determine samples
            if (sampleString == null) sampleString = DefaultSamples;

            //Generate
            Builder.Clear();
            for (int i = 0; i < Size; i++)
            {
                var rng = Random.Shared.Next(0, sampleString.Length);
                Builder.Append(sampleString[rng]);
            }
            return Builder.ToString();
        }
    }
}
