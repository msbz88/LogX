using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LogX.Models;
using Oracle.ManagedDataAccess.Client;

namespace LogX.ViewModels {
    public class BatchJobGropViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<BatchJobGroup> BatchJobGroups { get; set; }
        public EventHandler ErrorConnection { get; set; }
        public EventHandler LogsExtracted { get; set; }
        public EventHandler Error { get; set; }
        OraSession MasterSession { get; set; }
        OraSession TestSession { get; set; }
        DateTime fromDate;
        public DateTime FromDate {
            get { return fromDate; }
            set {
                fromDate = value;
                OnPropertyChanged("FromDate");
            }
        }

        public BatchJobGropViewModel() {
            BatchJobGroups = new ObservableCollection<BatchJobGroup>();
            FromDate = GetProjectStartDate();
        }

        public void InsertFromClipBoard(int index) {
            var text = Clipboard.GetText();
            var data = text.Split(new string[] { Environment.NewLine },StringSplitOptions.None);
            foreach (var item in data) {
                var firstCol = item.Split('\t')[0];
                bool isExists = BatchJobGroups.Any(i => i.Name == firstCol);
                if (isExists) {
                    Error?.Invoke("Duplicate item(s) deleted.", null);
                }
                if (firstCol != "" && !isExists) {
                    var bjg = new BatchJobGroup(firstCol);
                    if (index < BatchJobGroups.Count) {
                        BatchJobGroups.RemoveAt(index);
                        BatchJobGroups.Insert(index, bjg);
                    } else {
                        BatchJobGroups.Add(bjg);
                    }
                    index++;
                }             
            }
        }

        public void GetFomDatabase() {
            if (BatchJobGroups.Count > 0) {
                OraSession oraSession = new OraSession();
                var masterConString = oraSession.Load("Master");
                var testConString = oraSession.Load("Test");
                string errorMessage = "";
                errorMessage = oraSession.OpenConnection(masterConString);
                if (errorMessage != "") {
                    ErrorConnection?.Invoke(null, null);
                    return;
                }
                errorMessage = oraSession.OpenConnection(testConString);
                if (errorMessage != "") {
                    ErrorConnection?.Invoke(null, null);
                    return;
                } else {
                    foreach (var bjg in BatchJobGroups) {
                            ExtractLogs(bjg);
                            bjg.CompareLogs();
                    }
                    LogsExtracted?.Invoke(BatchJobGroups.Where(item=>item.Name!="").ToList(), null);
                }
            }  
        }

        private OraSession OpenConnection(string name) {
            var oraSession = new OraSession();
            var conString = oraSession.Load(name);
            oraSession.OpenConnection(conString);
            return oraSession;
        }

        public void ExtractLogs(BatchJobGroup bjg) {
            string query = File.ReadAllText("getLog.query");
            var masterSession = OpenConnection("Master");
            var testSession = OpenConnection("Test");
            OracleCommand masterCmd = new OracleCommand(query, masterSession.OracleConnection);
            OracleCommand testCmd = new OracleCommand(query, testSession.OracleConnection);
            masterCmd.Parameters.Add(":bjgName", OracleDbType.Varchar2).Value = bjg.Name;
            masterCmd.Parameters.Add(":fromDate", OracleDbType.TimeStamp).Value = FromDate;
            testCmd.Parameters.Add(":bjgName", OracleDbType.Varchar2).Value = bjg.Name;
            testCmd.Parameters.Add(":fromDate", OracleDbType.TimeStamp).Value = FromDate;
            var masterExtract = masterSession.AsyncGetBJGLogs(masterCmd);
            var testExtract = testSession.AsyncGetBJGLogs(testCmd);
            bjg.AddMasterLogLines(masterExtract);
            bjg.AddTestLogLines(testExtract);
            masterSession.CloseConnection();
            testSession.CloseConnection();
        }

        public DateTime GetProjectStartDate() {
            string query = "select max(laststartts) from BATCHSTATS";           
            var masterSession = new OraSession();
            var conString = masterSession.Load("Master");
            masterSession.OpenConnection(conString);
            OracleCommand masterCmd = new OracleCommand(query, masterSession.OracleConnection);
            var result = masterSession.AsyncGetLastExecuted(masterCmd);
            masterSession.CloseConnection();
            return result;
        }

        public void OnPropertyChanged(string propName) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public void SaveAsScope() {
            File.WriteAllLines("BJG_scope.txt", BatchJobGroups.Select(item=>item.Name));
        }

        public void LoadFromScope() {
            var names = File.ReadAllLines("BJG_scope.txt");
            BatchJobGroups.Clear();
            foreach (var name in names) {
                var bjg = new BatchJobGroup(name);
                BatchJobGroups.Add(bjg);
            }
        }
    }
}
