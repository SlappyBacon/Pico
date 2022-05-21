using System.Collections.Generic;
using System.IO;

namespace Pico.Files
{
    public class RunFinder
    {

        //Finds all executables of the following types
        //within a directory

        //////////////////////////////
        //Generic rework => FileFinder
        //Change to scan every folder for a certain file type
        //Then return the list...
        //////////////////////////////
        
        public Dictionary<string,string> exes = new Dictionary<string,string>(1000); //.exe files (Windows Executable)
        public Dictionary<string,string> bats = new Dictionary<string,string>(1000); //.bat files (Batch Script)
        public Dictionary<string,string> ps1s = new Dictionary<string,string>(1000); //.pl1 files (PowerShell Script)
        public Dictionary<string,string> shs = new Dictionary<string,string>(1000);  //.sh files (Bash Script)
        public Dictionary<string,string> pys = new Dictionary<string,string>(1000);  //.py files (Python Script)

        public void Update(string root)
        {
            ClearAll();
            ProcessDirectory(root);
        }





        void ProcessDirectory(string dir)
        {
            AddExecutables(dir);
            try
            {
                var subDirs = Directory.GetDirectories(dir);
                foreach (var subDir in subDirs) ProcessDirectory(subDir);
            }
            catch { }
        }



         void AddExecutables(string dir)
         {
            try
            {
                var files = Directory.GetFiles(dir);
                //process each file
                foreach (var file in files)
                {
                    //Get ext
                    var lastDot = file.LastIndexOf('.');
                    if (lastDot == -1) continue;
                    var ext = file.Substring(lastDot + 1);


                    //Get name
                    string name = null;
                    var lastSlash = file.LastIndexOf('/');
                    if (lastSlash != -1)
                    {
                        name = file.Substring(lastSlash + 1);
                    }
                    var lastBackSlash = file.LastIndexOf('\\');
                    if (lastBackSlash != -1)
                    {
                        var maybeName = file.Substring(lastBackSlash + 1);
                        if (name == null || maybeName.Length < name.Length) name = maybeName;
                    }

                    switch (ext)
                    {
                        case "exe":
                            exes[name] = file;
                            break;
                        case "bat":
                            bats[name] = file;
                            break;
                        case "ps1":
                            ps1s[name] = file;
                            break;
                        case "sh":
                            shs[name] = file;
                            break;
                        case "py":
                            pys[name] = file;
                            break;
                        default:
                            //Do nothing
                            break;
                    }
                }
            }
            catch { }
        }



        


        void ClearAll()
        {
            exes.Clear();
            bats.Clear();
            ps1s.Clear();
            shs.Clear();
            pys.Clear();
        }

    }
}
