using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogX.Models {
    public class BatchJobGroup : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        string name { get; set; }
        public string Name {
            get { return name; }
            set { name = value.ToUpper(); }
        }
        string lastExecuted;
        public string LastExecuted {
            get { return lastExecuted; }
            set {
                lastExecuted = value;
                OnPropertyChanged("LastExecuted");
            }
        }
        string result;
        public string Result {
            get { return result; }
            set {
                result = value;
                OnPropertyChanged("Result");
            }
        }
        string status;
        public string Status {
            get { return status; }
            set {
                status = value;
                OnPropertyChanged("Status");
            }
        }
        List<LogLine> MasterLogLines { get; set; } = new List<LogLine>();
        List<LogLine> TestLogLines { get; set; } = new List<LogLine>();
        public List<string> MasterLog { get; set;}
        public List<string> TestLog { get; set; }

        public BatchJobGroup() { }

        public BatchJobGroup(string name) {
            Name = name;
        }

        public void AddMasterLogLines(List<LogLine> logLines) {
            MasterLogLines.Clear();
            MasterLogLines.AddRange(logLines);
        }

        public void AddTestLogLines(List<LogLine> logLines) {
            TestLogLines.Clear();
            TestLogLines.AddRange(logLines);
        }

        private string SetStatus() {
            if (MasterLog.Count > 0 || TestLog.Count > 0) {
                return "Failed";
            } else if (MasterLogLines.Count == 0 && TestLogLines.Count == 0) {
                return "No Data Extracted";
            } else {
                return "Passed";
            }
        }
      
        public void CompareLogs() {
            MasterLog = MasterLogLines.Select(item=>item.Content).Except(TestLogLines.Select(item=>item.Content)).ToList();
            TestLog = TestLogLines.Select(item => item.Content).Except(MasterLogLines.Select(item => item.Content)).ToList();
            Result = SetStatus();
        }

        public void OnPropertyChanged(string propName) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }


    }
}
