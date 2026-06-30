using MedicalMonitor.WinForm.BLL;
using MedicalMonitor.WinForm.BLL.Interface;
using MedicalMonitor.WinForm.DAL;
using MedicalMonitor.WinForm.UI;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows.Forms;

namespace MedicalMonitor.WinForm
{
    internal static class Program
    {
        public static ServiceProvider ServiceProvider { get; private set; }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            // 全局异常处理
            Application.ThreadException += (sender, e) =>
            {
                MessageBox.Show(
                    $"未处理的异常:\n{e.Exception.Message}\n\n详情请查看日志文件。",
                    "系统错误",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            };

            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                var ex = e.ExceptionObject as Exception;
                MessageBox.Show(
                    $"严重错误:\n{ex?.Message}\n\n程序将退出。",
                    "致命错误",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            };

            var mainForm = ServiceProvider.GetRequiredService<MainForm>();
            Application.Run(mainForm);
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            // ======== EF6 DbContext ========
            services.AddSingleton<MedicalMonitorDbContext>();

            // ======== Dapper 连接工厂 ========
            services.AddSingleton<DbConnectionFactory>();

            // ======== DAL — Dapper（复杂查询：聚合/联表/窗口函数）========
            services.AddSingleton<VitalSignsQueryRepository>();
            services.AddSingleton<StatisticsRepository>();
            services.AddSingleton<AlarmEventRepository>();
            services.AddSingleton<UserRepository>();

            // ======== DAL — EF6（简单 CRUD：单表增删改查）========
            services.AddSingleton<PatientRepository>();
            services.AddSingleton<DeviceRepository>();

            // ======== BLL 层 ========
            services.AddSingleton<AuthenticationService>();
            services.AddSingleton<DataParserService>();
            services.AddSingleton<ThresholdConfigService>();
            services.AddSingleton<PatientBindingService>();

            // MonitoringService 不再通过构造函数注入数据源，
            // 数据源在 DashboardForm.OnLoad 中根据数据库患者数动态创建后通过 AddDataSource 添加
            services.AddSingleton<MonitoringService>();
            services.AddSingleton<AlarmDetectionService>();

            services.AddSingleton<ExportService>();
            services.AddSingleton<BackgroundStatisticsService>();
            services.AddSingleton<AuditLogService>();

            // ======== UI 层 ========
            services.AddSingleton<MainForm>();
            services.AddTransient<DashboardForm>();
            services.AddTransient<SettingsForm>();
            services.AddTransient<AlarmListForm>();
            services.AddTransient<SetPatientsInfo>();
        }
    }
}
