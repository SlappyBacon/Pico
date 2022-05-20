using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Pico.Args;
using  Pico.Threads;

namespace Pico.Files
{
	public static class FileTools
    {


        public static string FileName(string filePath)
        {
            var slash = filePath.LastIndexOf('/');
            var bslash = filePath.LastIndexOf('\\');
            if (slash != -1 && slash > bslash) return filePath.Substring(slash + 1);
            if (bslash != -1 && bslash > slash) return filePath.Substring(bslash + 1);
            return "";
        }

        public static string[] GetAllFilePaths(string directory, bool inCludeSubDirs = false)
        {
            List<string> result = new List<string>(64);

            try
            {
                var filePaths = Directory.GetFiles(directory);
                foreach (var filePath in filePaths) result.Add(filePath);
                if (!inCludeSubDirs) return result.ToArray();
                var subDirs = Directory.GetDirectories(directory);
                foreach (var subDir in subDirs)
                {
                    var subDirFilePaths = GetAllFilePaths(subDir, true);
                    result.AddRange(subDirFilePaths);
                }
            }
            catch (Exception o)
            {
                Console.WriteLine(o.Message);
            }
            
            return result.ToArray();
        }

        public static bool TryCopy(string from, string to, bool overwrite)
        {
            try
            {
                File.Copy(from, to, overwrite);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void CopyAllFiles(string fromRootDirectory, string toRootDirectory, bool recursive = false, bool overwrite = false, int threadCount = 1)
        {

            //Add file extension filtering?  Input string[] of w/b listed extensions

            if (!Directory.Exists(fromRootDirectory)) return;
            var files = FileTools.GetAllFilePaths(fromRootDirectory, true);

            if (threadCount == 1) SingleThread();
            else MultiThread();




            void SingleThread()
            {
                //SINGLE THREAD
                // Copy all on this thread
                foreach (string file in files)
                {
                    string to = ChangeRoot(file);
                    string fileName = FileTools.FileName(file);
                    string toDir = to.Replace(fileName, "");
                    if (!Directory.Exists(toDir)) Directory.CreateDirectory(toDir);
                    FileTools.TryCopy(file, to, overwrite);
                }
            }
            void MultiThread()
            {
                //MULTITHREAD
                if (files.Length < threadCount) threadCount = files.Length;
                Thread[] threads = new Thread[threadCount];
                //Go through the list, assigning to threads
                for (int i = 0; i < files.Length;)
                {
                    string to = ChangeRoot(files[i]);
                    string toDir = to.Replace(FileTools.FileName(files[i]), "");
                    if (!Directory.Exists(toDir)) Directory.CreateDirectory(toDir);
                    //Could check if file already exists here, instead of after starting the new thread...
                    string ow;
                    if (overwrite) ow = "1";
                    else ow = "0";
                    string[] fromToOverwrite = new string[] { files[i], to, ow };
                    int freeThreadIndex;  //Wait for next available thread
                    while (!ThreadTools.FindFreeThread(threads, out freeThreadIndex)) Thread.Sleep(25);
                    threads[freeThreadIndex] = new Thread(CopyFileThreadAction);
                    threads[freeThreadIndex].Start(fromToOverwrite);   //Start next job on list
                    i++;
                }
                ThreadTools.JoinThreads(threads);
            }

            void CopyFileThreadAction(object fromToOverwrite)
            {
                string[] args = (string[])fromToOverwrite;
                string from = args[0];
                string to = args[1];
                bool overwrite = (args[2] == "1");
                FileTools.TryCopy(from, to, overwrite);
            }

            string ChangeRoot(string filePath)
            {
                string subFromRoot = filePath.Replace(fromRootDirectory, "");
                return toRootDirectory + subFromRoot;
            }
        }






        public static string FormatPath(string path)
        {
            path = path.Replace("\\", "/");
            while (path.Contains("//")) path = path.Replace("//", "/");
            return path;
        }













        //Woah there...  What about a string[] and a for()?
        //Way simpler, but oh well.
        static bool FileIsImage(string filePath)
        {
            string lower = filePath.ToLower();
            //Image file
            if (lower.EndsWith(".jpg")) return true;
            if (lower.EndsWith(".jpeg")) return true;
            if (lower.EndsWith(".jfif")) return true;
            if (lower.EndsWith(".exif")) return true;
            if (lower.EndsWith(".tif")) return true;
            if (lower.EndsWith(".tiff")) return true;
            if (lower.EndsWith(".gif")) return true;
            if (lower.EndsWith(".bmp")) return true;
            if (lower.EndsWith(".png")) return true;
            if (lower.EndsWith(".ppm")) return true;
            if (lower.EndsWith(".pgm")) return true;
            if (lower.EndsWith(".pbm")) return true;
            if (lower.EndsWith(".pnm")) return true;
            if (lower.EndsWith(".heif")) return true;
            if (lower.EndsWith(".bpg")) return true;
            if (lower.EndsWith(".deep")) return true;
            if (lower.EndsWith(".drw")) return true;
            if (lower.EndsWith(".ecw")) return true;
            if (lower.EndsWith(".fits")) return true;
            if (lower.EndsWith(".flif")) return true;
            if (lower.EndsWith(".ico")) return true;
            if (lower.EndsWith(".ilbm")) return true;
            if (lower.EndsWith(".img")) return true;
            if (lower.EndsWith(".nrrd")) return true;
            if (lower.EndsWith(".pam")) return true;
            if (lower.EndsWith(".pcx")) return true;
            if (lower.EndsWith(".pgf")) return true;
            if (lower.EndsWith(".plbm")) return true;
            if (lower.EndsWith(".sgi")) return true;
            if (lower.EndsWith(".sid")) return true;
            if (lower.EndsWith(".tga")) return true;
            if (lower.EndsWith(".xisf")) return true;

            //Editor files
            if (lower.EndsWith(".cd5")) return true;
            if (lower.EndsWith(".cpt")) return true;
            if (lower.EndsWith(".kra")) return true;
            if (lower.EndsWith(".mdp")) return true;
            if (lower.EndsWith(".pdn")) return true;
            if (lower.EndsWith(".psd")) return true;
            if (lower.EndsWith(".psp")) return true;
            if (lower.EndsWith(".sai")) return true;
            if (lower.EndsWith(".xcf")) return true;

            //Vector Images
            if (lower.EndsWith(".cgm")) return true;
            if (lower.EndsWith(".svg")) return true;
            if (lower.EndsWith(".cdr")) return true;

            //PDF
            if (lower.EndsWith(".pdf")) return true;

            return false;
        }
    }
}