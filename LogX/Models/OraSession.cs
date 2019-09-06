using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Oracle.ManagedDataAccess.Client;

namespace LogX.Models {
    public class OraSession {
        public OracleConnection OracleConnection { get; set; }
        public string ConnectionString { get; set; }

        public OraSession() {
        }

        public void Save(string name) {
            File.WriteAllText(name + "Db.con", ConnectionString);
        }

        public string Load(string name) {
            return File.ReadAllText(name + "Db.con");
        }

        public bool IsConnectionFileExists(string name) {
            return File.Exists(name + "Db.con");
        }

        public string OpenConnection(string conString) {
            ConnectionString = conString;
            OracleConnection = new OracleConnection { ConnectionString = ConnectionString };
            try {
                OracleConnection.Open();
                return "";
            } catch (Exception ex) {
                return ex.Message;
            }           
        }

        public void CloseConnection() {
            OracleConnection.Close();
            OracleConnection.Dispose();
        }

        private List<LogLine> ExecuteQuery(OracleCommand cmd) {
            List<LogLine> result = new List<LogLine>();
            cmd.CommandType = CommandType.Text;
            using (OracleDataReader dataAdapter = cmd.ExecuteReader()) {
                while (dataAdapter.Read()) {
                    var logLine = new LogLine(
                        dataAdapter.IsDBNull(0) ? "" : dataAdapter.GetString(0),
                        dataAdapter.IsDBNull(1) ? 0 : dataAdapter.GetInt32(1),
                        dataAdapter.IsDBNull(2) ? "" : dataAdapter.GetString(2),
                        dataAdapter.IsDBNull(3) ? "" : dataAdapter.GetString(3)
                        );
                    result.Add(logLine);
                }
            }
            return result;
        }

        public  List<LogLine> AsyncGetBJGLogs(OracleCommand cmd) {
            return ExecuteQuery(cmd);
        }

        public DateTime AsyncGetLastExecuted(OracleCommand cmd) {
                cmd.CommandType = CommandType.Text;
                var lastExecuted = DateTime.MinValue;
                using (OracleDataReader dataAdapter = cmd.ExecuteReader()) {
                    while (dataAdapter.Read()) {
                        if (!dataAdapter.IsDBNull(0)) {
                            lastExecuted = dataAdapter.GetDateTime(0);
                        }
                    }
                }
                return lastExecuted;
        }
        
    }
}
