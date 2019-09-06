using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogX.Models;

namespace LogX.ViewModels {
    public class DBConnectionViewModel {
        public OraSession OraSession { get; set; }
        public string Name { get; set; }

        public DBConnectionViewModel(string name) {
            Name = name;
        }

        public string StartSession(string host, string port, string user, string password, string dbName, string sid) {
            DbConnection dbConnection = new DbConnection(Name, host, port, user, password, dbName, sid);
            OraSession = new OraSession();
            try {
                OraSession.OpenConnection(dbConnection.CreateConnectionString());
                return "";
            }catch(Exception ex) {
                return ex.Message;
            }
        }

        public string StartSession(string conString) {
            OraSession = new OraSession();
            try {
                OraSession.OpenConnection(conString);
                return "";
            } catch (Exception ex) {
                return ex.Message;
            }
        }

        public void EndSession() {
            if (OraSession != null) {
                OraSession.CloseConnection();
            }
        }

        public void Save() {
            OraSession.Save(Name);
        }

        public string Load() {
            return File.ReadAllText(Name + "Db.con");
        }

        public bool IsConnectionFileExists() {
            return File.Exists(Name + "Db.con");
        }
    }
}
