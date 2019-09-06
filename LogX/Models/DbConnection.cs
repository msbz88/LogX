using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogX.Models {
    public class DbConnection {
        public string Name { get; set; }
        public string Schema { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }
        public string ServiceName { get; set; }
        public string Sid { get; set; }

        public DbConnection(string name, string host, string port, string schema, string password, string serviceName, string sid) {
            Name = name;
            Host = host;
            Port = port;
            Schema = schema;
            Password = password;
            ServiceName = serviceName;
            Sid = sid;
        }

        public DbConnection() { }

        public string CreateConnectionString() {
            var serviceNameOrSid = "";
            if (Sid == "") {
                serviceNameOrSid = "SERVICE_NAME=" + ServiceName;
            } else {
                serviceNameOrSid = "SID=" + Sid;
            }
            return "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(" +
                   "HOST=" + Host + ")(" +
                   "PORT=" + Port + ")))(" +
                   "CONNECT_DATA=(" + serviceNameOrSid + "))); " +
                   "USER ID=" + Schema + "; " +
                   "PASSWORD=" + Password + ";";
        }
    }
}
