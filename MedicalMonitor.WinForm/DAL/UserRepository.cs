using Dapper;
using MedicalMonitor.WinForm.DAL.Entities;
using System.Linq;

namespace MedicalMonitor.WinForm.DAL
{
    /// <summary>
    /// 用户数据访问仓库 — DAL 层。
    ///
    /// 使用 Dapper（轻量级 ORM）执行 SQL 查询，返回实体对象。
    /// 本类只关心数据存取，不做业务判断（角色校验归 BLL）。
    /// </summary>
    public class UserRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        /// <summary>
        /// 通过 DI 注入连接工厂。不在此处打开连接，每个方法内部自行管理 using。
        /// </summary>
        public UserRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        /// <summary>
        /// 根据用户名、密码、角色查询用户。
        ///
        /// 查询策略：
        ///   用户名 → 唯一索引 IX_Users_Username → 单行返回
        ///   密码和角色在 SQL 中直接比较，减少无效数据传输
        ///
        /// 返回 null 表示用户名不存在、密码错误或角色不匹配。
        /// </summary>
        public UserEntity GetUserByCredentials(string username, string password, string role)
        {
            // 当前阶段明文比较，第五阶段改为 PasswordHash = @Hash
            const string sql = @"
SELECT UserId, Username, PasswordHash, Role, FullName, IsActive
FROM dbo.Users
WHERE Username   = @Username
  AND PasswordHash = @Password
  AND Role         = @Role
  AND IsActive     = 1;";

            using (var conn = _connectionFactory.CreateConnection())
            {
                return conn.Query<UserEntity>(sql, new
                {
                    Username = username,
                    Password = password,
                    Role = role
                }).FirstOrDefault();
            }
        }

        /// <summary>
        /// 更新用户最后登录时间。
        /// 登录成功后由 BLL 层调用。
        /// </summary>
        public void UpdateLastLogin(int userId)
        {
            const string sql = @"
UPDATE dbo.Users
SET LastLoginAt = SYSDATETIME()
WHERE UserId = @UserId;";

            using (var conn = _connectionFactory.CreateConnection())
            {
                conn.Execute(sql, new { UserId = userId });
            }
        }
    }
}
