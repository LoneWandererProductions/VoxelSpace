/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     FileHandler
 * FILE:        FileHandler/FileUtility.cs
 * PURPOSE:     Some Basic helper Functions
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable UnusedMember.Global

using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace FileHandler
{
    public static class FileUtility
    {
        /// <summary>
        ///     Gets the new name of the file, if it already exists
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>new Path with new FileName</returns>
        [return: MaybeNull]
        public static string GetNewFileName(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            var fileNameOnly = Path.GetFileNameWithoutExtension(path);
            var extension = Path.GetExtension(path);
            var directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                return null;
            }

            var newPath = path;

            var count = 0;

            while (File.Exists(newPath))
            {
                var cache = $"{fileNameOnly}({count++})";
                var tempFileName = Path.Combine(directory!, Path.ChangeExtension(cache, extension));
                if (File.Exists(tempFileName))
                {
                    continue;
                }

                newPath = tempFileName;
            }

            return newPath;
        }
    }
}
