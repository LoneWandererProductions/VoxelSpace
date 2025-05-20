/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     FileHandler
 * FILE:        FileHandler/FileHandleSearch.cs
 * PURPOSE:     Does all types of File Operations, search Files
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBeInternal, we use it external
// ReSharper disable UnusedMember.Global, it is a library

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileHandler
{
    /// <summary>
    ///     Handles most File Searches
    /// </summary>
    public static class FileHandleSearch
    {
        /// <summary>
        ///     Collects all files with a specific Extension
        /// </summary>
        /// <param name="path">Target Folder</param>
        /// <param name="appendix">List of File Extension</param>
        /// <param name="subdirectories">Include Sub-folders</param>
        /// <returns>List of Files with Path and extensions<see cref="T:List{string}" />.</returns>
        [return: MaybeNull]
        public static List<string> GetFilesByExtensionFullPath(string path, IEnumerable<string> appendix,
            bool subdirectories)
        {
            var lst = new List<string>();

            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
            {
                return null;
            }

            foreach (var file in appendix.Select(app =>
                         FileHandlerProcessing.GetFilesByExtension(path, app, subdirectories)))
            {
                if (file == null)
                {
                    return null;
                }

                lst.AddRange(file);
            }

            return lst;
        }

        /// <summary>
        ///     Collects all files with a specific Extension
        /// </summary>
        /// <param name="path">Target Folder</param>
        /// <param name="appendix">File Extension</param>
        /// <param name="subdirectories">Include Sub-folders</param>
        /// <returns>List of Files with Path and extension<see cref="T:List{string}" />.</returns>
        [return: MaybeNull]
        public static List<string> GetFilesByExtensionFullPath(string path, string appendix, bool subdirectories)
        {
            return FileHandlerProcessing.GetFilesByExtension(path, appendix, subdirectories);
        }

        /// <summary>
        ///     Collects all files with a specific Extension
        /// </summary>
        /// <param name="path">Target Folder</param>
        /// <param name="appendix">File Extension</param>
        /// <param name="subdirectories">Include Sub-folders</param>
        /// <returns>List of Name of Files, with extension<see cref="T:List{string}" />.</returns>
        [return: MaybeNull]
        public static List<string> GetFileByExtensionWithExtension(string path, string appendix, bool subdirectories)
        {
            var files = FileHandlerProcessing.GetFilesByExtension(path, appendix, subdirectories);

            if (files == null)
            {
                return null;
            }

            var lst = new List<string>();
            lst.AddRange(files.Select(Path.GetFileName));

            return lst;
        }

        /// <summary>
        ///     Get the all files.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="subdirectories">The subdirectories.</param>
        /// <returns>List of Name of Files, with extension and Path<see cref="T:List{string}" />.</returns>
        [return: MaybeNull]
        public static List<string> GetAllFiles(string path, bool subdirectories)
        {
            return FileHandlerProcessing.GetFilesByExtension(path, null, subdirectories);
        }

        /// <summary>
        ///     Gets the file detail.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The Details of a File, to be extended<see cref="T:List{FileDetails}" /> can return null.</returns>
        [return: MaybeNull]
        public static FileDetails GetFileDetails(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            var fileInfo = FileVersionInfo.GetVersionInfo(path);
            var fi = new FileInfo(path);

            return new FileDetails
            {
                Path = path,
                FileName = fi.Name,
                OriginalFilename = fileInfo.OriginalFilename,
                Extension = fi.Extension,
                Size = fi.Length,
                Description = fileInfo.FileDescription,
                CompanyName = fileInfo.CompanyName,
                ProductName = fileInfo.ProductName,
                FileVersion = fileInfo.FileVersion,
                ProductVersion = fileInfo.ProductVersion
            };
        }

        /// <summary>
        ///     Get the file details.
        /// </summary>
        /// <param name="files">The files.</param>
        /// <returns>The Details of the File, to be extended<see cref="T:List{FileDetails}" />.</returns>
        [return: MaybeNull]
        public static List<FileDetails> GetFilesDetails(List<string> files)
        {
            if (files == null || files.Count == 0)
            {
                return null;
            }

            var data = new List<FileDetails>(files.Count);

            data.AddRange(from file in files where File.Exists(file) select GetFileDetails(file));

            return data;
        }

        /// <summary>
        ///     Collects all files with a specific Extension
        /// </summary>
        /// <param name="path">Target Folder</param>
        /// <param name="appendix">File Extension</param>
        /// <param name="subdirectories">Include Sub-folders</param>
        /// <returns>List of Name of Files, without extension and Path</returns>
        [return: MaybeNull]
        public static List<string> GetFileByExtensionWithoutExtension(string path, string appendix, bool subdirectories)
        {
            var files = FileHandlerProcessing.GetFilesByExtension(path, appendix, subdirectories);

            if (files == null)
            {
                return null;
            }

            var lst = new List<string>();
            lst.AddRange(files.Select(Path.GetFileNameWithoutExtension));

            return lst;
        }

        /// <summary>
        ///     Mostly used for Save Game Operations
        /// </summary>
        /// <param name="path">Target Folder</param>
        /// <returns>Returns all Subfolders</returns>
        /// <exception cref="FileHandlerException">No Correct Path was provided</exception>
        [return: MaybeNull]
        public static List<string> GetAllSubfolders(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new FileHandlerException(FileHandlerResources.ErrorEmptyString);
            }

            if (!Directory.Exists(path))
            {
                return null;
            }

            var list = Directory.GetDirectories(path).ToList();

            return list.ConvertAll(Path.GetFileName);
        }

        /// <summary>
        ///     Simple Check if Folder Contains something
        /// </summary>
        /// <param name="path">Target Path</param>
        /// <returns>True if we find Something</returns>
        /// <exception cref="FileHandlerException">No Correct Path was provided</exception>
        public static async Task<bool> CheckIfFolderContainsElement(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new FileHandlerException(FileHandlerResources.ErrorEmptyString);
            }

            if (!Directory.Exists(path))
            {
                return false;
            }

            return await Task.Run(() =>
            {
                var fileCheck = Directory.GetFiles(path).FirstOrDefault();
                return fileCheck != null;
            });
        }


        /// <summary>
        ///     Get the files that contain this sub string. If Invert is true, files that don't contain it.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="appendix">The appendix.</param>
        /// <param name="subdirectories">if set to <c>true</c> [subdirectories].</param>
        /// <param name="subString">The sub string.</param>
        /// <param name="invert">if set to <c>true</c> [invert], does not contain [subString].</param>
        /// <returns>List of files with Extension and Path that contain this string</returns>
        [return: MaybeNull]
        public static List<string> GetFilesWithSubString(string path, IEnumerable<string> appendix, bool subdirectories,
            string subString, bool invert)
        {
            var lst = GetFilesByExtensionFullPath(path, appendix, subdirectories);

            if (lst == null || lst.Count == 0)
            {
                return null;
            }

            var list = new List<string>();

            if (invert)
            {
                list.AddRange(from element in lst
                    let file = Path.GetFileName(element)
                    where !file.Contains(subString)
                    select element);

                return list;
            }

            list.AddRange(from element in lst
                let file = Path.GetFileName(element)
                where file.Contains(subString)
                select element);

            return list;
        }
    }
}
