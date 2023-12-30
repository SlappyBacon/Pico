using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Pico.Guid;

namespace Pico.Files;

/// <summary>
/// A collection of tools for 
/// saving, searching, and loading
/// objects to/from a disk.  Each
/// saved object gets assigned a GUID.
/// </summary>
class DiskItemCollection<T>
{
    string _rootDirectory;
    public string RootDirectory => _rootDirectory;
    const string FileExtention = ".json";

    public DiskItemCollection(string rootDirectory)
    {
        _rootDirectory = Path.GetFullPath(rootDirectory);
    }









    /// <summary>
    /// Adds object,
    /// then return its GUID.
    /// </summary>
    /// <param name="item">Object to add.</param>
    /// <returns></returns>
    public string Add(in T item)
    {
        var guid = GuidGenerator.Next();
        var saved = Save(guid, item);
        if (!saved) return null;
        return guid;
    }
    /// <summary>
    /// Deletes an object from the collection,
    /// using a given GUID.
    /// </summary>
    /// <param name="guid">GUID to use.</param>
    /// <returns></returns>
    public bool Delete(string guid)
    {
        var filePath = GetFilePath(guid);
        if (!File.Exists(filePath)) return false;

        File.Delete(filePath);
        return true;

    }
    




    /// <summary>
    /// Saves an object to the collection,
    /// using a given GUID.
    /// </summary>
    /// <param name="guid">GUID to assign object.</param>
    /// <param name="item">Object to save.</param>
    /// <returns></returns>
    public bool Save(string guid, in T item) => DiskItem.Save(GetFilePath(guid), item);
    
    /// <summary>
    /// Loads an object from the collection,
    /// using a given GUID.
    /// </summary>
    /// <param name="guid">GUID to search for.</param>
    /// <param name="item">Object loaded.</param>
    /// <returns></returns>
    public bool Load(string guid, out T item) => DiskItem.Load(GetFilePath(guid), out item);

    /// <summary>
    /// Returns if guid exists within collection.
    /// </summary>
    /// <param name="guid">GUID to search for.</param>
    /// <returns></returns>
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
