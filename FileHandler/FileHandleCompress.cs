/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     FileHandler
 * FILE:        FileHandler/FileHandleCompress.cs
 * PURPOSE:     File Compression Utilities
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 * Sources:     https://docs.microsoft.com/de-de/dotnet/api/system.io.compression.zipfile?view=net-5.0
 *              https://docs.microsoft.com/de-de/dotnet/api/system.io.compression.ziparchive.entries?view=net-5.0
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;

// ReSharper disable UnusedMember.Global

namespace FileHandler
{
    /// <summary>
    ///     Included Compression Library
    /// </summary>
    public static class FileHandleCompress
    {
        /// <summary>
        ///     Saves the zip.
        /// </summary>
        /// <param name="zipPath">The path of the zip.</param>
        /// <param name="fileToAdd">The file(s) to add.</param>
        /// <param name="delele">if set to <c>true</c> [delele] Source Files.</param>
        /// <returns>Operation Success</returns>
        public static bool SaveZip(string zipPath, List<string> fileToAdd, bool delele)
        {
            var check = true;

            try
            {
                using var archive = ZipFile.Open(zipPath, ZipArchiveMode.Update);
                foreach (var file in fileToAdd)
                {
                    //does not exist? Well next one
                    if (!FileHandleSearch.FileExists(file)) continue;

                    // Add the entry for each file
                    var fileInfo = new FileInfo(file);
                    _ = archive.CreateEntryFromFile(fileInfo.FullName, fileInfo.Name);

                    FileHandlerRegister.SendStatus?.Invoke(nameof(SaveZip), file);
                }
            }
            catch (IOException ex)
            {
                Trace.WriteLine(ex);
                FileHandlerRegister.AddError(nameof(SaveZip), zipPath, ex);
                return false;
            }
            catch (ArgumentException ex)
            {
                Trace.WriteLine(ex);
                FileHandlerRegister.AddError(nameof(SaveZip), zipPath, ex);
                return false;
            }
            catch (UnauthorizedAccessException ex)
            {
                Trace.WriteLine(ex);
                FileHandlerRegister.AddError(nameof(SaveZip), zipPath, ex);
                return false;
            }

            //shall we delete old files?
            if (!delele) return true;

            foreach (var unused in fileToAdd.Select(FileHandleDelete.DeleteFile).Where(cache => !cache)) check = false;

            return check;
        }

        /// <summary>
        ///     Opens the zip.
        /// </summary>
        /// <param name="zipPath">The zip path.</param>
        /// <param name="extractPath">The extract path.</param>
        /// <param name="delete">if set to <c>true</c> [delete].</param>
        /// <returns>Operation Success</returns>
        /// <exception cref="FileHandlerException"></exception>
        public static bool OpenZip(string zipPath, string extractPath, bool delete)
        {
            if (!FileHandleSearch.FileExists(zipPath))
                throw new FileHandlerException(string.Concat(FileHandlerResources.ErrorFileNotFound, zipPath));

            try
            {
                using var archive = ZipFile.Open(zipPath, ZipArchiveMode.Update);
                archive.ExtractToDirectory(extractPath);
            }
            catch (IOException ex)
            {
                Trace.WriteLine(ex);
                FileHandlerRegister.AddError(nameof(OpenZip), zipPath, ex);
                return false;
            }
            catch (ArgumentException ex)
            {
                Trace.WriteLine(ex);
                FileHandlerRegister.AddError(nameof(OpenZip), zipPath, ex);
                return false;
            }
            catch (UnauthorizedAccessException ex)
            {
                Trace.WriteLine(ex);
                FileHandlerRegister.AddError(nameof(OpenZip), zipPath, ex);
                return false;
            }

            //shall we delete old files?
            return !delete || FileHandleDelete.DeleteFile(zipPath);
        }
    }
}