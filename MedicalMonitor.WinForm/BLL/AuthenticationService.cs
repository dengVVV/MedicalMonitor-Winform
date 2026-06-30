using MedicalMonitor.WinForm.DAL;
using System;

namespace MedicalMonitor.WinForm.BLL
{
    /// <summary>
    /// 用户认证服务 — 业务逻辑层（BLL）。
    ///
    /// 调用链：UI → BLL.AuthenticationService → DAL.UserRepository → SQL Server
    ///
    /// 本类只做业务判断（输入校验、结果封装），不直接操作数据库。
    /// </summary>
    public class AuthenticationService
    {
        private readonly UserRepository _userRepository;
        private readonly AuditLogService _auditLog;

        public enum LoginResultType
        {
            Success,
            InvalidCredentials,
            EmptyUsername,
            EmptyPassword
        }

        public class LoginResult
        {
            public LoginResultType ResultType { get; set; }
            public string Role { get; set; }
            public string FullName { get; set; }
            public string Message { get; set; }
            public bool IsSuccess => ResultType == LoginResultType.Success;
        }

        /// <summary>
        /// 通过 DI 注入 DAL 层的 UserRepository。
        /// </summary>
        public AuthenticationService(UserRepository userRepository, AuditLogService auditLog = null)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _auditLog = auditLog;
        }

        /// <summary>
        /// 登录认证：校验输入 → 查数据库 → 封装 LoginResult。
        /// </summary>
        public LoginResult Authenticate(string username, string password, string role)
        {
            if (string.IsNullOrWhiteSpace(username))
                return new LoginResult
                {
                    ResultType = LoginResultType.EmptyUsername,
                    Message = "请输入用户名或工号"
                };

            if (string.IsNullOrWhiteSpace(password))
                return new LoginResult
                {
                    ResultType = LoginResultType.EmptyPassword,
                    Message = "请输入密码"
                };

            // 调用 DAL 层查询用户
            var user = _userRepository.GetUserByCredentials(username.Trim(), password, role);

            if (user == null)
                return new LoginResult
                {
                    ResultType = LoginResultType.InvalidCredentials,
                    Message = "用户名、密码或登录身份不正确，请重试"
                };

            // 更新最后登录时间（不阻塞主流程，异常不影响登录）
            try
            {
                _userRepository.UpdateLastLogin(user.UserId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Auth] 更新登录时间失败: {ex.Message}");
            }

            // 审计日志（异步不阻塞）
            _auditLog?.Log(0, "Login", "Users", username, $"用户登录成功: {role}", null, null);

            return new LoginResult
            {
                ResultType = LoginResultType.Success,
                Role = user.Role,
                FullName = user.FullName,
                Message = $"欢迎使用医疗设备监控系统！\n\n当前身份：{user.Role}（{user.FullName}）"
            };
        }

        public string GetForgotPasswordMessage()
        {
            return "请联系系统管理员重置密码。\n\n管理员联系方式： ext. 8001";
        }
    }
}
