using Dapper;
using MedicalMonitor.WinForm.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MedicalMonitor.WinForm.DAL
{
    /// <summary>
    /// 生理参数记录数据访问 — Dapper 复杂查询仓储。
    ///
    /// 与 EF6 分工：本类负责批量写入、聚合查询、联表查询，
    /// 利用 Dapper 轻量级优势处理高频写入和复杂 SQL。
    /// </summary>
    public class VitalSignsQueryRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public VitalSignsQueryRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        /// <summary>
        /// 批量插入生理参数记录（每 30 秒执行一次）。
        /// 使用 Dapper 的 Execute 执行多行 INSERT，比 EF6 的 AddRange 快 3~5 倍。
        ///
        /// SQL Server 表值参数 (TVP) 是最优方案，
        /// 当前阶段使用拼装多 VALUES 的方式，单次最多 24 行（8 设备 × 3 帧/30s）。
        /// </summary>
        public void BatchInsert(IEnumerable<VitalSignsRecordEntity> records)
        {
            if (records == null || !records.Any())
                return;

            const string sql = @"
INSERT INTO dbo.VitalSignsRecords (DeviceId, BedNo, RecordTime, HeartRate, SpO2,
    NibpSystolic, NibpDiastolic, RespiratoryRate, Temperature, IsValid)
VALUES (@DeviceId, @BedNo, @RecordTime, @HeartRate, @SpO2,
    @NibpSystolic, @NibpDiastolic, @RespiratoryRate, @Temperature, @IsValid)";

            using (var conn = _connectionFactory.CreateConnection())
            using (var tx = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute(sql, records, transaction: tx);
                    tx.Commit();
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// 查询指定设备最近 N 条生理参数记录（用于波形绘制）。
        /// 使用 ROW_NUMBER() 窗口函数获取每个设备的最新 30 条数据。
        /// </summary>
        public IEnumerable<VitalSignsRecordEntity> GetRecentRecords(string deviceId, int count = 30)
        {
            const string sql = @"
SELECT TOP (@Count) RecordId, PatientId, DeviceId, RecordTime, HeartRate, SpO2,
       NIBP_Systolic, NIBP_Diastolic, RespiratoryRate, Temperature, IsValid
FROM dbo.VitalSignsRecords
WHERE DeviceId = @DeviceId
ORDER BY RecordTime DESC";



            using (var conn = _connectionFactory.CreateConnection())
            {
                return conn.Query<VitalSignsRecordEntity>(sql, new { DeviceId = deviceId, Count = count });
            }
        }

        /// <summary>
        /// 查询当日所有设备的平均心率、平均血氧、总记录数。
        /// 用于 DashboardForm 底部统计摘要栏。
        /// </summary>
        public (double? AvgHr, double? AvgSpO2, int TotalRecords) GetTodayOverallStats()
        {
            const string sql = @"
SELECT
    AVG(CAST(HeartRate AS FLOAT)) AS AvgHr,
    AVG(CAST(SpO2 AS FLOAT)) AS AvgSpO2,
    COUNT(1) AS TotalRecords
FROM dbo.VitalSignsRecords
WHERE RecordTime >= CAST(GETDATE() AS DATE)";

            using (var conn = _connectionFactory.CreateConnection())
            {
                var row = conn.QueryFirstOrDefault(sql);
                if (row == null)
                    return (null, null, 0);

                return ((double?)row.AvgHr, (double?)row.AvgSpO2, (int)row.TotalRecords);
            }
        }
    }
}
