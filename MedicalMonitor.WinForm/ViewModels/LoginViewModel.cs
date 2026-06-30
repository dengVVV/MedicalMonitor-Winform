using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace MedicalMonitor.WinForm.ViewModels
{
    /// <summary>
    /// 登录页面 ViewModel。
    ///
    /// MVVM 职责边界：
    ///   View (MainForm)   — 只做数据绑定和命令触发，不写任何业务判断
    ///   ViewModel (本类)   — 持有输入状态、执行登录逻辑、暴露命令
    ///
    /// 使用 CommunityToolkit.Mvvm：
    ///   ObservableObject — SetProperty() 驱动 PropertyChanged 通知 View
    ///   RelayCommand     — 把按钮点击和键盘回车映射到相同的业务逻辑
    ///
    /// 注意：.NET Framework 4.8 旧 csproj 格式下，源码生成器（[ObservableProperty]）
    ///   不可靠，此处全部使用显式的属性 + SetProperty() 写法，保证编译通过。
    /// </summary>
    public class LoginViewModel : ObservableObject
    {
        // ================================================================
        // 测试账号（第五阶段：Service 层 + 数据库 + BCrypt 替代）
        // ================================================================
        private static class DefaultCredentials
        {
            public const string NurseUser = "nurse01";
            public const string NursePass = "123456";

            public const string DoctorUser = "doctor01";
            public const string DoctorPass = "123456";

            public const string AdminUser = "admin";
            public const string AdminPass = "admin123";
        }

        // ================================================================
        // 字段 — 由属性包装，SetProperty 负责通知 View
        // ================================================================
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _selectedRole = "护士";
        private bool _isLoggingIn;
        private string _errorMessage = string.Empty;

        // ================================================================
        // 属性
        // ================================================================

        /// <summary>用户名 / 工号，双向绑定到 txtUsername.Text</summary>
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value ?? string.Empty);
        }

        /// <summary>密码，双向绑定到 txtPassword.Text</summary>
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value ?? string.Empty);
        }

        /// <summary>选中的角色文本（"护士"/"医生"/"管理员"），绑定到 cmbRole.SelectedItem</summary>
        public string SelectedRole
        {
            get => _selectedRole;
            set => SetProperty(ref _selectedRole, value ?? "护士");
        }

        /// <summary>是否正在执行登录操作（防止重复点击），绑定到 btnLogin.Enabled 的反逻辑</summary>
        public bool IsLoggingIn
        {
            get => _isLoggingIn;
            set
            {
                if (SetProperty(ref _isLoggingIn, value))
                {
                    // 登录进行中时禁止修改输入
                    LoginCommand.NotifyCanExecuteChanged();
                }
            }
        }

        /// <summary>错误提示信息，供 View 显示</summary>
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value ?? string.Empty);
        }

        // ================================================================
        // 命令
        // ================================================================

        /// <summary>登录命令：验证输入 → 认证 → 驱动 View 跳转</summary>
        public IRelayCommand LoginCommand { get; }

        /// <summary>忘记密码命令：弹出提示</summary>
        public IRelayCommand ForgotPasswordCommand { get; }

        // ================================================================
        // 事件 — View 订阅此事件以获取登录成功信号并切换页面
        // ================================================================

        /// <summary>登录成功时触发。View 层收到后调用 this.Close() 或导航到监控页。</summary>
        public event EventHandler<LoginSuccessEventArgs> LoginSucceeded;

        // ================================================================
        // 构造函数
        // ================================================================
        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(ExecuteLogin, CanExecuteLogin);
            ForgotPasswordCommand = new RelayCommand(ExecuteForgotPassword);
        }

        // ================================================================
        // 登录命令 — CanExecute
        // ================================================================
        private bool CanExecuteLogin()
        {
            // 用户名和密码不能为空，且当前不在登录中
            return !string.IsNullOrWhiteSpace(Username)
                && !string.IsNullOrWhiteSpace(Password)
                && !IsLoggingIn;
        }

        // ================================================================
        // 登录命令 — Execute
        // ================================================================
        private void ExecuteLogin()
        {
            IsLoggingIn = true;
            ErrorMessage = string.Empty;

            try
            {
                string user = Username.Trim();
                string pass = Password;
                string role = SelectedRole;

                // 输入二次校验（Command 内部的 CanExecute 已经拦截空值，防御性编程）
                if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
                {
                    ErrorMessage = "请输入用户名和密码";
                    return;
                }

                // 凭证验证
                if (!Authenticate(user, pass, role))
                {
                    ErrorMessage = "用户名、密码或登录身份不正确，请重试";

                    // 清空密码（View 层通过 PropertyChanged 自动同步）
                    Password = string.Empty;
                    return;
                }

                // 登录成功 → 清空错误信息 → 通知 View 跳转
                ErrorMessage = string.Empty;

                LoginSucceeded?.Invoke(this, new LoginSuccessEventArgs
                {
                    Username = user,
                    Role = role,
                    LoginTime = DateTime.Now
                });
            }
            finally
            {
                IsLoggingIn = false;
            }
        }

        // ================================================================
        // 忘记密码命令 — Execute
        // ================================================================
        private void ExecuteForgotPassword()
        {
            ErrorMessage = "请联系系统管理员重置密码。\n管理员联系方式： ext. 8001";
        }

        // ================================================================
        // 凭证认证（第五阶段：注入 IAuthService，改为 BCrypt + 数据库查询）
        // ================================================================
        private bool Authenticate(string username, string password, string role)
        {
            switch (role)
            {
                case "护士":
                    return username == DefaultCredentials.NurseUser
                        && password == DefaultCredentials.NursePass;
                case "医生":
                    return username == DefaultCredentials.DoctorUser
                        && password == DefaultCredentials.DoctorPass;
                case "管理员":
                    return username == DefaultCredentials.AdminUser
                        && password == DefaultCredentials.AdminPass;
                default:
                    return false;
            }
        }
    }

    /// <summary>
    /// 登录成功事件参数。
    /// View 层（MainForm）订阅此事件以获取认证结果并执行页面跳转。
    /// </summary>
    public class LoginSuccessEventArgs : EventArgs
    {
        public string Username { get; set; }
        public string Role { get; set; }
        public DateTime LoginTime { get; set; }
    }
}
