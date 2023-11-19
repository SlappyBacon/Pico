using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pico.Files
{
    /// <summary>
    /// A collection of abstract file tools.
    /// </summary>
	public static class FileTools
    {
        /// <summary>
        /// Creates a dummy file at the specified path.
        /// File bytes are random, and can take some time to generate.
        /// </summary>
        /// <param name="outputPath">File output path.</param>
        /// <param name="fileSize">File size, in bytes.</param>
        public static void CreateDummyFile(string outputPath, long fileSize = 1024)
        {
            if (File.Exists(outputPath)) return;

            const int defaultBufferSize = 1024000;
            byte[] buffer;
            if (fileSize < defaultBufferSize)
            {
                buffer = new byte[fileSize];
            }
            else buffer = new byte[defaultBufferSize];

            FileStream writer = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
            while (true)
            {
                if (fileSize < 1) break;

                if (fileSize < defaultBufferSize)
                {
                    Random.Shared.NextBytes(buffer);
                    writer.Write(buffer, 0, (int)fileSize);
                    fileSize -= fileSize;
                }
                else
                {
                    Random.Shared.NextBytes(buffer);
                    writer.Write(buffer, 0, buffer.Length);
                    fileSize -= buffer.Length;
                }
            }
            writer.Dispose();
        }

        /// <summary>
        /// Input file path, returns the name of the file.
        /// </summary>
        /// <param name="filePath">File path.</param>
        /// <returns>File name.</returns>
        public static string GetFileName(string filePath)
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
        /// <param name="includeSubDirs">Recursive?</param>
        /// <returns></returns>
        public static string[] GetAllFilePaths(string directory, bool includeSubDirs = false, CancellationToken? ct = null)
        {
            List<string> result = new List<string>();
            ForEachFilePath(directory, result.Add, includeSubDirs, ct);
            return result.ToArray();
        }

        /// <summary>
        /// Iterates through each file, and performs an action to each one.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="action"></param>
        /// <param name="includeSubDirs"></param>
        /// <returns></returns>
        public static void ForEachFilePath(string directory, Action<string> action, bool includeSubDirs = false, CancellationToken? ct = null)
        {
            if (directory == null) return;
            if (action == null) return;

            if (!Directory.Exists(directory)) return;

            string[] filePaths;
            try
            {
                filePaths = Directory.GetFiles(directory);
            }
            catch
            {
                filePaths = new string[0];
            }

            for (int i = 0; i < filePaths.Length; i++)
            {
                //Cancelled?
                if (ct?.IsCancellationRequested == true) break;
                var path = filePaths[i];
                action.Invoke(path);
            }

            if (includeSubDirs)
            {
                string[] subDirs;
                try
                {
                    subDirs = Directory.GetDirectories(directory);
                }
                catch
                {
                    subDirs = new string[0];
                }

                for (int i = 0; i < subDirs.Length; i++)
                {
                    //Cancelled?
                    if (ct?.IsCancellationRequested == true) break;
                    ForEachFilePath(subDirs[i], action, includeSubDirs, ct);
                }
            }
        }

        /// <summary>
        /// Iterates through each file, and performs an action to each one.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="action"></param>
        /// <param name="includeSubDirs"></param>
        /// <returns></returns>
        public static void ForEachFilePathParallel(string directory, Action<string> action, bool includeSubDirs = false, bool waitForFinish = false, CancellationToken? ct = null)
        {
            if (directory == null) return;
            if (action == null) return;
            if (!Directory.Exists(directory)) return;

            string[] filePaths;
            try
            {
                filePaths = Directory.GetFiles(directory);
            }
            catch
            {
                filePaths = new string[0];
            }


            Task[] tasks = new Task[filePaths.Length];

            for (int i = 0; i < filePaths.Length; i++)
            {
                //Cancelled?
                if (ct?.IsCancellationRequested == true) break;
                var path = filePaths[i];
                tasks[i] = Task.Run(() => action.Invoke(path));
            }

            if (includeSubDirs)
            {
                string[] subDirs;
                try
                {
                    subDirs = Directory.GetDirectories(directory);
                }
                catch
                {
                    subDirs = new string[0];
                }

                for (int i = 0; i < subDirs.Length; i++)
                {
                    //Cancelled?
                    if (ct?.IsCancellationRequested == true) break;
                    ForEachFilePathParallel(subDirs[i], action, includeSubDirs, waitForFinish, ct);
                }
            }

            if (waitForFinish) Task.WaitAll(tasks);
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
        /// Formats a file path so it's universally readable.
        /// </summary>
        /// <param name="path">Path.</param>
        /// <returns>Formatted path.</returns>
        public static void FormatPath(ref string path, bool isDirectory = false)
        {
            if (path == null) return;
            
            path = path.Replace("\\", "/");
            
            while (path.Contains("//")) path = path.Replace("//", "/");
            
            if (isDirectory && !path.EndsWith('/')) path += '/';
        }
        /// <summary>
        /// Formats text so there's only one '.' at the beginning.
        /// </summary>
        /// <param name="extention">Extention.</param>
        /// <returns>Formatted path.</returns>
        public static void FormatExtention(ref string extention)
        {
            if (extention == null)
            {
                extention = "";
                return;
            }
            if (extention == "")
            {
                return;
            }

            //ONLY ONE DOT, AT START :)
            extention = extention.Replace(".", "");
            extention = extention.Insert(0, ".");
        }

        /// <summary>
        /// Returns if the path ends with an image file extension.
        /// </summary>
        /// <param name="filePath">File path.</param>
        /// <returns>File is an image.</returns>
        public static bool IsImagePath(string filePath)
        {
            filePath = filePath.ToLower();
            string[] imageExtentions = { ".jpg", ".jpeg" , ".jfif" , ".exif" , ".tif" , ".tiff" ,
            ".gif" , ".bmp" , ".png" , ".ppm" , ".pgm" , ".pbm" , ".pnm" ,".heif" , ".bpg" , ".deep" ,
            ".drw" , ".ecw" , ".fits" , ".flif" , ".ico" , ".ilbm" ,".img",".nrrd",".pam",".pcx",
            ".pgf",".plbm",".sgi",".sid",".tga",".xisf",".cd5",".cpt",".kra",".mdp",".pdn",".psd",
            ".psp",".sai",".xcf",".cgm",".svg",".cdr",".pdf"};
            for (int i = 0; i < imageExtentions.Length; i++)
            {
                if (filePath.EndsWith(imageExtentions[i])) return true;
            }
            return false;
        }
        /// <summary>
        /// Compares two files, and returns if all bytes match.
        /// </summary>
        /// <param name="filePath1">File path one.</param>
        /// <param name="filePath2">File path two.</param>
        /// <returns></returns>
        public static bool FilesAreSame(string filePath1, string filePath2)
        {
            bool result = true;

            byte[] buffer1 = new byte[1024];
            FileStream reader1 = new FileStream(filePath1, FileMode.Open, FileAccess.Read);

            byte[] buffer2 = new byte[1024];
            FileStream reader2 = new FileStream(filePath2, FileMode.Open, FileAccess.Read);

            int readCount1 = -1;
            int readCount2 = -1;

            bool isDone1 = false;
            bool isDone2 = false;

            while (true)
            {
                //Read next bytes
                if (!isDone1)
                {
                    readCount1 = reader1.Read(buffer1, 0, buffer1.Length);
                    if (readCount1 < 1) isDone1 = true;
                }
                if (!isDone2)
                {
                    readCount2 = reader2.Read(buffer2, 0, buffer2.Length);
                    if (readCount2 < 1) isDone2 = true;
                }


                //Process chunk
                if (readCount1 != readCount2)
                {
                    //Different file length
                    result = false;
                    break;
                }
                if (readCount1 > 0 || readCount2 > 0)
                {
                    //Compare byte values
                    for (int i = 0; i < readCount1; i++)
                    {
                        if (buffer1[i] != buffer2[i])
                        {
                            result = false;
                            break;
                        }
                    }
                }

                //Check break
                if (isDone1 && isDone2) break;
            }


            reader1.Dispose();
            reader2.Dispose();
            return result;
        }
        /// <summary>
        /// Compares every bit of each file, and returns the percent
        /// that matches, between 0 and 1.
        /// </summary>
        /// <param name="filePath1">File path one.</param>
        /// <param name="filePath2">File path two.</param>
        /// <returns></returns>
        public static async Task<double> CompareFileBytesAsync(string filePath1, string filePath2)
        {
            byte[] buffer1 = new byte[1024];
            FileStream reader1 = new FileStream(filePath1, FileMode.Open, FileAccess.Read);

            byte[] buffer2 = new byte[1024];
            FileStream reader2 = new FileStream(filePath2, FileMode.Open, FileAccess.Read);

            bool isDone1 = false;
            bool isDone2 = false;

            long totalBits = 0;
            long sameBits = 0;

            int readCount1 = -1;
            int readCount2 = -1;

            while (true)
            {
                //Read next bytes
                if (!isDone1)
                {
                    readCount1 = reader1.Read(buffer1, 0, buffer1.Length);
                    if (readCount1 < 1)
                    {
                        isDone1 = true;
                        reader1.Dispose();
                    }
                }
                if (!isDone2)
                {
                    readCount2 = reader2.Read(buffer2, 0, buffer2.Length);
                    if (readCount2 < 1)
                    {
                        isDone2 = true;
                        reader2.Dispose();
                    }
                }

                //Process chunk
                if (readCount1 > 0 || readCount2 > 0)
                {
                    long chunkTotalBits;
                    long chunkSameBits;
                    CompareChunk(ref buffer1, ref readCount1, ref buffer2, ref readCount2, out chunkTotalBits, out chunkSameBits);
                    totalBits += chunkTotalBits;
                    sameBits += chunkSameBits;
                }

                //Check done break
                if (isDone1 && isDone2) break;
            }

            if (totalBits < 1) return -1;
            return (double)sameBits / (double)totalBits;

            void CompareChunk(ref byte[] array1, ref int array1Count, ref byte[] array2, ref int array2Count, out long chunkTotalBits, out long chunkSameBits)
            {
                chunkSameBits = 0;   //Increment

                if (array1Count > array2Count)
                {
                    //Array 1 is longer
                    chunkTotalBits = array1Count * 8;

                    for (int i = 0; i < array1Count; i++)
                    {
                        if (i > array2.Length) break;

                        var same = CountSameBits(array1[i], array2[i]);
                        chunkSameBits += same;
                    }
                }
                else
                {
                    //Array 2 is longer
                    chunkTotalBits = array2Count * 8;

                    for (int i = 0; i < array2Count; i++)
                    {
                        if (i > array1.Length) break;

                        var same = CountSameBits(array1[i], array2[i]);
                        chunkSameBits += same;
                    }
                }
            }

            int CountSameBits(byte a, byte b)
            {
                int count = 0;
                for (int i = 0; i < 8; i++)
                {
                    if ((a & (1 << i)) == (b & (1 << i)))
                    {
                        count++;
                    }
                }
                return count;
            }
        }

        /// <summary>
        /// Prints all file contents to console, as if it were a text file.
        /// </summary>
        /// <param name="path">File path.</param>
        public static void PrintAllText(string path) => ForEachLine(path, Console.WriteLine);
        
        /// <summary>
        /// A memory saving alternative to using File.ReadAllLines.  Preferrable for large text files.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <param name="lineAction">Action to be performed with each line.</param>
        public static void ForEachLine(string path, Action<string> lineAction)
        {
            const byte nextLineCharByte = (byte)'\n';

            if (!File.Exists(path)) return;
            
            StringBuilder sb = new StringBuilder();

            FileStream fs = new FileStream(path, FileMode.Open);

            byte[] buffer = new byte[1024];
            while (true)
            {
                var readCount = fs.Read(buffer, 0, buffer.Length);
                if (readCount == 0)
                {
                    break;
                }

                //Read Bytes

                for (int i = 0; i < readCount; i++)
                {
                    if (buffer[i] == nextLineCharByte)
                    {
                        lineAction.Invoke(sb.ToString());
                        sb.Clear();
                        continue;
                    }
                    sb.Append((char)buffer[i]);
                }
            }

            fs.Dispose();
        }
    }
}