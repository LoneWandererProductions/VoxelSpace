/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     FileHandler
 * FILE:        FileHandler/FileHandleCreate.cs
 * PURPOSE:     Does all types of File Operations, Create Files
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable UnusedMember.Global

using System;
using System.Diagnostics;
using System.IO;

namespace FileHandler
{
    /// <summary>
    ///     The file handle create class.
    /// </summary>
    public static class FileHandleCreate
    {
        /// <summary>
        ///     Create a Folder
        /// </summary>
        /// <param name="path">path</param>
        /// <exception cref="FileHandlerException">No Correct Path was provided</exception>
        public static void CreateFolder(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new FileHandlerException(FileHandlerResources.ErrorEmptyString);

            _ = CreateDirectory(path);
        }

        /// <summary>
        ///     Creates a Folder in a specific path, based on the root folder
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="name">Folder Name</param>
        /// <returns>If path was generated </returns>
        /// <exception cref="FileHandlerException">No Correct Path was provided</exception>
        public static bool CreateFolder(string path, string name)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(name))
                throw new FileHandlerException(FileHandlerResources.ErrorEmptyString);

            var root = Path.Combine(path, name);

            return CreateDirectory(root);
        }

        /// <summary>
        ///     Creates the directory.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>If path was generated </returns>
        private static bool CreateDirectory(string path)
        {
            try
            {
                _ = Directory.CreateDirectory(path);
                return true;
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException or IOException or PathTooLongException)
            {
                FileHandlerRegister.AddError(nameof(CreateFolder), path, ex);
                Trace.WriteLine(ex);
                return false;
            }
        }
    }
}