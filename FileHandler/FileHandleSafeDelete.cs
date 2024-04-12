/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     FileHandler
 * FILE:        FileHandler/FileHandleSafeDelete.cs
 * PURPOSE:     Variation of delete files, with Progress Bar and Deletion to Recycle BIn
 * PROGRAMER:   Peter Geinitz (Wayfarer)
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
        ///     Deletes the file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Success Status</returns>
        /// <exception cref="FileHandlerException"></exception>
        public static bool DeleteFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new FileHandlerException(FileHandlerResources.ErrorEmptyString);
            }

            if (!File.Exists(path))
            {
                return false;
            }

            var count = 0;

            //Handle the fact that file is in Use
            for (var i = 0; FileHandleDelete.IsFileLocked(path) && i < FileHandlerRegister.Tries + 1; i++)
            {
                count++;
                Trace.WriteLine(string.Concat(FileHandlerResources.Tries, count));
                Thread.Sleep(1000);
            }

            //well we tried the max number of tries so no need to go further
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
                FileSystem.DeleteFile(path, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
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
    }
}
