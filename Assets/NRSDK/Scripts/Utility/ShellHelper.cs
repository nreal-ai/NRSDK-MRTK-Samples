/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal
{
    using System.Diagnostics;
    using System.Text;

    /// <summary> Misc helper methods for running shell commands. </summary>
    public static class ShellHelper
    {
        /// <summary> Run a shell command. </summary>
        /// <param name="fileName">  File name for the executable.</param>
        /// <param name="arguments"> Command line arguments, space delimited.</param>
        /// <param name="output">    [out] Filled out with the result as printed to stdout.</param>
        /// <param name="error">     [out] Filled out with the result as printed to stderr.</param>
        public static void RunCommand(string fileName, string arguments, out string output, out string error)
        {
            using (var process = new Process())
            {
                var startInfo = new ProcessStartInfo(fileName, arguments);
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardError = true;
                startInfo.RedirectStandardOutput = true;
                startInfo.CreateNoWindow = true;
                process.StartInfo = startInfo;

                var errorBuilder = new StringBuilder();
                process.ErrorDataReceived += (sender, ef) => errorBuilder.AppendLine(ef.Data);

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
                var existcode = process.ExitCode;
                process.Close();

                // Trims the output strings to make comparison easier.
                output = existcode.ToString();
                error = errorBuilder.ToString().Trim();
            }
        }

        /// <summary> Run a shell command. </summary>
        /// <param name="fileName">  File name for the executable.</param>
        /// <param name="arguments"> Command line arguments, space delimited.</param>
        public static void RunCommand(string fileName, string arguments)
        {
            using (var process = new Process())
            {
                var startInfo = new ProcessStartInfo(fileName, arguments);
                startInfo.CreateNoWindow = false;
                process.StartInfo = startInfo;
                process.Start();
            }
        }
    }
}
