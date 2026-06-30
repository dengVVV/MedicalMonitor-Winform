using Dapper;
using MedicalMonitor.WinForm.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MedicalMonitor.WinForm.DAL
{
    /// <summary>
    /// 每日统计仓储 — Dapper 复杂聚合查询。
    ///
    /// 核心 SQL 使用 AVG/MAX/MIN/STDEV + GROUP BY + CAST AS DATE，
    /// 这些 SQL Server 内置函数和分组操作，Dapper 比 EF6 更合适。
    /// </summary>
    public class StatisticsRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public StatisticsRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        /// <summary>
        /// 计算当日每个设备的生理参数统计（AVG/MAX/MIN/STDEV）。
        /// 使用 SQL Server 内置聚合函数，效率优于应用层聚合。
        /// </summary>
        public IEnumerable<DailyStatisticsEntity> CalculateDailyStatistics(DateTime statDate)
        {
            const string sql = @"
SELECT
    DeviceId,
    CAST(@StatDate AS DATE) AS StatDate,
    AVG(CAST(HeartRate AS FLOAT))   AS AvgHeartRate,
    AVG(CAST(SpO2 AS FLOAT))        AS AvgSpO2,
    AVG(CAST(NibpSystolic AS FLOAT)) AS AvgNibpSystolic,
    AVG(CAST(NibpDiastolic AS FLOAT)) AS AvgNibpDiastolic,
    AVG(CAST(RespiratoryRate AS FLOAT)) AS AvgRespiratoryRate,
    AVG(CAST(Temperature AS FLOAT)) AS AvgTemperature,
    MAX(HeartRate)                  AS MaxHeartRate,
    MIN(HeartRate)                  AS MinHeartRate,
    STDEV(CAST(HeartRate AS FLOAT)) AS StdDevHeartRate,
    COUNT(1)                        AS RecordCount,
    SYSDATETIME()                   AS CalculatedAt
FROM dbo.VitalSignsRecords
WHERE CAST(RecordTime AS DATE) = CAST(@StatDate AS DATE)
GROUP BY DeviceId";

            using (var conn = _connectionFactory.CreateConnection())
            {
                return conn.Query<DailyStatisticsEntity>(sql, new { StatDate = statDate });
            }
        }

        /// <summary>
        /// 批量 Upsert 每日统计数据（存在则更新，不存在则插入）。
        /// 使用 MERGE 语句保证原子性，避免先 DELETE 后 INSERT 导致的数据真空。
        /// </summary>
        public void UpsertDailyStatistics(IEnumerable<DailyStatisticsEntity> stats)
        {
            if (stats == null || !stats.Any())
                return;

            const string sql = @"
MERGE dbo.DailyStatistics AS target
USING (SELECT @DeviceId AS DeviceId, CAST(@StatDate AS DATE) AS StatDate) AS source
ON target.DeviceId = source.DeviceId AND CAST(target.StatDate AS DATE) = source.StatDate
WHEN MATCHED THEN UPDATE SET
    AvgHeartRate       = @AvgHeartRate,
    AvgSpO2            = @AvgSpO2,
    AvgNibpSystolic    = @AvgNibpSystolic,
    AvgNibpDiastolic   = @AvgNibpDiastolic,
    AvgRespiratoryRate = @AvgRespiratoryRate,
    AvgTemperature     = @AvgTemperature,
    MaxHeartRate       = @MaxHeartRate,
    MinHeartRate       = @MinHeartRate,
    StdDevHeartRate    = @StdDevHeartRate,
    RecordCount        = @RecordCount,
    CalculatedAt       = SYSDATETIME()
WHEN NOT MATCHED THEN INSERT
    (DeviceId, StatDate, AvgHeartRate, AvgSpO2, AvgNibpSystolic, AvgNibpDiastolic,
     AvgRespiratoryRate, AvgTemperature, MaxHeartRate, MinHeartRate, StdDevHeartRate,
     RecordCount, CalculatedAt)
VALUES
    (@DeviceId, CAST(@StatDate AS DATE), @AvgHeartRate, @AvgSpO2, @AvgNibpSystolic,
     @AvgNibpDiastolic, @AvgRespiratoryRate, @AvgTemperature, @MaxHeartRate,
     @MinHeartRate, @StdDevHeartRate, @RecordCount, SYSDATETIME());";

            using (var conn = _connectionFactory.CreateConnection())
            using (var tx = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute(sql, stats, transaction: tx);
                    tx.Commit();
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
        }
    }
}
