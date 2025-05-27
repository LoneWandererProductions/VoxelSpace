/*
* COPYRIGHT:   See COPYING in the top level directory
* PROJECT:     FileHandler
* FILE:        FileHandler/FileHandleRename.cs
* PURPOSE:     Utility to Rename Files and Folders
* PROGRAMER:   Peter Geinitz (Wayfarer)
*/

// ReSharper disable MemberCanBeInternal

using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace FileHandler
{
    /// <summary>
    ///     The file handle create class.
    /// </summary>
    public static class FileHandleRename
    {
        /// <summary>
        ///     Rename a Directory.
        /// </summary>
        /// <param name="source">Full qualified location Path</param>
        /// <param name="target">Full qualified target Path</param>
        /// <returns>The <see cref="bool" />Was the Folder Renamed and all contents moved.</returns>
        /// <exception cref="FileHandlerException">No Correct Path was provided</exception>
        public static async Task<bool> RenameDirectory(string source, string target)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target))
                throw new FileHandlerException(FileHandlerResources.ErrorEmptyString);

            if (source.Equals(target, StringComparison.OrdinalIgnoreCase))
                throw new FileHandlerException(FileHandlerResources.ErrorEqualPath);

            //if nothing exists we can return anyways
            if (!Directory.Exists(source)) return false;

            try
            {
                await Task.Run(() => Directory.Move(source, target));
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException or IOException)
            {
                FileHandlerRegister.AddError(nameof(RenameFile), source, ex);
                Trace.WriteLine(ex);
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Rename a file.
        /// </summary>
        /// <param name="source">Full qualified location File Name</param>
        /// <param name="target">Full qualified target File Name</param>
        /// <returns>The <see cref="bool" />Was the File Renamed.</returns>
        /// <exception cref="FileHandlerException">No Correct Path was provided</exception>
        public static async Task<bool> RenameFile(string source, string target)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target))
                throw new FileHandlerException(FileHandlerResources.ErrorEmptyString);

            if (source.Equals(target, StringComparison.InvariantCultureIgnoreCase))
                throw new FileHandlerException(FileHandlerResources.ErrorEqualPath);

            //if nothing exists we can return anyways
            if (!File.Exists(source)) return false;

            try
            {
                await Task.Run(() => File.Move(source, target));
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException or IOException or NotSupportedException)
            {
                FileHandlerRegister.AddError(nameof(RenameFile), source, ex);
                Trace.WriteLine(ex);
                return false;
            }

            return true;
        }
    }
}