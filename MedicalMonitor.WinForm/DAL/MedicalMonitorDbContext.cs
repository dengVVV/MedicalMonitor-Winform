using MedicalMonitor.WinForm.DAL.Entities;
using System.Data.Entity;
using System.Configuration;

namespace MedicalMonitor.WinForm.DAL
{
    /// <summary>
    /// EF6 数据库上下文 — 负责简单 CRUD 操作（患者、设备基础查询）。
    ///
    /// 与 Dapper 分工：
    ///   EF6:  简单单表 CRUD（Patients、Devices）
    ///   Dapper: 复杂聚合查询、联表、存储过程、批量写入
    ///
    /// 连接字符串：从 App.config 的 "MedicalMonitorDb" 读取。
    /// 使用 TrustServerCertificate=True 兼容本地 SQL Server 开发环境。
    /// </summary>
    public class MedicalMonitorDbContext : DbContext
    {
        /// <summary>
        /// 使用 App.config 中名为 "MedicalMonitorDb" 的连接字符串。
        /// 构造函数注入：DI 容器通过此构造创建实例。
        /// </summary>
        public MedicalMonitorDbContext() : base("name=MedicalMonitorDb")
        {
            // EF6 默认 LazyLoadingEnabled=true，不需要额外配置
            // 生产环境可通过 Database.SetInitializer 禁用自动迁移
            Database.SetInitializer<MedicalMonitorDbContext>(null);
        }

        // ========== DbSet 属性 ==========

        /// <summary>用户表（已有 Dapper UserRepository，保留 EF6 映射供后续使用）</summary>
        public DbSet<UserEntity> Users { get; set; }

        /// <summary>患者表</summary>
        public DbSet<PatientEntity> Patients { get; set; }

        /// <summary>设备表</summary>
        public DbSet<DeviceEntity> Devices { get; set; }

        /// <summary>生理参数记录表</summary>
        public DbSet<VitalSignsRecordEntity> VitalSignsRecords { get; set; }

        /// <summary>报警事件表</summary>
        public DbSet<AlarmEventEntity> AlarmEvents { get; set; }

        /// <summary>每日统计表</summary>
        public DbSet<DailyStatisticsEntity> DailyStatistics { get; set; }

        // ========== Fluent API 配置 ==========

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // VitalSignsRecords — 复合索引（DeviceId + RecordTime）用于趋势查询
            modelBuilder.Entity<VitalSignsRecordEntity>()
                .HasIndex(e => new { e.DeviceId, e.RecordTime })
                .HasName("IX_VitalSigns_DeviceId_RecordTime");

            // AlarmEvents — TriggerTime 倒序索引（报警列表分页查询）
            modelBuilder.Entity<AlarmEventEntity>()
                .HasIndex(e => e.TriggerTime)
                .HasName("IX_AlarmEvents_TriggerTime");

            // DailyStatistics — 唯一约束（DeviceId + StatDate）
            modelBuilder.Entity<DailyStatisticsEntity>()
                .HasIndex(e => new { e.DeviceId, e.StatDate })
                .IsUnique()
                .HasName("IX_DailyStats_DeviceId_Date");
        }
    }
}
