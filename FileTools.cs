using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Pico.Args;
using  Pico.Threads;

namespace Pico.Files
{
    /// <summary>
    /// A collection of abstract file tools.
    /// </summary>
	public static class FileTools
    {

        /// <summary>
        /// Input file path, returns the name of the file.
        /// </summary>
        /// <param name="filePath">File path.</param>
        /// <returns>File name.</returns>
        public static string FileName(string filePath)
        {
            var slash = filePath.LastIndexOf('/');
            var bslash = filePath.LastIndexOf('\\');
            if (slash != -1 && slash > bslash) return filePath.Substring(slash + 1);
            if (bslash != -1 && bslash > slash) return filePath.Substring(bslash + 1);
            return "";
        }

        /// <summary>
        /// Returns a list of all files within a directory.
        /// </summary>
        /// <param name="directory">Root directory.</param>
        /// <param name="inCludeSubDirs">Recursive?</param>
        /// <returns></returns>
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

        /// <summary>
        /// File copy operation, wrapped in a 'try' statement.
        /// </summary>
        /// <param name="from">From path.</param>
        /// <param name="to">To path.</param>
        /// <param name="overwrite">Overwrite?</param>
        /// <returns>Copy successful.</returns>
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



        /// <summary>
        /// Copy all files from one directory to another.
        /// Can also multithread :)
        /// </summary>
        /// <param name="fromRootDirectory">From path.</param>
        /// <param name="toRootDirectory">To path.</param>
        /// <param name="overwrite">Overwrite?</param>
        /// <returns></returns>
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





        /// <summary>
        /// Formats a file path so it's universally readable.
        /// </summary>
        /// <param name="path">Path.</param>
        /// <returns>Formatted path.</returns>
        public static string FormatPath(string path)
        {
            path = path.Replace("\\", "/");
            while (path.Contains("//")) path = path.Replace("//", "/");
            return path;
        }













        //Woah there...  What about a string[] and a for()?
        //Way simpler, but oh well.
        /// <summary>
        /// Returns if the path ends with an image file extension.
        /// </summary>
        /// <param name="filePath">File path.</param>
        /// <returns>File is an image.</returns>
        static bool FileIsImage(string filePath)
        {
            filePath = filePath.ToLower();

            //Image file
            if (filePath.EndsWith(".jpg")) return true;
            if (filePath.EndsWith(".jpeg")) return true;
            if (filePath.EndsWith(".jfif")) return true;
            if (filePath.EndsWith(".exif")) return true;
            if (filePath.EndsWith(".tif")) return true;
            if (filePath.EndsWith(".tiff")) return true;
            if (filePath.EndsWith(".gif")) return true;
            if (filePath.EndsWith(".bmp")) return true;
            if (filePath.EndsWith(".png")) return true;
            if (filePath.EndsWith(".ppm")) return true;
            if (filePath.EndsWith(".pgm")) return true;
            if (filePath.EndsWith(".pbm")) return true;
            if (filePath.EndsWith(".pnm")) return true;
            if (filePath.EndsWith(".heif")) return true;
            if (filePath.EndsWith(".bpg")) return true;
            if (filePath.EndsWith(".deep")) return true;
            if (filePath.EndsWith(".drw")) return true;
            if (filePath.EndsWith(".ecw")) return true;
            if (filePath.EndsWith(".fits")) return true;
            if (filePath.EndsWith(".flif")) return true;
            if (filePath.EndsWith(".ico")) return true;
            if (filePath.EndsWith(".ilbm")) return true;
            if (filePath.EndsWith(".img")) return true;
            if (filePath.EndsWith(".nrrd")) return true;
            if (filePath.EndsWith(".pam")) return true;
            if (filePath.EndsWith(".pcx")) return true;
            if (filePath.EndsWith(".pgf")) return true;
            if (filePath.EndsWith(".plbm")) return true;
            if (filePath.EndsWith(".sgi")) return true;
            if (filePath.EndsWith(".sid")) return true;
            if (filePath.EndsWith(".tga")) return true;
            if (filePath.EndsWith(".xisf")) return true;

            //Editor files
            if (filePath.EndsWith(".cd5")) return true;
            if (filePath.EndsWith(".cpt")) return true;
            if (filePath.EndsWith(".kra")) return true;
            if (filePath.EndsWith(".mdp")) return true;
            if (filePath.EndsWith(".pdn")) return true;
            if (filePath.EndsWith(".psd")) return true;
            if (filePath.EndsWith(".psp")) return true;
            if (filePath.EndsWith(".sai")) return true;
            if (filePath.EndsWith(".xcf")) return true;

            //Vector Images
            if (filePath.EndsWith(".cgm")) return true;
            if (filePath.EndsWith(".svg")) return true;
            if (filePath.EndsWith(".cdr")) return true;

            //PDF
            if (filePath.EndsWith(".pdf")) return true;

            return false;
        }
    }
}