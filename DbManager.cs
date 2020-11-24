using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.SqlClient;

namespace CarApi
{
    public interface IDbManager : IDisposable
    {
        string DbConnectionString { get; }
        public IDbConnection GetDbConnection();
        public IDbCommand GetDbCommand(string commandString);
    }

    public class SqlDbManager : IDbManager
    {
        private SqlConnection connection;
        private SqlCommand command;
        public string DbConnectionString  => "Server=tcp:slon01.database.windows.net,1433;Initial Catalog=DB1;Persist Security Info=False;User ID=pdi;Password=Cerberos23;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public IDbConnection GetDbConnection()
        {
            this.connection = new SqlConnection(DbConnectionString);
            this.connection.Open();
            return this.connection;
        }

        public void Dispose() 
        {
            this.command?.Dispose();
            if(this.connection?.State != ConnectionState.Closed)
            {
                this.connection?.Close();
            }
            this.connection?.Dispose();
        }

        public IDbCommand GetDbCommand(string cmdString) 
        {
            if(this.connection == null)
            {
                this.GetDbConnection();
            }
            if(this.connection.State != ConnectionState.Open)
            {
                this.connection.Open();
            }

            this.command = new SqlCommand(cmdString, this.connection);
            return this.command;
        }
    }

}