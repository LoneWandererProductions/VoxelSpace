/*
* COPYRIGHT:   See COPYING in the top level directory
* PROJECT:     FileHandler
* FILE:        FileHandler/FileHandleDelete.cs
* PURPOSE:     Does all types of File Operations, delete Files
* PROGRAMER:   Peter Geinitz (Wayfarer)
*/

// ReSharper disable MemberCanBePrivate.Global, Config item, leave it be
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBeInternal

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileHandler
{
    /// <summary>
    ///     Handle all kinds of deletions
    /// </summary>
    public static class FileHandleDelete
    {
        /// <summary>
        ///     Deletes a File
        /// </summary>
        /// <param name="path">Target File with Path</param>
        /// <returns>
        ///     Status if we encountered any problems
        /// </returns>
        /// <exception cref="FileHandlerException">No Correct Path was provided</exception>
        public static async Task<bool> DeleteFile(string path)
        {
            // Validate the input path
            if (string.IsNullOrEmpty(path)) throw new FileHandlerException(FileHandlerResources.ErrorEmptyString);

            // Check if the file exists
            if (!File.Exists(path)) return false;

            var count = 0;

            // Handle the fact that the file might be in use
            while (IsFileLocked(path) && count < FileHandlerRegister.Tries)
            {
                count++;
                Trace.WriteLine($"{FileHandlerResources.Tries} {count} {path}");
                await Task.Delay(1000); // Use Task.Delay instead of Thread.Sleep for async context
            }

            // If the max number of tries is reached, log the error and return false
            if (count == FileHandlerRegister.Tries)
            {
                var ex = new Exception($"{FileHandlerResources.ErrorLock} {path}");
                Trace.WriteLine(ex);
                FileHandlerRegister.AddError(nameof(DeleteFile), path, ex);
                return false;
            }

            // No locks, proceed to delete the file
            try
            {
                await Task.Run(() => File.Delete(path)).ConfigureAwait(false);
                FileHandlerRegister.SendStatus?.Invoke(nameof(DeleteFile), path);
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException or IOException)
            {
                FileHandlerRegister.AddError(nameof(DeleteFile), path, ex);
                Trace.WriteLine(ex);
                return false;
            }

            return true; // File deleted successfully
        }

        /// <summary>
        ///     Deletes a File
        /// </summary>
        /// <param name="paths">The paths.</param>
        /// <returns>
        ///     Status if we encountered any problems
        /// </returns>
        /// <exception cref="FileHandlerException">No Correct Path was provided</exception>
        public static bool DeleteFiles(List<string> paths)
        {
            if (paths == null || paths.Count == 0) throw new FileHandlerException(FileHandlerResources.ErrorEmptyList);

            var results = new ConcurrentBag<bool>();

            Parallel.ForEach(paths, async path =>
            {
                var result = await DeleteFile(path);
                results.Add(result);
            });

            // Determine overall success: true only if all deletions were successful
            return results.All(success => success);
        }

        /// <summary>
        ///     Deletes a complete folder and all it's contents
        ///     \path\"example"
        /// </summary>
        /// <param name="path">Target Folder</param>
        /// <returns>Status if we encountered any problems</returns>
        /// <exception cref="FileHandlerException">No Correct Path was provided</exception>
        public static bool DeleteCompleteFolder(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new FileHandlerException(FileHandlerResources.ErrorEmptyString);

            _ = DeleteAllContents(path);

            _ = DeleteFolder(path);

            return !Directory.Exists(path);
        }

        /// <summary>
        ///     Deletes the contents of a Folder, (optional) includes sub folder
        /// </summary>
        /// <param name="path">Target Folder</param>
        /// <param name="subdirectories">Include Sub-folders, optional as default true</param>
        /// <returns>Status if we encountered any problems</returns>
        /// <exception cref="FileHandlerException">No Correct Path was provided</exception>
        public static bool DeleteAllContents(string path, bool subdirectories = true)
        {
            if (string.IsNullOrEmpty(path)) throw new FileHandlerException(FileHandlerResources.ErrorEmptyString);

            if (!Directory.Exists(path)) return false;

            var check = true;

            var option = subdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            try
            {
                var myFiles = Directory.GetFiles(path, FileHandlerResources.FileSeparator, option);

                //Give the User Optional Infos about the Amount we delete
                var itm = new FileItems
                {
                    Elements = new List<string>(myFiles), Message = FileHandlerResources.InformationFileDeletion
                };

                FileHandlerRegister.SendOverview?.Invoke(nameof(DeleteAllContents), itm);

                if (myFiles.Length == 0) return false;

                foreach (var file in myFiles) _ = DeleteFile(file);
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException or IOException)
            {
                FileHandlerRegister.AddError(nameof(DeleteAllContents), string.Empty, ex);
                Trace.WriteLine(ex);
                check = false;
            }

            return check;
        }

        /// <summary>
        ///     Deletes the contents of a Folder by extension
        /// </summary>
        /// <param name="path">Target Folder</param>
        /// <param name="fileExtList">List of file Extensions</param>
        /// <param name="subdirectories">Include Sub-folders, optional, default true</param>
        /// <returns>Status if we encountered any problems</returns>
        /// <exception cref="FileHandlerException">No Correct Path was provided</exception>
        public static async Task<bool> DeleteFolderContentsByExtension(string path, List<string> fileExtList,
            bool subdirectories = true)
        {
            if (string.IsNullOrEmpty(path)) throw new FileHandlerException(FileHandlerResources.ErrorEmptyString);

            if (!Directory.Exists(path)) return false;

            if (fileExtList == null) return false;

            fileExtList = FileHandlerProcessing.CleanUpExtensionList(fileExtList);

            var myFiles = new List<string>();

            foreach (var files
                     in
                     fileExtList.Select(
                         appendix => FileHandleSearch.GetFilesByExtensionFullPath(path, appendix, subdirectories))
                    )
            {
                if (files == null) return false;

                myFiles.AddRange(files);

                //Give the User Optional Infos about the Amount we delete
                var itm = new FileItems
                {
                    Elements = new List<string>(myFiles), Message = FileHandlerResources.InformationFileDeletion
                };

                FileHandlerRegister.SendOverview?.Invoke(nameof(DeleteFolderContentsByExtension), itm);
            }

            if (myFiles.Count == 0) return false;

            // Asynchronously delete files
            var deletionTasks = myFiles.Select(DeleteFile).ToList();

            // Await all deletion tasks to complete
            var results = await Task.WhenAll(deletionTasks);

            // Return true if all deletions were successful
            return results.All(result => result);
        }

        /// <summary>
        ///     Deletes a Folder
        /// </summary>
        /// <param name="path">Target Folder with Path</param>
        /// <exception cref="FileHandlerException">No Correct Path was provided</exception>
        public static bool DeleteFolder(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new FileHandlerException(FileHandlerResources.ErrorEmptyString);

            if (!Directory.Exists(path)) return true;

            try
            {
                Directory.Delete(path, true);
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException or IOException)
            {
                FileHandlerRegister.AddError(nameof(DeleteFolder), path, ex);
                Trace.WriteLine(ex);
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Checks if File is Locked
        /// </summary>
        /// <param name="path">Target File with Path</param>
        /// <returns>
        ///     File Status
        /// </returns>
        public static bool IsFileLocked(string path)
        {
            FileStream stream = null;

            try
            {
                stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (Exception ex) when (ex is ArgumentException or PathTooLongException or IOException
                                           or UnauthorizedAccessException or NotSupportedException)
            {
                Trace.WriteLine(string.Concat(FileHandlerResources.ErrorLock, ex));
                return true;
            }
            finally
            {
                stream?.Close();
            }

            //file is not locked
            return false;
        }
    }
}