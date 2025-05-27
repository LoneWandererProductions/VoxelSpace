/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     FileHandler
 * FILE:        FileHandler/PathInformation.cs
 * PURPOSE:     Generic System Functions for Path
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System.IO;

namespace FileHandler
{
    /// <summary>
    ///     The path information class.
    /// </summary>
    public static class PathInformation
    {
        /// <summary>
        ///     Returns full Path with file and no File Extension
        /// </summary>
        /// <param name="path">Path</param>
        /// <returns>Path with File and without File Extension</returns>
        /// <exception cref="FileHandlerException">No Correct Path was provided</exception>
        public static string GetPathWithoutExtension(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new FileHandlerException(FileHandlerResources.ErrorEmptyString);

            var root = Path.GetDirectoryName(path);

            return string.IsNullOrEmpty(root)
                ? string.Empty
                : Path.Combine(root, Path.GetFileNameWithoutExtension(path));
        }
    }
}