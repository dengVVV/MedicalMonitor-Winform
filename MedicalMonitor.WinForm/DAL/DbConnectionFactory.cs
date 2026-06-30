using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace MedicalMonitor.WinForm.DAL
{
    /// <summary>
    /// SQL Server 数据库连接工厂。
    /// </summary>
    public class DbConnectionFactory
    {
        private readonly string _connectionString;

        /// <summary>
        /// 从 App.config 的 connectionStrings 节读取名为 "MedicalMonitorDb" 的连接串。
        /// </summary>
        public DbConnectionFactory()
        {
            var connSetting = ConfigurationManager.ConnectionStrings["MedicalMonitorDb"];
            if (connSetting == null)
                throw new ConfigurationErrorsException(
                    "未在 App.config 中找到名为 'Medical_Device_Monitor_DB' 的连接字符串配置。");

            _connectionString = connSetting.ConnectionString;
        }

        /// <summary>
        /// 创建并返回一个新的 SqlConnection，调用方负责 Dispose。
        /// </summary>
        public IDbConnection CreateConnection()
        {
            var conn = new SqlConnection(_connectionString);
            conn.Open();
            return conn;
        }
    }
}
