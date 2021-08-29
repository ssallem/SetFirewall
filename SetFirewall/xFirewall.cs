using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetFwTypeLib;

namespace SetFirewall
{
    public class xFirewall
    {
        /// <summary>
        /// 방화벽 사용 여부 조회
        /// </summary>
        public bool GetFirewallEnable()
        {
            // 2개 중에 1개라도 방화벽 걸려있으면 방화벽 On으로 리턴한다..
            // NET_FW_PROFILE_STANDARD = 개인 네트워크, NET_FW_PROFILE_DOMAIN = 공용 네트워크
            return mFirewallMng.LocalPolicy.GetProfileByType(NET_FW_PROFILE_TYPE_.NET_FW_PROFILE_STANDARD).FirewallEnabled &&
                   mFirewallMng.LocalPolicy.GetProfileByType(NET_FW_PROFILE_TYPE_.NET_FW_PROFILE_DOMAIN).FirewallEnabled;
            
        }
        public xFirewall()
        {
            // firewall manager 객체 생성
            Type objectType = Type.GetTypeFromCLSID(new Guid(CLSID_FIREWALL_MANAGER));
            mFirewallMng = Activator.CreateInstance(objectType) as INetFwMgr;
        }

        #region 방화벽 설정 : CMD 방식
        private readonly string FirewallCmd = "netsh firewall add allowedprogram \"{1}\" \"{0}\" ENABLE"; 
        private readonly string AdvanceFirewallCmd = "netsh advfirewall firewall add rule name=\"{0}\" dir=in action=allow program=\"{1}\" enable=yes";
        private readonly string FirewallEnable = "netsh advfirewall set allprofile state {0}";
        private readonly int VistaMajorVersion = 6;

        /// <summary>
        /// 방화벽 사용 설정/해제
        /// </summary>
        /// <param name="isOn">설정/해제</param>
        /// <returns></returns>
        public bool FirewallOnOff(bool isOn)
        {
            try
            {
                string stateString = "off";
                if (isOn)
                {
                    stateString = "on";
                }

                // Start to register
                string command = String.Format(FirewallEnable, stateString);
                System.Console.WriteLine(command);
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
                process.StandardInput.Write(command + Environment.NewLine);
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

        /// <summary>
        /// 방화벽 예외 등록
        /// </summary>
        /// <param name="name">프로그램 명</param>
        /// <param name="programFullPath">프로그램 풀 경로</param>
        /// <returns></returns>
        public bool AuthorizeProgram(string name, string programFullPath)
        {
            try
            { 
                //OS version check
                string strFormat = this.FirewallCmd; 

                if (System.Environment.OSVersion.Version.Major >= this.VistaMajorVersion) 
                { 
                    strFormat = this.AdvanceFirewallCmd; 
                } 
                
                // Start to register
                string command = String.Format(strFormat, name, programFullPath); 
                System.Console.WriteLine(command); 
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
                process.StandardInput.Write(command + Environment.NewLine); 
                process.StandardInput.Close(); 

                string result = process.StandardOutput.ReadToEnd(); 
                string error = process.StandardError.ReadToEnd(); 

                process.WaitForExit(); 
                process.Close(); 
            } 
            catch 
            { 
                return false; 
            } 
            return true; 
        }
        #endregion

        #region C:\Windows\System32\FirewallAPI.dll 참조

        private bool mListAdded; // 방화벽 앱 목록에 추가되어있는지 여부
        private bool mEnabled;   // 방화벽 앱 목록에 '허용'으로 되어있는지 여부

        private const string CLSID_FIREWALL_MANAGER = "{304CE942-6E39-40D8-943A-B913C40C9CD4}";
        private INetFwMgr mFirewallMng = null;

        /// <summary>
        /// 방화벽 이름으로 방화벽 리스트 등록 여부와 허용 여부를 반환한다.
        /// </summary>
        /// <param name="appPathName">방화벽 이름</param>
        /// <returns>방화벽 목록 존재 여부, 방화벽 허용 여부</returns>
        public (bool, bool) GetFwAllowAppInfo(string appPathName)
        {
            INetFwAuthorizedApplication authoredApp = FindFwAllowApp(appPathName);

            if (authoredApp == null)
            {
                mListAdded = false;
                mEnabled = false;
            }
            else
            {
                mListAdded = true;
                mEnabled = authoredApp.Enabled;
            }

            return (mListAdded, mEnabled);
        }

        /// <summary>
        /// 방화벽 허용 여부를 반환한다.
        /// </summary>
        /// <param name="appPathName"></param>
        /// <returns></returns>
        private INetFwAuthorizedApplication FindFwAllowApp(string appPathName)
        {   
            foreach (INetFwAuthorizedApplication app in mFirewallMng.LocalPolicy.CurrentProfile.AuthorizedApplications)
            {
                // 일치하는 앱을 찾음
                Console.WriteLine(app.ProcessImageFileName);
                if (app.ProcessImageFileName.ToLower().Equals(appPathName.ToLower()))
                {
                    return app;
                }
            }
            return null;
        }

        /// <summary>
        /// C:\Windows\System32\FirewallAPI.dll 참조
        /// </summary>
        public void GetFirewallInfo()
        {
            Type tNetFwPolicy2 = Type.GetTypeFromProgID("HNetCfg.FwPolicy2");
            INetFwPolicy2 fwPolicy2 = (INetFwPolicy2)Activator.CreateInstance(tNetFwPolicy2);

            // Let's create a new rule
            INetFwRule2 inboundRule = (INetFwRule2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
            inboundRule.Enabled = true;

            //Allow through firewall
            inboundRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;

            //For all profile
            inboundRule.Profiles = (int)NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_ALL;

            //Using protocol TCP
            inboundRule.Protocol = 6; // TCP
                                      //Local Port 1433
            inboundRule.LocalPorts = "1433";
            //Name of rule
            inboundRule.Name = "SQLRule";

            // Now add the rule
            INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
            firewallPolicy.Rules.Add(inboundRule);
        }
        #endregion
    }
}
