using Dapper;
using MedicalMonitor.WinForm.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MedicalMonitor.WinForm.DAL
{
    /// <summary>
    /// 报警事件数据访问 — Dapper。
    /// 处理 AlarmEvents 表的增删改查。
    /// </summary>
    public class AlarmEventRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public AlarmEventRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        /// <summary>插入一条新报警</summary>
        public void Insert(AlarmEventEntity entity)
        {
            const string sql = @"
INSERT INTO dbo.AlarmEvents (AlarmId, PatientId, DeviceId, ParameterType, ActualValue, ThresholdValue, AlarmLevel, TriggerTime, Message)
VALUES (@AlarmId, @PatientId, @DeviceId, @ParameterType, @ActualValue, @ThresholdValue, @AlarmLevel, @TriggerTime, @Message)";

            using (var conn = _connectionFactory.CreateConnection())
            {
                conn.Execute(sql, entity);
            }
        }

        /// <summary>确认报警</summary>
        public void Acknowledge(string alarmId, string acknowledgedBy)
        {
            const string sql = @"
UPDATE dbo.AlarmEvents
SET AcknowledgedTime = SYSDATETIME(), AcknowledgedBy = @AcknowledgedBy
WHERE AlarmId = @AlarmId AND AcknowledgedTime IS NULL";

            using (var conn = _connectionFactory.CreateConnection())
            {
                conn.Execute(sql, new { AlarmId = alarmId, AcknowledgedBy = acknowledgedBy });
            }
        }

        /// <summary>获取所有未确认的报警</summary>
        public List<AlarmEventEntity> GetUnacknowledged()
        {
            const string sql = @"
SELECT * FROM dbo.AlarmEvents
WHERE AcknowledgedTime IS NULL
ORDER BY TriggerTime DESC";

            using (var conn = _connectionFactory.CreateConnection())
            {
                return conn.Query<AlarmEventEntity>(sql).ToList();
            }
        }

        /// <summary>按设备获取最近报警</summary>
        public List<AlarmEventEntity> GetRecentByDevice(string deviceId, int hours = 24)
        {
            const string sql = @"
SELECT * FROM dbo.AlarmEvents
WHERE DeviceId = @DeviceId AND TriggerTime >= @Since
ORDER BY TriggerTime DESC";

            using (var conn = _connectionFactory.CreateConnection())
            {
                return conn.Query<AlarmEventEntity>(sql, new { DeviceId = deviceId, Since = DateTime.Now.AddHours(-hours) }).ToList();
            }
        }

        /// <summary>分页查询报警（支持按确认状态、等级筛选）</summary>
        public (List<AlarmEventEntity> Items, int Total) QueryPaged(
            int page, int pageSize, bool? acknowledged = null, int? alarmLevel = null)
        {
            using (var conn = _connectionFactory.CreateConnection())
            {
                var where = new List<string>();
                if (acknowledged.HasValue)
                    where.Add(acknowledged.Value ? "AcknowledgedTime IS NOT NULL" : "AcknowledgedTime IS NULL");
                if (alarmLevel.HasValue)
                    where.Add($"AlarmLevel = {alarmLevel.Value}");

                string whereClause = where.Count > 0 ? "WHERE " + string.Join(" AND ", where) : "";

                string countSql = $"SELECT COUNT(*) FROM dbo.AlarmEvents {whereClause}";
                int total = conn.ExecuteScalar<int>(countSql);

                string dataSql = $@"
SELECT * FROM dbo.AlarmEvents {whereClause}
ORDER BY TriggerTime DESC
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                var items = conn.Query<AlarmEventEntity>(dataSql,
                    new { Offset = (page - 1) * pageSize, PageSize = pageSize }).ToList();

                return (items, total);
            }
        }
    }
}
