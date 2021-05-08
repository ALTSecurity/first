using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ALTSecurity.Web.Models
{
    public class RiskModel
    {
        public int SourceCount { get; set; }

        public double TotalProbability { get; set; }

        public List<TriggeredRule> TriggeredRules { get; set; }
    }

    public class TriggeredRule
    {
        public int SourceCount { get; set; }

        public RuleType RuleType { get; set; }

        public object Data { get; set; }

        public double Probability { get; set; }

        public double ActionProbability { get; set; }
    }

    public enum RuleType
    {
        metadata = 0,
        ips = 1,
        leakage = 2,
        versions = 3
    }
}