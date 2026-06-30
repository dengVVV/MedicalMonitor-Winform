
USE Medical_Device_Monitor_DB;

GO

-- ============================================================
-- 用户表 — 存储系统用户及登录凭证
-- 对应业务：登录认证（BLL.AuthenticationService）
-- ============================================================
CREATE TABLE dbo.Users
(
    UserId          INT IDENTITY(1,1)   NOT NULL,
    Username        NVARCHAR(50)        NOT NULL,
    PasswordHash    NVARCHAR(256)       NOT NULL,   -- BCrypt / SHA256 哈希值
    Role            NVARCHAR(20)        NOT NULL,   -- 护士 / 医生 / 管理员
    FullName        NVARCHAR(50)        NOT NULL,   -- 真实姓名
    IsActive        BIT                 NOT NULL DEFAULT 1,
    CreatedAt       DATETIME2(3)        NOT NULL DEFAULT SYSDATETIME(),
    LastLoginAt     DATETIME2(3)        NULL,

    CONSTRAINT PK_Users PRIMARY KEY CLUSTERED (UserId),
    CONSTRAINT UQ_Users_Username UNIQUE (Username)
);
GO

-- 用户名查询（登录高频操作）
CREATE NONCLUSTERED INDEX IX_Users_Username
    ON dbo.Users (Username)
    INCLUDE (PasswordHash, Role, FullName, IsActive);
GO

-- 角色筛选（管理员查询用户列表）
CREATE NONCLUSTERED INDEX IX_Users_Role
    ON dbo.Users (Role)
    WHERE (IsActive = 1);
GO

-- ============================================================
-- 默认测试账号（与 AuthenticationService 硬编码一致）
-- 第五阶段改为 BCrypt 哈希
-- ============================================================
INSERT INTO dbo.Users (Username, PasswordHash, Role, FullName)
VALUES
    (N'nurse01',  N'123456', N'护士',   N'张护士'),
    (N'doctor01', N'123456', N'医生',   N'李医生'),
    (N'admin',    N'admin123', N'管理员', N'系统管理员');
GO




-- ============================================================
-- 患者表 — 被监护患者的基本信息
-- 对应 Model：PatientModel
-- ============================================================
CREATE TABLE dbo.Patients
(
    PatientId       NVARCHAR(20)        NOT NULL,   -- 住院号，如 "ZY202606001"
    Name            NVARCHAR(50)        NOT NULL,   -- 患者姓名（敏感信息，建议加密存储）
    BedNumber       NVARCHAR(20)        NOT NULL,   -- 床位号，如 "ICU-01"
    Age             INT                 NOT NULL,
    Gender          NCHAR(1)            NOT NULL,   -- M / F / U
    Diagnosis       NVARCHAR(500)       NULL,       -- 入院诊断
    AdmissionTime   DATETIME2(3)        NOT NULL,
    DischargeTime   DATETIME2(3)        NULL,       -- 出院时间（null 表示在院）
    BoundDeviceId   NVARCHAR(50)        NULL,       -- 绑定的监护仪设备ID
    Status          NVARCHAR(20)        NOT NULL DEFAULT N'在院', -- 在院 / 出院 / 转科
    CreatedAt       DATETIME2(3)        NOT NULL DEFAULT SYSDATETIME(),

    CONSTRAINT PK_Patients PRIMARY KEY CLUSTERED (PatientId)
);
GO

-- 按床位号查询（护士站常用操作）
CREATE NONCLUSTERED INDEX IX_Patients_BedNumber
    ON dbo.Patients (BedNumber)
    WHERE (Status = N'在院');
GO

-- 按绑定设备查询（通过设备ID反向查患者）
CREATE NONCLUSTERED INDEX IX_Patients_BoundDevice
    ON dbo.Patients (BoundDeviceId)
    WHERE (BoundDeviceId IS NOT NULL AND Status = N'在院');
GO

-- 按入院时间查询（日报 / 统计）
CREATE NONCLUSTERED INDEX IX_Patients_AdmissionTime
    ON dbo.Patients (AdmissionTime DESC);
GO





