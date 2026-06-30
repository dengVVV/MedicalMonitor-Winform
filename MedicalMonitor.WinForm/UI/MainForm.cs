using Microsoft.Extensions.DependencyInjection;
using Sunny.UI;
using MedicalMonitor.WinForm.BLL;
using System;
using System.Windows.Forms;

namespace MedicalMonitor.WinForm.UI
{
    /// <summary>
    /// 登录页面 — 表示层（UI Layer）。
    ///
    /// 三层架构中的角色：
    ///   本类只做三件事：
    ///     1. 收集用户在控件中输入的数据
    ///     2. 调用 BLL.AuthenticationService 执行业务逻辑
    ///     3. 根据返回的 LoginResult 跳转或显示消息
    ///
    /// 严禁在本类中编写任何凭证验证、角色判断、登录条件逻辑。
    /// </summary>
    public partial class MainForm : UIForm
    {
        private readonly AuthenticationService _authService;

        /// <summary>
        /// 通过 DI 注入 AuthenticationService（BLL 层）。
        /// </summary>
        public MainForm(AuthenticationService authService)
        {
            InitializeComponent();

            _authService = authService ?? throw new ArgumentNullException(nameof(authService));

            this.txtPassword.ImeMode = ImeMode.Disable;

            // 注册按钮 Click 事件
            this.btnLogin.Click += BtnLogin_Click;
            this.lnkForgotPwd.Click += LnkForgotPwd_Click;

            this.txtUsername.KeyDown += OnInputKeyDown;
            this.txtPassword.KeyDown += OnInputKeyDown;
            this.cmbRole.KeyDown += OnInputKeyDown;
        }

        // ================================================================
        // SunnyUI 兼容：OnLoad 初始化下拉
        // ================================================================
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.cmbRole.Items.Clear();
            this.cmbRole.Items.Add("护士");
            this.cmbRole.Items.Add("医生");
            this.cmbRole.Items.Add("管理员");
            this.cmbRole.SelectedIndex = 0;
        }

        // ================================================================
        // 登录按钮 → 收集输入 → 调用 BLL → 处理结果
        // ================================================================
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            DoLogin();
        }

        private void OnInputKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                DoLogin();
            }
        }

        private void LnkForgotPwd_Click(object sender, EventArgs e)
        {
            string msg = _authService.GetForgotPasswordMessage();
            UIMessageBox.ShowInfo(msg);
        }

        // ================================================================
        // 登录流程 — UI 层核心方法
        // ================================================================
        private void DoLogin()
        {
            this.btnLogin.Enabled = false;

            try
            {
                string username = (txtUsername?.Text ?? string.Empty).Trim();
                string password = txtPassword?.Text ?? string.Empty;
                string role = cmbRole?.SelectedItem?.ToString() ?? "护士";

                var result = _authService.Authenticate(username, password, role);

                switch (result.ResultType)
                {
                    case AuthenticationService.LoginResultType.Success:
                        HandleLoginSuccess(result);
                        break;

                    case AuthenticationService.LoginResultType.EmptyUsername:
                        UIMessageBox.ShowWarning(result.Message);
                        txtUsername?.Focus();
                        break;

                    case AuthenticationService.LoginResultType.EmptyPassword:
                        UIMessageBox.ShowWarning(result.Message);
                        txtPassword?.Focus();
                        break;

                    case AuthenticationService.LoginResultType.InvalidCredentials:
                        UIMessageBox.ShowError(result.Message);
                        txtPassword.Text = string.Empty;
                        txtPassword.Focus();
                        break;
                }
            }
            finally
            {
                this.btnLogin.Enabled = true;
            }
        }

        /// <summary>
        /// 登录成功 → 打开 DashboardForm 并隐藏登录窗。
        ///
        /// 设计决策：
        ///   隐藏而非关闭登录窗，是因为 DI 容器中 MainForm 注册为 Singleton。
        ///   关闭会导致容器引用失效。Hide() 保留实例，退出登录时可重新 Show()。
        /// </summary>
        private void HandleLoginSuccess(AuthenticationService.LoginResult result)
        {
            Console.WriteLine(
                $"[Login] 角色:{result.Role} | 时间:{DateTime.Now:yyyy-MM-dd HH:mm:ss}"
            );

            // 获取 DashboardForm 并传递操作员信息
            var dashboard = ((System.IServiceProvider)Program.ServiceProvider).GetRequiredService<DashboardForm>();
            dashboard.SetOperator(result.FullName, result.Role);

            // 导航到仪表盘
            dashboard.FormClosed += (s, e) =>
            {
                // DashboardForm 关闭时（非退出登录），也关闭登录窗 → 程序退出
                if (this.Visible == false)
                    this.Close();
            };

            dashboard.Show();
            this.Hide();
        }
    }
}
