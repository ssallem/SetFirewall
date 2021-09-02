using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetFirewall
{
    public static class xCommon
    {
        public static bool ExcuteCmd(string sendMsg)
        {
            try
            {
                System.Console.WriteLine(sendMsg);
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.CreateNoWindow = true;
                startInfo.FileName = "cmd.exe";
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardInput = true;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;

                Process process = new Process();
                process.EnableRaisingEvents = false;
                process.StartInfo = startInfo;
                process.Start();
                process.StandardInput.Write(sendMsg + Environment.NewLine);
                process.StandardInput.Close();

                string result = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();
                process.Close();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
