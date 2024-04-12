/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     FileHandler
 * FILE:        FileHandler/FileHandleCut.cs
 * PURPOSE:     Does all types of File Operations, Copy Files and deletes them afterwards.
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace FileHandler
{
    /// <summary>
    ///     The file handle Cut class.
    /// </summary>
    public static class FileHandleCut
    {
        /// <summary>
        ///     Copy and delete all Files to another Location, includes subdirectories
        ///     Example: https://msdn.microsoft.com/de-de/library/bb762914%28v=vs.110%29.aspx
        /// </summary>
        /// <param name="source">Full qualified location Path</param>
        /// <param name="target">Full qualified target Path</param>
        /// <param name="overwrite">Is overwrite allowed</param>
        /// <returns>Status if we encountered any problems</returns>
        /// <exception cref="FileHandlerException">No Correct Path was provided</exception>
        public static bool CutFiles(string source, string target, bool overwrite)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target))
            {
                throw new FileHandlerException(FileHandlerResources.ErrorEmptyString);
            }

            if (source.Equals(target, StringComparison.OrdinalIgnoreCase))
            {
                throw new FileHandlerException(FileHandlerResources.ErrorEqualPath);
            }

            //if nothing exists we can return anyways
            if (!Directory.Exists(source))
            {
                return false;
            }

            var check = true;
            var dir = new DirectoryInfo(source);
            var dirs = dir.GetDirectories();
            var files = dir.GetFiles();

            //Give the User Optional Infos about the Amount we Copy
            var lstFiles = (from file in files
                select file.Name).ToList();

            var itm = new FileItems
            {
                Elements = new List<string>(lstFiles), Message = FileHandlerResources.InformationFileDeletion
            };

            FileHandlerRegister.SendOverview?.Invoke(nameof(CutFiles), itm);

            //do the actual work
            if (files.Length > 0)
            {
                if (!Directory.Exists(target))
                {
                    _ = Directory.CreateDirectory(target);
                }

                foreach (var file in files)
                {
                    var tempPath = Path.Combine(target, file.Name);

                    try
                    {
                        file.MoveTo(tempPath, overwrite);

                        FileHandlerRegister.SendStatus?.Invoke(nameof(CutFiles), file.Name);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        check = false;
                        Trace.WriteLine(ex);
                        FileHandlerRegister.AddError(nameof(CutFiles), file.Name, ex);
                    }
                    catch (ArgumentException ex)
                    {
                        check = false;
                        Trace.WriteLine(ex);
                        FileHandlerRegister.AddError(nameof(CutFiles), file.Name, ex);
                    }
                    catch (IOException ex)
                    {
                        check = false;
                        Trace.WriteLine(ex);
                        FileHandlerRegister.AddError(nameof(CutFiles), file.Name, ex);
                    }
                    catch (NotSupportedException ex)
                    {
                        check = false;
                        Trace.WriteLine(ex);
                        FileHandlerRegister.AddError(nameof(CutFiles), file.Name, ex);
                    }
                }
            }

            foreach (var subDir in dirs)
            {
                var tempPath = Path.Combine(target, subDir.Name);
                if (!Directory.Exists(target))
                {
                    _ = Directory.CreateDirectory(target);
                }

                if (File.Exists(tempPath))
                {
                    continue;
                }

                _ = CutFiles(subDir.FullName, tempPath, overwrite);
            }

            return check;
        }

        /// <summary>
        ///     Copy and delete all Files to another Location, includes subdirectories
        /// </summary>
        /// <param name="source">Full qualified location Path</param>
        /// <param name="target">List of Files</param>
        /// <param name="overwrite">Is overwrite allowed</param>
        /// <returns>Status if we encountered any problems</returns>
        /// <exception cref="FileHandlerException">No Correct Path was provided</exception>
        public static bool CutFiles(List<string> source, string target, bool overwrite)
        {
            if (source == null || source.Count == 0 || string.IsNullOrEmpty(target))
            {
                throw new FileHandlerException(FileHandlerResources.ErrorEmptyString);
            }

            if (!Directory.Exists(target))
            {
                _ = Directory.CreateDirectory(target);
            }

            //Give the User Optional Infos about the Amount we Copy
            var itm = new FileItems
            {
                Elements = new List<string>(source), Message = FileHandlerResources.InformationFileDeletion
            };

            FileHandlerRegister.SendOverview?.Invoke(nameof(CutFiles), itm);

            var check = true;
            //Do the work
            var root = FileHandleCopy.SearchRoot(source);

            foreach (var element in source)
            {
                try
                {
                    var file = new FileInfo(element);

                    //Get Sub Folder
                    var path = FileHandlerProcessing.GetSubFolder(element, root, target);

                    if (path?.Length == 0)
                    {
                        continue;
                    }

                    var tempPath = Path.Combine(path!, file.Name);

                    if (!Directory.Exists(path))
                    {
                        _ = Directory.CreateDirectory(path);
                    }

                    file.MoveTo(tempPath, overwrite);

                    FileHandlerRegister.SendStatus?.Invoke(nameof(CutFiles), file.Name);
                }
                catch (UnauthorizedAccessException ex)
                {
                    check = false;
                    Trace.WriteLine(ex);
                    FileHandlerRegister.AddError(nameof(CutFiles), element, ex);
                }
                catch (ArgumentException ex)
                {
                    check = false;
                    Trace.WriteLine(ex);
                    FileHandlerRegister.AddError(nameof(CutFiles), element, ex);
                }
                catch (IOException ex)
                {
                    check = false;
                    Trace.WriteLine(ex);
                    FileHandlerRegister.AddError(nameof(CutFiles), element, ex);
                }
                catch (NotSupportedException ex)
                {
                    check = false;
                    Trace.WriteLine(ex);
                    FileHandlerRegister.AddError(nameof(CutFiles), element, ex);
                }
            }

            return check;
        }
    }
}