-- ============================================================
-- 设备表 — 接入上位机的医疗设备注册信息
-- 对应 Model：DeviceModel
-- ============================================================
CREATE TABLE dbo.Devices
(
    DeviceId            NVARCHAR(50)    NOT NULL,   -- 设备唯一标识，如 "MON-001"
    DeviceType          TINYINT         NOT NULL,   -- 1=监护仪 2=输液泵 3=注射泵
    ModelName           NVARCHAR(100)   NOT NULL,   -- 型号名称，如 "mindray PM-9000"
    PortName            NVARCHAR(20)    NULL,       -- 串口名，如 "COM3"
    BaudRate            INT             NULL,       -- 波特率
    IsOnline            BIT             NOT NULL DEFAULT 0,
    LastDataReceivedTime DATETIME2(3)   NULL,       -- 心跳检测：最近收数时间
    ConnectedTime       DATETIME2(3)    NULL,       -- 最近一次连接建立时间
    RegisteredAt        DATETIME2(3)    NOT NULL DEFAULT SYSDATETIME(),

    CONSTRAINT PK_Devices PRIMARY KEY CLUSTERED (DeviceId)
);
GO

-- 按设备类型筛选
CREATE NONCLUSTERED INDEX IX_Devices_DeviceType
    ON dbo.Devices (DeviceType);
GO

-- 按在线状态查询（轮询活跃设备）
CREATE NONCLUSTERED INDEX IX_Devices_IsOnline
    ON dbo.Devices (IsOnline)
    INCLUDE (DeviceId, DeviceType, ModelName)
    WHERE (IsOnline = 1);
GO





-- ============================================================
-- 生理参数历史表 — 存储每一帧解析后的生理参数
-- 对应 Model：VitalSignsModel
--
-- 存储策略：
--   高频采集（2秒/帧）：只存 5 分钟聚合数据，不存全量原始帧
--   趋势数据（5分钟/点）：全量存入本表，用于趋势图和历史回放
-- ============================================================
CREATE TABLE dbo.VitalSignsRecords
(
    RecordId        BIGINT IDENTITY(1,1) NOT NULL,
    PatientId       NVARCHAR(20)         NOT NULL,   -- 关联患者
    DeviceId        NVARCHAR(50)         NOT NULL,   -- 来源设备
    RecordTime      DATETIME2(3)         NOT NULL,   -- 数据采集时间（设备端）
    HeartRate       DECIMAL(6,1)         NULL,       -- 心率 bpm，null = 信号丢失
    SpO2            DECIMAL(5,1)         NULL,       -- 血氧 %
    NIBP_Systolic   DECIMAL(6,1)         NULL,       -- 收缩压 mmHg
    NIBP_Diastolic  DECIMAL(6,1)         NULL,       -- 舒张压 mmHg
    RespiratoryRate DECIMAL(5,1)         NULL,       -- 呼吸率 次/分
    Temperature     DECIMAL(4,1)         NULL,       -- 体温 ℃
    IsValid         BIT                  NOT NULL DEFAULT 1, -- 数据是否通过校验
    CreatedAt       DATETIME2(3)         NOT NULL DEFAULT SYSDATETIME(), -- 入库时间

    CONSTRAINT PK_VitalSignsRecords PRIMARY KEY CLUSTERED (RecordId)
);
GO

-- 按患者+时间查询趋势（最核心查询：波形回放、趋势图）
CREATE NONCLUSTERED INDEX IX_VitalSigns_Patient_Time
    ON dbo.VitalSignsRecords (PatientId, RecordTime DESC)
    INCLUDE (HeartRate, SpO2, NIBP_Systolic, NIBP_Diastolic, RespiratoryRate, Temperature);
GO

-- 按设备+时间查询
CREATE NONCLUSTERED INDEX IX_VitalSigns_Device_Time
    ON dbo.VitalSignsRecords (DeviceId, RecordTime DESC);
GO

-- 按数据有效性过滤（数据质量审计）
CREATE NONCLUSTERED INDEX IX_VitalSigns_IsValid_Time
    ON dbo.VitalSignsRecords (IsValid, RecordTime DESC)
    WHERE (IsValid = 0);
GO

-- 按入库时间清理（定期归档/删除过期数据）
CREATE NONCLUSTERED INDEX IX_VitalSigns_CreatedAt
    ON dbo.VitalSignsRecords (CreatedAt);
GO






