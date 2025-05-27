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
using System.Threading.Tasks;

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
        /// <param name="delele">if set to <c>true</c> [delele] Source Files. Optional, default true.</param>
        /// <returns>Operation Success</returns>
        public static async Task<bool> SaveZip(string zipPath, List<string> fileToAdd, bool delele = true)
        {
            try
            {
                using var archive = ZipFile.Open(zipPath, ZipArchiveMode.Update);
                foreach (var file in fileToAdd)
                {
                    //does not exist? Well next one
                    if (!File.Exists(file)) continue;

                    // Add the entry for each file
                    var fileInfo = new FileInfo(file);
                    _ = archive.CreateEntryFromFile(fileInfo.FullName, fileInfo.Name);

                    FileHandlerRegister.SendStatus?.Invoke(nameof(SaveZip), file);
                }
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException or ArgumentException or IOException
                                           or NotSupportedException)
            {
                FileHandlerRegister.AddError(nameof(SaveZip), zipPath, ex);
                Trace.WriteLine(ex);
                return false;
            }

            //shall we delete old files?
            if (!delele) return true;

            var deleteTasks = fileToAdd.Select(async file => await FileHandleDelete.DeleteFile(file));
            var results = await Task.WhenAll(deleteTasks);

            return results.All(result => result);
        }

        /// <summary>
        ///     Opens the zip.
        /// </summary>
        /// <param name="zipPath">The zip path.</param>
        /// <param name="extractPath">The extract path.</param>
        /// <param name="delete">if set to <c>true</c> [delete]. Optional, default true.</param>
        /// <returns>Operation Success</returns>
        /// <exception cref="FileHandlerException"></exception>
        public static async Task<bool> OpenZip(string zipPath, string extractPath, bool delete = true)
        {
            if (!File.Exists(zipPath))
                throw new FileHandlerException(string.Concat(FileHandlerResources.ErrorFileNotFound, zipPath));

            try
            {
                using var archive = ZipFile.Open(zipPath, ZipArchiveMode.Update);
                archive.ExtractToDirectory(extractPath);
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException or ArgumentException or IOException
                                           or NotSupportedException)
            {
                FileHandlerRegister.AddError(nameof(OpenZip), zipPath, ex);
                Trace.WriteLine(ex);
                return false;
            }

            // Shall we delete old files?
            return !delete || await FileHandleDelete.DeleteFile(zipPath);
        }
    }
}