﻿// Copyright (c) Microsoft and contributors.  All rights reserved.
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.

namespace Microsoft.Azure.Management.ANF.Samples.Common
{

    using System;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Contains public methods to get configuration settings, to initiate authentication, output error results, etc.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Simple function to display this console app basic information
        /// </summary>
        public static void DisplayConsoleAppHeader()
        {
            Console.WriteLine("Azure NetApp Files - Dual-Protocol .Net Core SDK Sample - Sample project that shows how to create a Dual-Protocol volume (NFSv3 and SMB) for Microsoft.NetApp Resource Provider");
            Console.WriteLine("------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine("");
        }

        /// <summary>
        /// Displays errors messages in red
        /// </summary>
        /// <param name="message">Message to be written in console</param>
        public static void WriteErrorMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Utils.WriteConsoleMessage(message);
            Console.ResetColor();
        }

        /// <summary>
        /// Displays errors messages in red
        /// </summary>
        /// <param name="message">Message to be written in console</param>
        public static void WriteConsoleMessage(string message)
        {
            Console.WriteLine($"{DateTime.Now}: {message}");
        }

        /// <summary>
        /// Gets file content
        /// </summary>
        /// <param name="filePath">Path of the file</param>
        /// <returns>File content</returns>
        public static string GetFileContent(string filePath)
        {
            return File.ReadAllText(filePath);
        }

        /// <summary>
        /// Simple method to avoid displaying a password.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Sample code from https://gist.github.com/huobazi/1039424 </remarks>
        public static string GetConsolePassword()
        {
            StringBuilder sb = new StringBuilder();
            while (true)
            {
                ConsoleKeyInfo cki = Console.ReadKey(true);
                if (cki.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }

                if (cki.Key == ConsoleKey.Backspace)
                {
                    if (sb.Length > 0)
                    {
                        Console.Write("\b\0\b");
                        sb.Length--;
                    }

                    continue;
                }

                Console.Write('*');
                sb.Append(cki.KeyChar);
            }

            return sb.ToString();
        }
    }
}