-- ============================================================
-- 报警事件表 — 所有生理报警与技术报警记录
-- 对应 Model：AlarmEventModel
--
-- 业务规则：
--   同一参数在同一次越界期间只产生一条事件，
--   直到参数回到正常范围后再次越界才产生新事件（防报警风暴）。
-- ============================================================
CREATE TABLE dbo.AlarmEvents
(
    AlarmId             CHAR(32)            NOT NULL,   -- UUID(无连字符)，如 "a1b2c3..."
    PatientId           NVARCHAR(20)        NOT NULL,
    DeviceId            NVARCHAR(50)        NOT NULL,
    ParameterType       TINYINT             NOT NULL,   -- 1=HR 2=SpO2 3=NIBP_SYS 4=NIBP_DIA 5=RESP 6=TEMP
    ActualValue         DECIMAL(8,2)        NOT NULL,   -- 触发报警时的实际参数值
    ThresholdValue      DECIMAL(8,2)        NOT NULL,   -- 越界的阈值边界
    AlarmLevel          TINYINT             NOT NULL,   -- 1=高 2=中 3=低
    TriggerTime         DATETIME2(3)        NOT NULL,
    AcknowledgedTime    DATETIME2(3)        NULL,       -- null = 未确认
    AcknowledgedBy      NVARCHAR(50)        NULL,       -- 确认人工号
    IsAcknowledged      AS (CASE WHEN AcknowledgedTime IS NOT NULL THEN 1 ELSE 0 END) PERSISTED,
    Message             NVARCHAR(500)       NULL,       -- 报警描述
    CreatedAt           DATETIME2(3)        NOT NULL DEFAULT SYSDATETIME(),

    CONSTRAINT PK_AlarmEvents PRIMARY KEY CLUSTERED (AlarmId)
);
GO

-- 按患者查报警历史
CREATE NONCLUSTERED INDEX IX_AlarmEvents_Patient_Time
    ON dbo.AlarmEvents (PatientId, TriggerTime DESC)
    INCLUDE (ParameterType, ActualValue, ThresholdValue, AlarmLevel, IsAcknowledged);
GO

-- 未确认报警查询（护士站报警列表，高频轮询）
CREATE NONCLUSTERED INDEX IX_AlarmEvents_Unacknowledged
    ON dbo.AlarmEvents (TriggerTime DESC)
    INCLUDE (PatientId, ParameterType, ActualValue, AlarmLevel, Message)
    WHERE (AcknowledgedTime IS NULL);
GO

-- 按报警等级筛选
CREATE NONCLUSTERED INDEX IX_AlarmEvents_Level_Time
    ON dbo.AlarmEvents (AlarmLevel, TriggerTime DESC);
GO







-- ============================================================
-- 审计日志表 — 记录所有关键操作
-- 对应 IEC 62304 合规要求：软件维护与审计追溯
-- ============================================================
CREATE TABLE dbo.AuditLogs
(
    LogId           BIGINT IDENTITY(1,1)   NOT NULL,
    UserId          INT                     NOT NULL,   -- 操作用户
    Action          NVARCHAR(50)            NOT NULL,   -- 操作类型：Login / Logout / CreatePatient / AcknowledgeAlarm / ...
    TableName       NVARCHAR(50)            NULL,       -- 操作表名
    RecordId        NVARCHAR(100)           NULL,       -- 操作记录ID
    Description     NVARCHAR(500)           NOT NULL,   -- 操作描述
    OldValue        NVARCHAR(MAX)           NULL,       -- 变更前值（JSON）
    NewValue        NVARCHAR(MAX)           NULL,       -- 变更后值（JSON）
    IpAddress       NVARCHAR(50)            NULL,       -- 操作终端IP
    CreatedAt       DATETIME2(3)            NOT NULL DEFAULT SYSDATETIME(),

    CONSTRAINT PK_AuditLogs PRIMARY KEY CLUSTERED (LogId)
);
GO

-- 按时间倒序（最近日志查询）
CREATE NONCLUSTERED INDEX IX_AuditLogs_CreatedAt
    ON dbo.AuditLogs (CreatedAt DESC);
GO

-- 按用户+时间（审计某用户的操作记录）
CREATE NONCLUSTERED INDEX IX_AuditLogs_UserId_Time
    ON dbo.AuditLogs (UserId, CreatedAt DESC);
GO

-- 按操作类型查询（审计特定操作）
CREATE NONCLUSTERED INDEX IX_AuditLogs_Action_Time
    ON dbo.AuditLogs (Action, CreatedAt DESC);
GO





