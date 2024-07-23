using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
namespace Pico.Files;

/// <summary>
/// A collection of tools for 
/// saving, searching, and loading
/// objects to/from a disk.  Each object
/// must be returned back to the library
/// before being borrowed again.
/// </summary>
class DiskItemLibrary<T> : IDisposable
{
    object _padlock = new object();
    DiskItemCollection<T> _collection;
    List<string> _borrowedGuids;
    DiskItemCollection<T> Collection => _collection;
    List<string> BorrowedGuids => _borrowedGuids;

    bool _isDisposing = false;
    public bool IsDisposing => _isDisposing;

    public string RootDirectory => Collection.RootDirectory;




    /// <summary>
    /// Creates a new instance within a directory.
    /// </summary>
    /// <param name="rootDirectory">Directory.</param>
    public DiskItemLibrary(string rootDirectory)
    {
        _collection = new DiskItemCollection<T>(rootDirectory);
        _borrowedGuids = new List<string>();
    }
    /// <summary>
    /// Adds object with random GUID,
    /// then return its GUID.
    /// </summary>
    /// <param name="item">Object to add.</param>
    /// <returns></returns>
    public string Add(in T item)
    {
        if (IsDisposing) return null;
        lock (_padlock)
        {
            return Collection.Add(item);
        }
    }
    /// <summary>
    /// Overwrites existing object,
    /// or creates a new one.
    /// </summary>
    /// <param name="item">Object to add.</param>
    /// <param name="guid">Manually set guid.</param>
    /// <returns></returns>
    public bool OverWrite(in T item, string guid)
    {
        if (IsDisposing) return false;
        lock (_padlock)
        {
            return Collection.OverWrite(item, guid);
        }
    }
    /// <summary>
    /// Deletes object.
    /// </summary>
    /// <param name="guid">GUID to search for.</param>
    /// <returns></returns>
    public bool TryDelete(string guid)
    {
        if (IsDisposing) return false;
        lock (_padlock)
        {
            if (IsBorrowed(guid)) return false;
            return Collection.Delete(guid);
        }
    }
    
    
    
    
    
    
    /// <summary>
    /// Copies object, without borrowing the original.
    /// </summary>
    /// <param name="guid">GUID to search for.</param>
    /// <param name="item">Object copied.</param>
    /// <returns></returns>
    public bool TryCopy(string guid, out T item)
    {
        //Does not "borrow" so you don't need to return the copy.
        if (IsDisposing)
        {
            item = default;
            return false;
        }
        lock (_padlock)
        {
            if (IsBorrowed(guid))
            {
                item = default;
                return false;
            }

            var loaded = Collection.Load(guid, out item);
            if (!loaded) return false;

            return true;
        }
    }

    /// <summary>
    /// Copies object, without borrowing the original.
    /// NOTE: Will load last saved state, even if the
    /// object is currently borrowed, being modified, ect...
    /// </summary>
    /// <param name="guid">GUID to search for.</param>
    /// <param name="item">Object copied.</param>
    /// <returns></returns>
    public bool TryCopyDirty(string guid, out T item)
    {
        //Does not "borrow" so you don't need to return the copy.
        if (IsDisposing)
        {
            item = default;
            return false;
        }
        lock (_padlock)
        {
            var loaded = Collection.Load(guid, out item);
            if (!loaded) return false;

            return true;
        }
    }







    /// <summary>
    /// Borrows object.
    /// </summary>
    /// <param name="guid">GUID to search for.</param>
    /// <param name="item">Object borrowed.</param>
    /// <returns></returns>
    public bool TryBorrow(string guid, out T item)
    {
        if (IsDisposing)
        {
            item = default;
            return false;
        }
        lock (_padlock)
        {
            if (IsBorrowed(guid))
            {
                item = default;
                return false;
            }

            var loaded = Collection.Load(guid, out item);
            if (!loaded) return false;

            BorrowedGuids.Add(guid);

            return true;
        }
    }
    /// <summary>
    /// Returns object.
    /// </summary>
    /// <param name="guid">GUID to search for.</param>
    /// <param name="item">Object to return.</param>
    /// <returns></returns>
    public bool TryReturn(string guid, in T item)
    {
        lock (_padlock)
        {
            if (!IsBorrowed(guid))
            {
                return false;
            }

            var saved = Collection.Save(guid, item);
            if (!saved) return false;

            BorrowedGuids.Remove(guid);

            return true;
        } 
    }
    /// <summary>
    /// Returns if object with GUID exists within collection.
    /// </summary>
    /// <param name="guid">GUID to search for.</param>
    /// <returns></returns>
    
    
    
    
    
    public bool GuidExists(string guid)
    {
        lock (_padlock)
        {
            return Collection.GuidExists(guid);
        }
    }
    /// <summary>
    /// Returns if object with GUID is currently borrowed.
    /// </summary>
    /// <param name="guid">GUID to search for.</param>
    /// <returns></returns>
    public bool IsBorrowed(string guid)
    {
        lock (_padlock)
        {
            return BorrowedGuids.Contains(guid);
        }
    }





    /// <summary>
    /// Waits for all objects to be returned,
    /// then frees memory.
    /// </summary>
    public void Dispose()
    {
        _isDisposing = true;
        while (true)
        {
            if (BorrowedGuids.Count < 1) break;
            Thread.Sleep(1);
        }
    }
}
