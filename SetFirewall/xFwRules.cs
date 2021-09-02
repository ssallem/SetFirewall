using NetFwTypeLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetFirewall
{
    public class xFwRules
    {
        public string RuleName { get; set; }
        public string RuleProgram { get; set; }
        public string Protocol { get; set; }
        public string LocalPort { get; set; }
        public string RemotePort { get; set; }
        public bool IsEnabled { get; set; }
        //public bool IsEnabled 
        //{
        //    set 
        //    { 
        //        if (value)
        //            EnableName = "사용";
        //        else
        //            EnableName = "중지";
        //    }
        //}
        public string EnableName { get; set; }
        public NET_FW_RULE_DIRECTION_ Direction { get; set; }
        public string DirectionName { get; set; }
        public NET_FW_ACTION_ Action { get; set; }
        public string ActionName { get; set; }
    }
}
