using System;
using System.Collections.Generic;
using System.Threading;
namespace Pico.Files;
class DiskItemLibrary<T> : IDisposable
{
    object _padlock = new object();
    DiskItemCollection<T> _collection;
    List<string> _borrowedGuids;
    DiskItemCollection<T> Collection => _collection;
    List<string> BorrowedGuids => _borrowedGuids;

    bool _isDisposing = false;
    public bool IsDisposing => _isDisposing;

    public DiskItemLibrary(string rootDirectory)
    {
        _collection = new DiskItemCollection<T>(rootDirectory);
        _borrowedGuids = new List<string>();
    }

    public string Add(in T item)
    {
        if (IsDisposing) return null;
        lock (_padlock)
        {
            return Collection.Add(item);
        }
    }
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
    public bool TryDelete(string guid)
    {
        if (IsDisposing) return false;
        lock (_padlock)
        {
            if (IsBorrowed(guid)) return false;
            return Collection.Delete(guid);
        }
    }
    public bool GuidExists(string guid)
    {
        lock (_padlock)
        {
            return Collection.GuidExists(guid);
        }
    }
    public bool IsBorrowed(string guid)
    {
        lock (_padlock)
        {
            return BorrowedGuids.Contains(guid);
        }
    }

    
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
