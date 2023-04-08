using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pico.Files;

class DiskBuffer<T>
{
    string rootDirectory;        //Directory where accounts are saved and loaded

    //Update last used accounts any time one gets found from the disc, or writted to disc.
    int totalFiles;
    public int TotalFiles { get { return totalFiles; } }

    long nextId;
    (long, T)[] recentlyConfirmed;    //Updated each time a Disc search match is found.  To reduce repeat time.
    long[] recentlyDenied;        //Updated each time a search fails. (Memory and Disc.)

    public DiskBuffer(string rootDirectory)
    {
        this.rootDirectory = rootDirectory;
        if (!Directory.Exists(rootDirectory)) Directory.CreateDirectory(rootDirectory);

        nextId = GetNextIdFromDisc();
        
        recentlyConfirmed = new (long, T)[100];
        
        recentlyDenied = new long[1000];
    }

    long GetNextIdFromDisc()
    {
        var allPaths = Directory.GetFiles(rootDirectory);
        totalFiles = allPaths.Length;

        long highestId = -1;
        foreach (var path in allPaths)
        {
            var fileId = FileId(path);
            
            if (fileId > highestId) highestId = fileId;
        }
        return highestId + 1;   //If no files, 0 is returned.

        //Multithread this process for large files?
        //Lock count, compare, iterate?, unlock
    }

    //Return the ID of newThing
    public long Add(T newThing)
    {
        var touple = (nextId++, newThing);
        var saved = SaveToFile(touple);
        if (!saved) return -1;
        totalFiles++;
        TryRemoveFromRecentlyDenied(touple.Item1);
        UpdateRecentlyConfirmed(touple);
        return touple.Item1;
    }
    public bool Overwrite(long id, T newThing)
    {
        if (!IdIsValid(id)) return false;
        (long, T) touple = (id, newThing);
        var saved = SaveToFile(touple);
        if (!saved) return false;
        TryRemoveFromRecentlyDenied(touple.Item1);
        UpdateRecentlyConfirmed(touple);
        return true;
    }
    public bool Delete(long id)
    {
        if (!IdIsValid(id)) return false;
        var path = FilePath(id);
        if (!File.Exists(path)) return false;
        totalFiles--;
        File.Delete(path);
        TryRemoveFromRecentlyConfirmed(id);
        UpdateRecentlyDenied(id);
        return true;
    }




    public T Find(long id)
    {
        if (!IdIsValid(id)) return default(T);   //Invalid ID

        //Check recently used accounts first (in memory)

        var d = IsRecentlyDenied(id);
        if (d > -1) return default(T);


        var c = IsRecentlyConfirmed(id);
        if (c > -1) return recentlyConfirmed[c].Item2;

        //Search disc

        T onDisc = FindFromDisc(id);   //Can be null, if not found.

        if (onDisc == null) UpdateRecentlyDenied(id);

        return onDisc;
    }
    T FindFromDisc(long id)
    {
        var accountDataFilePaths = Directory.GetFiles(rootDirectory);

        foreach (var path in accountDataFilePaths)
        {
            var fileId = FileId(path);
            if (id != fileId) continue;     //File name is not Account ID (SHOULD BE)

            T fromFile = LoadFromFile(path);

            UpdateRecentlyConfirmed((id, fromFile));
            return fromFile;
        }

        return default(T);
    }
    


    bool IdIsValid(long id) => id > 0 && id < nextId;
    
    int IsRecentlyConfirmed(long id)
    {
        for (int i = 0; i < recentlyConfirmed.Length; i++)
        {
            if (id != recentlyConfirmed[i].Item1) continue;
            return i;    //ID Match
        }
        return -1;
    }
    int IsRecentlyDenied(long id)
    {
        for (int i = 0; i < recentlyDenied.Length; i++)
        {
            if (id != recentlyDenied[i]) continue;
            return i;    //ID Match
        }
        return -1;
    }

    void UpdateRecentlyConfirmed((long, T) touple)
    {
        //Cycle
        for (int i = recentlyConfirmed.Length - 1; i > 0; i--)
        {
            recentlyConfirmed[i] = recentlyConfirmed[i - 1];
        }

        //New is now entry[0]
        recentlyConfirmed[0] = touple;
    }
    void UpdateRecentlyDenied(long id)
    {
        //Cycle
        for (int i = recentlyDenied.Length - 1; i > 0; i--)
        {
            recentlyDenied[i] = recentlyDenied[i - 1];
        }

        //New is now entry[0]
        recentlyDenied[0] = id;
    }


    bool TryRemoveFromRecentlyConfirmed(long id)
    {
        for (int i = 0; i < recentlyConfirmed.Length; i++)
        {
            if (id != recentlyConfirmed[i].Item1) continue;
            recentlyConfirmed[i] = (default(long), default(T));
            return true;
        }
        return false;
    }
    bool TryRemoveFromRecentlyDenied(long id)
    {
        for (int i = 0; i < recentlyDenied.Length; i++)
        {
            if (id != recentlyDenied[i]) continue;
            recentlyDenied[i] = default(long);
            return true;
        }
        return false;
    }

    bool SaveToFile((long, T) touple)
    {
        //Serialize
        var json = JsonConvert.SerializeObject(touple.Item2);

        //Write
        File.WriteAllText(FilePath(touple.Item1), json);

        return true;
    }

    T LoadFromFile(string path)
    {
        //Read
        var fileText = File.ReadAllText(path);
        if (fileText == null) return default(T);

        //Deserialize
        return JsonConvert.DeserializeObject<T>(fileText);
    }

    string FilePath(long id) => $@"{rootDirectory}{id}.dbo";
    long FileId(string path)
    {
        string stringId = Path.GetFileName(path);
        var dot = stringId.IndexOf('.');
        stringId = stringId.Substring(0, dot);
        return long.Parse(stringId);
    }
}
