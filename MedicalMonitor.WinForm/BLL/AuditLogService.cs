using Dapper;
using MedicalMonitor.WinForm.DAL;
using MedicalMonitor.WinForm.DAL.Entities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalMonitor.WinForm.BLL
{
    /// <summary>
    /// 审计日志服务 — 记录所有关键操作，满足 IEC 62304 合规要求。
    ///
    /// 设计决策：
    ///   - 所有 Log 方法通过 Task.Run 异步写入 DB，不阻塞业务流程
    ///   - 写入失败仅记录 Serilog 日志，不抛异常
    /// </summary>
    public class AuditLogService
    {
        private readonly DbConnectionFactory _connectionFactory;

        public AuditLogService(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        /// <summary>
        /// 异步记录审计日志（fire-and-forget，不阻塞调用方）
        /// </summary>
        public void Log(
            int userId, string action, string tableName, string recordId,
            string description, string oldValue = null, string newValue = null)
        {
            var entity = new AuditLogEntity
            {
                UserId = userId,
                Action = action,
                TableName = tableName,
                RecordId = recordId,
                Description = description,
                OldValue = oldValue,
                NewValue = newValue,
                IpAddress = Environment.MachineName,
                CreatedAt = DateTime.Now,
            };

            // 异步写入，不阻塞主流程
            Task.Run(() =>
            {
                try
                {
                    const string sql = @"
INSERT INTO dbo.AuditLogs (UserId, Action, TableName, RecordId, Description, OldValue, NewValue, IpAddress, CreatedAt)
VALUES (@UserId, @Action, @TableName, @RecordId, @Description, @OldValue, @NewValue, @IpAddress, @CreatedAt)";

                    using (var conn = _connectionFactory.CreateConnection())
                    {
                        conn.Execute(sql, entity);
                    }
                }
                catch (Exception ex)
                {
                    Serilog.Log.Error(ex, "写入审计日志失败 Action={Action}", action);
                }
            });
        }

        /// <summary>按用户查询审计日志</summary>
        public List<AuditLogEntity> GetByUser(int userId, int days = 7)
        {
            const string sql = @"
SELECT * FROM dbo.AuditLogs
WHERE UserId = @UserId AND CreatedAt >= @Since
ORDER BY CreatedAt DESC";

            using (var conn = _connectionFactory.CreateConnection())
            {
                return conn.Query<AuditLogEntity>(sql,
                    new { UserId = userId, Since = DateTime.Now.AddDays(-days) }).ToList();
            }
        }

        /// <summary>按操作类型查询审计日志</summary>
        public List<AuditLogEntity> GetByAction(string action, int days = 7)
        {
            const string sql = @"
SELECT * FROM dbo.AuditLogs
WHERE Action = @Action AND CreatedAt >= @Since
ORDER BY CreatedAt DESC";

            using (var conn = _connectionFactory.CreateConnection())
            {
                return conn.Query<AuditLogEntity>(sql,
                    new { Action = action, Since = DateTime.Now.AddDays(-days) }).ToList();
            }
        }

        /// <summary>分页查询（用于审计日志查看界面）</summary>
        public (List<AuditLogEntity> Items, int Total) QueryPaged(int page, int pageSize,
            int? userId = null, string action = null)
        {
            using (var conn = _connectionFactory.CreateConnection())
            {
                var where = new List<string>();
                var param = new DynamicParameters();
                if (userId.HasValue) { where.Add("UserId = @UserId"); param.Add("UserId", userId.Value); }
                if (!string.IsNullOrEmpty(action)) { where.Add("Action = @Action"); param.Add("Action", action); }

                string whereClause = where.Count > 0 ? "WHERE " + string.Join(" AND ", where) : "";

                string countSql = $"SELECT COUNT(*) FROM dbo.AuditLogs {whereClause}";
                int total = conn.ExecuteScalar<int>(countSql, param);

                string dataSql = $@"
SELECT * FROM dbo.AuditLogs {whereClause}
ORDER BY CreatedAt DESC
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
                param.Add("Offset", (page - 1) * pageSize);
                param.Add("PageSize", pageSize);

                var items = conn.Query<AuditLogEntity>(dataSql, param).ToList();
                return (items, total);
            }
        }
    }
}
