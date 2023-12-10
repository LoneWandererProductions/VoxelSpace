/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     FileHandler
 * FILE:        FileHandler/FileHandlerRegister.cs
 * PURPOSE:     Register for basic configs and some Messages
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBeInternal
// ReSharper disable MemberCanBePrivate.Global

using System;
using System.Collections.Generic;

namespace FileHandler
{
    public static class FileHandlerRegister
    {
        /// <summary>
        ///     Set Number of retries we do, when we delete a File
        /// </summary>
        public static int Tries { get; set; } = 3;

        /// <summary>
        ///     Gets or sets the maximum log.
        /// </summary>
        /// <value>
        ///     The maximum log.
        /// </value>
        public static int MaxLog { get; set; } = 10000;

        /// <summary>
        ///     Gets the error log.
        /// </summary>
        /// <value>
        ///     The error log.
        /// </value>
        public static List<string> ErrorLog { get; private set; }

        /// <summary>
        ///     Send the status
        /// </summary>
        public static EventHandler<string> SendStatus { get; set; }

        /// <summary>
        ///     Send the overview
        /// </summary>
        public static EventHandler<FileItems> SendOverview { get; set; }

        /// <summary>
        ///     Adds the error.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="path">The path.</param>
        /// <param name="exception">The exception.</param>
        public static void AddError(string method, string path, Exception exception)
        {
            ErrorLog ??= new List<string>();

            if (ErrorLog.Count == MaxLog) ErrorLog.Clear();

            ErrorLog.Add(string.Concat(FileHandlerResources.ErrorLogMethod, method, Environment.NewLine,
                FileHandlerResources.ErrorLogPath, path, Environment.NewLine,
                FileHandlerResources.ErrorLog, exception));
        }

        /// <summary>
        ///     Clears the log.
        /// </summary>
        public static void ClearLog()
        {
            ErrorLog ??= new List<string>();
            ErrorLog.Clear();
        }
    }

    /// <summary>
    ///     Basic Information holder for the curious user, if he wants to add an Progress bar
    /// </summary>
    public sealed class FileItems
    {
        /// <summary>
        ///     Gets or sets the Elements
        /// </summary>
        /// <value>
        ///     The Items.
        /// </value>
        public List<string> Elements { get; init; }

        /// <summary>
        ///     Gets or sets the Message.
        /// </summary>
        /// <value>
        ///     The Message.
        /// </value>
        public string Message { get; init; }
    }
}