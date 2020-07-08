using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.ServiceProcess;
using System.Diagnostics;
using System.Threading;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace Masterfine.Models
{
    public class DBConnection
    {
        public static IConfiguration configuration { get; set; }
        public static SqlConnection sqlcon { get; set; }
        private static string connectionstring { get; set; }
        
        public DBConnection()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();
            connectionstring = config.GetConnectionString("DefaultConnectionString");
            if (sqlcon == null)
            {
                sqlcon = new SqlConnection(connectionstring);
            }
            
            try
            {
                if (sqlcon.State == ConnectionState.Closed)
                {
                    sqlcon.Open();
                }
            }
            catch (SqlException sEx)
            {
                int i = sEx.Number;
                if (i == 233)
                {
                    Reconnect();
                }
                else if (i == -1)
                {
                    //Messages.InformationMessage("SQL connection failure... Please check the instance name of your sqlexpress with " + Application.StartupPath + " \\sys.txt");
                }
                else if (i == 18493)
                {
                    ChangeConnectionForServer();
                }
                else
                {
                    //Messages.InformationMessage("Could not connect to your database... Please check your SQL Configuration");
                }
            }
            catch (Exception ex)
            {
                ;
                //Catche any other exception
            }
            finally
            {

            }
            try
            {
                //PrintWorks.frmDBConnection.connectionString = sqlcon.ConnectionString;
            }
            catch
            {
                //sql Error
            }
        }
        /// <summary>
        /// SQL Express may take time to start up due to AutoClose Behaviour of SQLEXPRESS
        /// 
        /// </summary>
        private void Reconnect()
        {

            try
            {
                Thread.Sleep(30000);
                if (sqlcon.State == ConnectionState.Closed)
                {
                    sqlcon.Open();

                }
            }
            catch (Exception)
            {

                //Messages.InformationMessage("Your SQL Server is starting up... Please close and re-open the application");

            }
        }

        /// <summary>
        /// Cheanges the connection string to support SQLServer version instead of SQLExpress
        /// </summary>
        private void ChangeConnectionForServer()
        {
            //sqlcon = new SqlConnection(@"Data Source=" + strServer + ";AttachDbFilename=" + path + ";Integrated Security=True;Connect Timeout=120;");
        }
    }
}
