/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     FileHandler
 * FILE:        FileHandler/FileNameConverter.cs
 * PURPOSE:     Helps to perform some generic renaming Operations
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

//TODO Rollback for all Features

using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace FileHandler
{
    /// <summary>
    ///     File Name Converter Class
    /// </summary>
    public static class FileNameConverter
    {
        /// <summary>Removes the appendage of all Files in a Folder who start with it.</summary>
        /// <param name="appendage">The appendage of the Files in the Folder.</param>
        /// <param name="folder">The folder.</param>
        /// <param name="subFolder">if set to <c>true</c> [sub folder].</param>
        /// <returns>Number of Files renamed</returns>
        public static async Task<int> RemoveAppendage(string appendage, string folder, bool subFolder)
        {
            var lst = FileHandleSearch.GetAllFiles(folder, subFolder);

            if (lst == null || lst.Count == 0)
            {
                return 0;
            }

            var count = 0;

            foreach (var path in lst)
            {
                var str = Path.GetFileName(path);

                var file = str.RemoveAppendage(appendage);

                if (string.IsNullOrEmpty(file) ||
                    string.Equals(str, file, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var directory = Path.GetDirectoryName(path);
                if (string.IsNullOrEmpty(directory))
                {
                    continue;
                }

                var target = Path.Combine(directory, file);

                // Check if the target file already exists
                if (File.Exists(target))
                {
                    Trace.WriteLine($"{FileHandlerResources.ErrorFileAlreadyExists} {target}");
                    continue; // Skip renaming
                }

                var check = await FileHandleRename.RenameFile(path, target);

                if (!check)
                {
                    Trace.WriteLine(path);
                    return count;
                }

                count++;
            }

            return count;
        }

        /// <summary>Adds the appendage of all Files in a Folder who do not start with it.</summary>
        /// <param name="appendage">The appendage of the Files in the Folder.</param>
        /// <param name="folder">The folder.</param>
        /// <param name="subFolder">if set to <c>true</c> [sub folder].</param>
        /// <returns>Number of Files renamed</returns>
        public static async Task<int> AddAppendage(string appendage, string folder, bool subFolder)
        {
            var lst = FileHandleSearch.GetAllFiles(folder, subFolder);

            if (lst == null || lst.Count == 0)
            {
                return 0;
            }

            var count = 0;

            foreach (var path in lst)
            {
                var str = Path.GetFileName(path);

                var file = str.AddAppendage(appendage);

                if (string.IsNullOrEmpty(file) ||
                    string.Equals(str, file, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var directory = Path.GetDirectoryName(path);
                if (string.IsNullOrEmpty(directory))
                {
                    continue;
                }

                var target = Path.Combine(directory, file);

                // Check if the target file already exists
                if (File.Exists(target))
                {
                    Trace.WriteLine($"{FileHandlerResources.ErrorFileAlreadyExists} {target}");
                    continue; // Skip renaming
                }

                var check = await FileHandleRename.RenameFile(path, target);

                if (!check)
                {
                    Trace.WriteLine(path);
                    return count;
                }

                count++;
            }

            return count;
        }

        /// <summary>Removes the string.</summary>
        /// <param name="targetStr">Target string</param>
        /// <param name="update">Set the string replacement</param>
        /// <param name="folder">The folder.</param>
        /// <param name="subFolder">if set to <c>true</c> [sub folder].</param>
        /// <returns>Number of Files renamed</returns>
        public static async Task<int> ReplacePart(string targetStr, string update, string folder, bool subFolder)
        {
            var lst = FileHandleSearch.GetAllFiles(folder, subFolder);

            if (lst == null || lst.Count == 0)
            {
                return 0;
            }

            var count = 0;

            foreach (var path in lst)
            {
                var str = Path.GetFileName(path);
                var file = str.ReplacePart(targetStr, update);

                if (string.IsNullOrEmpty(file) ||
                    string.Equals(str, file, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var directory = Path.GetDirectoryName(path);
                if (string.IsNullOrEmpty(directory))
                {
                    continue;
                }

                file = file.Replace(targetStr, update);

                var target = Path.Combine(directory, file);

                // Check if the target file already exists
                if (File.Exists(target))
                {
                    Trace.WriteLine($"{FileHandlerResources.ErrorFileAlreadyExists} {target}");
                    continue; // Skip renaming
                }

                var check = await FileHandleRename.RenameFile(path, target);

                if (!check)
                {
                    Trace.WriteLine(path);
                    return count;
                }

                count++;
            }

            return count;
        }

        /// <summary>
        ///     Removes all Numbers from a file and puts them in order add the end of the separated with _
        ///     Not sure about the value, but I used it at least once
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="subFolder">if set to <c>true</c> [sub folder].</param>
        /// <returns>Number of Files renamed</returns>
        public static async Task<int> ReOrderNumbers(string folder, bool subFolder)
        {
            var lst = FileHandleSearch.GetAllFiles(folder, subFolder);

            if (lst == null || lst.Count == 0)
            {
                return 0;
            }

            var count = 0;

            foreach (var path in lst)
            {
                var str = Path.GetFileNameWithoutExtension(path);
                var ext = Path.GetExtension(path);

                var file = str.ReOrderNumbers();
                if (string.IsNullOrEmpty(file) ||
                    string.Equals(str, file, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                file = string.Concat(file, ext);

                var directory = Path.GetDirectoryName(path);
                if (string.IsNullOrEmpty(directory))
                {
                    continue;
                }

                file = Path.Combine(directory, file);

                // Check if the target file already exists
                if (File.Exists(file))
                {
                    Trace.WriteLine($"{FileHandlerResources.ErrorFileAlreadyExists} {file}");
                    continue; // Skip renaming
                }

                var check = await FileHandleRename.RenameFile(path, file);

                if (!check)
                {
                    Trace.WriteLine(path);
                    return count;
                }

                count++;
            }

            return count;
        }
    }
}
