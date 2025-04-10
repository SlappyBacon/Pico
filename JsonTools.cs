using Newtonsoft.Json;

namespace Pico.Json;

public static class JsonTools
{
    public static void Print<T>(T obj)
    {
        var json = JsonConvert.SerializeObject(obj, Formatting.Indented);
        Console.WriteLine(json);
    }
    public static void Rebuild<T>(ref T obj)
    {
        var json = JsonConvert.SerializeObject(obj);
        var disposeMe = obj as IDisposable;
        disposeMe?.Dispose();
        obj = JsonConvert.DeserializeObject<T>(json);
    }
}
