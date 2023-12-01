using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Pico.Guid;

namespace Pico.Files;

class DiskItemCollection<T>
{
    string _rootDirectory;
    public string RootDirectory => _rootDirectory;
    const string FileExtention = ".json";

    public DiskItemCollection(string rootDirectory)
    {
        _rootDirectory = Path.GetFullPath(rootDirectory);
    }
    public string Add(in T item)
    {
        var guid = GuidGenerator.Next();
        var saved = Save(guid, item);
        if (!saved) return null;
        return guid;
    }
    public bool Save(string guid, in T item) => DiskItem.Save(GetFilePath(guid), item);
    public bool Load(string guid, out T item) => DiskItem.Load(GetFilePath(guid), out item);
    public bool Delete(string guid)
    {
        var filePath = GetFilePath(guid);
        if (!File.Exists(filePath)) return false;

        File.Delete(filePath);
        return true;

    }

    public bool LoadFind(Func<T, bool> determinant, out T item) => DiskItem.LoadFind(RootDirectory, determinant, out item);
    public List<T> LoadFindAll(Func<T, bool> determinant) => DiskItem.LoadFindAll(RootDirectory, determinant);

    public bool GuidExists(string guid)
    {
        var filePath = GetFilePath(guid);
        return File.Exists(filePath);
    }
    string GetFilePath(string guid)
    {
        //New StringBuilder (root.length + guid.length + extention.length)
        var builder = new StringBuilder(RootDirectory.Length + guid.Length * FileExtention.Length);
        builder.Append(RootDirectory);
        builder.Append(guid);
        builder.Append(FileExtention);
        return builder.ToString();
    }
}
