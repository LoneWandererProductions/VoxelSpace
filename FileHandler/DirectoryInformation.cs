/*
* COPYRIGHT:   See COPYING in the top level directory
* PROJECT:     FileHandler
* FILE:        FileHandler/DirectoryInformation.cs
* PURPOSE:     Generic System Functions for Directories
* PROGRAMER:   Peter Geinitz (Wayfarer)
*/

// ReSharper disable UnusedType.Global

using System;
using System.Diagnostics;
using System.IO;

namespace FileHandler
{
    /// <summary>
    ///     The directory information class.
    /// </summary>
    public static class DirectoryInformation
    {
        /// <summary>
        ///     Returns Path at the defined level height
        /// </summary>
        /// <param name="level">Height of Path</param>
        /// <returns>Returns Path at the current height, can return null.</returns>
        /// <exception cref="FileHandlerException">No Correct Path was provided</exception>
        public static string GetParentDirectory(int level)
        {
            var root = Directory.GetCurrentDirectory();

            if (string.IsNullOrEmpty(root))
            {
                throw new FileHandlerException($"{FileHandlerResources.ErrorGetParentDirectory} {root}");
            }

            var path = Directory.GetParent(root)?.ToString();

            if (string.IsNullOrEmpty(path))
            {
                throw new FileHandlerException($"{FileHandlerResources.ErrorGetParentDirectory} {path}");
            }

            try
            {
                for (var i = 0; i < level; i++)
                {
                    path = Directory.GetParent(path!)?.ToString();
                }
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException or DirectoryNotFoundException or IOException)
            {
                FileHandlerRegister.AddError(nameof(GetParentDirectory), path, ex);
                Trace.WriteLine(ex);
                throw new FileHandlerException($"{FileHandlerResources.ErrorGetParentDirectory} {ex}");
            }

            return path;
        }
    }
}
