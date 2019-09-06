using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogX.Models {
    public class LogLine {
        public string BJob { get; set; }
        public int Order { get; set; }
        public string LogCode { get; set; }
        public string Content { get; set; }

        public LogLine(string bJob, int order, string logCode, string content) {
            BJob = bJob;
            Order = order;
            LogCode = logCode;
            Content = content;
        }
    }
}
