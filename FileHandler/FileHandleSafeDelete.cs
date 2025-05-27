/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     FileHandler
 * FILE:        FileHandler/FileHandleSafeDelete.cs
 * PURPOSE:     Variation of delete files, with Progress Bar and Deletion to Recycle Bin
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.VisualBasic.FileIO;

namespace FileHandler
{
    public static class FileHandleSafeDelete
    {
        /// <summary>
        ///     Deletes the specified file and moves it to the Recycle Bin.
        /// </summary>
        /// <param name="path">The path of the file to delete.</param>
        /// <returns>True if the file was successfully deleted; otherwise, false.</returns>
        /// <exception cref="FileHandlerException">Thrown when the path is empty or null.</exception>
        public static bool DeleteFile(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new FileHandlerException(FileHandlerResources.ErrorEmptyString);

            if (!File.Exists(path)) return false;

            if (!WaitForFileUnlock(path))
            {
                var ex = new Exception(string.Concat(FileHandlerResources.ErrorLock, path));
                Trace.WriteLine(ex);
                FileHandlerRegister.AddError(nameof(DeleteFile), path, ex);
                return false;
            }

            // Attempt to delete the file
            try
            {
                FileSystem.DeleteFile(path, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
            }
            catch (UnauthorizedAccessException ex)
            {
                FileHandlerRegister.AddError(nameof(DeleteFile), path, ex);
                Trace.WriteLine(ex);
                return false;
            }
            catch (IOException ex)
            {
                FileHandlerRegister.AddError(nameof(DeleteFile), path, ex);
                Trace.WriteLine(ex);
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Waits for a specified file to become unlocked.
        /// </summary>
        /// <param name="path">The path of the file to check.</param>
        /// <returns>True if the file is unlocked; otherwise, false.</returns>
        private static bool WaitForFileUnlock(string path)
        {
            var attempts = 0;

            while (FileHandleDelete.IsFileLocked(path) && attempts < FileHandlerRegister.Tries)
            {
                attempts++;
                Trace.WriteLine(string.Concat(FileHandlerResources.Tries, attempts));
                Thread.Sleep(1000);
            }

            return !FileHandleDelete.IsFileLocked(path);
        }
    }
}