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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

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
        public static bool DeleteFile(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new FileHandlerException(FileHandlerResources.ErrorEmptyString);

            if (!File.Exists(path)) return false;

            var count = 0;

            //Handle the fact that file is in Use
            for (var i = 0; IsFileLocked(path) && i < FileHandlerRegister.Tries + 1; i++)
            {
                count++;
                Trace.WriteLine(string.Concat(FileHandlerResources.Tries, count));
                Thread.Sleep(1000);
            }

            //well we tried the max of tries so no need to go further
            if (count == FileHandlerRegister.Tries)
            {
                var ex = new Exception(string.Concat(FileHandlerResources.ErrorLock, path));
                Trace.WriteLine(ex);
                FileHandlerRegister.AddError(nameof(DeleteFile), path, ex);
                return false;
            }

            //no locks so let's do it
            try
            {
                File.Delete(path);
                FileHandlerRegister.SendStatus?.Invoke(nameof(DeleteFile), path);
            }
            catch (UnauthorizedAccessException ex)
            {
                Trace.WriteLine(ex);
                FileHandlerRegister.AddError(nameof(DeleteFile), path, ex);
                return false;
            }
            catch (IOException ex)
            {
                //well something went wrong, unlucky Access
                Trace.WriteLine(ex);
                FileHandlerRegister.AddError(nameof(DeleteFile), path, ex);
                return false;
            }

            return true;
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

            var check = true;

            foreach (var path in paths) check = DeleteFile(path);

            return check;
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

            _ = DeleteAllContents(path, true);

            _ = DeleteFolder(path);

            return !Directory.Exists(path);
        }

        /// <summary>
        ///     Deletes the contents of a Folder, (optional) includes sub folder
        /// </summary>
        /// <param name="path">Target Folder</param>
        /// <param name="subdirectories">Include Sub-folders</param>
        /// <returns>Status if we encountered any problems</returns>
        /// <exception cref="FileHandlerException">No Correct Path was provided</exception>
        public static bool DeleteAllContents(string path, bool subdirectories)
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
            catch (UnauthorizedAccessException ex)
            {
                check = false;
                FileHandlerRegister.AddError(nameof(DeleteAllContents), string.Empty, ex);
                Trace.WriteLine(ex);
            }
            catch (IOException ex)
            {
                check = false;
                FileHandlerRegister.AddError(nameof(DeleteAllContents), string.Empty, ex);
                Trace.WriteLine(ex);
            }

            return check;
        }

        /// <summary>
        ///     Deletes the contents of a Folder by extension
        /// </summary>
        /// <param name="path">Target Folder</param>
        /// <param name="fileExtList">List of file Extensions</param>
        /// <param name="subdirectories">Include Sub-folders</param>
        /// <returns>Status if we encountered any problems</returns>
        /// <exception cref="FileHandlerException">No Correct Path was provided</exception>
        public static bool DeleteFolderContentsByExtension(string path, List<string> fileExtList, bool subdirectories)
        {
            if (string.IsNullOrEmpty(path)) throw new FileHandlerException(FileHandlerResources.ErrorEmptyString);

            if (!Directory.Exists(path)) return false;

            if (fileExtList == null) return false;

            fileExtList = FileHandlerProcessing.CleanUpExtensionList(fileExtList);

            var check = true;
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

            foreach (var unused in myFiles.Select(DeleteFile).Where(cache => !cache)) check = false;

            return check;
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
            catch (UnauthorizedAccessException ex)
            {
                Trace.WriteLine(ex);
                return false;
            }
            catch (IOException ex)
            {
                //well something went wrong, unlucky Access
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
            catch (ArgumentException ex)
            {
                Trace.WriteLine(string.Concat(FileHandlerResources.ErrorLock, ex));
                return true;
            }
            catch (PathTooLongException ex)
            {
                Trace.WriteLine(string.Concat(FileHandlerResources.ErrorLock, ex));
                return true;
            }
            catch (IOException ex)
            {
                Trace.WriteLine(string.Concat(FileHandlerResources.ErrorLock, ex));
                return true;
            }
            catch (UnauthorizedAccessException ex)
            {
                //the file is locked, probably
                Trace.WriteLine(FileHandlerResources.ErrorLock + ex);
                return true;
            }
            catch (NotSupportedException ex)
            {
                Trace.WriteLine(FileHandlerResources.ErrorLock + ex);
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