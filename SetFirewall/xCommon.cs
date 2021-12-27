using NetFwTypeLib;
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
        /// <summary>
        /// 인바운드, 아웃바운드 구분
        /// </summary>
        /// <param name="direction">NET_FW_RULE_DIRECTION_</param>
        /// <returns></returns>
        public static string GetDirectionName(NET_FW_RULE_DIRECTION_ direction)
        {
            string returnValue = "";
            switch (direction)
            {
                case NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN:
                    returnValue = "in";
                    break;
                case NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT:
                    returnValue = "out";
                    break;
                case NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_MAX:
                    returnValue = "max";
                    break;
                default:
                    break;
            }
            return returnValue;
        }

        /// <summary>
        /// 방화벽 허용, 차단 텍스트
        /// </summary>
        /// <param name="action">NET_FW_ACTION_</param>
        /// <returns></returns>
        public static string GetRuleActionName(NET_FW_ACTION_ action)
        {
            string returnValue = "";
            switch (action)
            {
                case NET_FW_ACTION_.NET_FW_ACTION_BLOCK:
                    returnValue = "차단";
                    break;
                case NET_FW_ACTION_.NET_FW_ACTION_ALLOW:
                    returnValue = "허용";
                    break;
                case NET_FW_ACTION_.NET_FW_ACTION_MAX:
                    returnValue = "최대";
                    break;
                default:
                    break;
            }
            return returnValue;
        }

        /// <summary>
        /// TCP, UDP, 모두 프로토콜 타입
        /// </summary>
        /// <param name="protocol"></param>
        /// <returns></returns>
        public static string GetRuleProtocol(int protocol)
        {
            string returnValue = "";
            switch (protocol)
            {
                case 1: returnValue = "ICMPv4"; break;
                case 2: returnValue = "ICMPv6"; break;
                case 6: returnValue = "TCP"; break;
                case 17: returnValue = "UDP"; break;
                case 47: returnValue = "GRE"; break;
                case 256: returnValue = "모두"; break;
                default:
                    break;
            }
            return returnValue;
        }

        /// <summary>
        /// Cmd창으로 명령어 전송한다.
        /// </summary>
        /// <param name="sendMsg">전송 메세지</param>
        /// <returns></returns>
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
