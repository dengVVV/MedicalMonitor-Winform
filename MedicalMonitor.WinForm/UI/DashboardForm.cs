using MedicalMonitor.WinForm.BLL;
using MedicalMonitor.WinForm.BLL.Interface;
using MedicalMonitor.WinForm.Controls;
using MedicalMonitor.WinForm.DAL;
using MedicalMonitor.WinForm.Models;
using Microsoft.Extensions.DependencyInjection;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Timers;
using Timer = System.Timers.Timer;

namespace MedicalMonitor.WinForm.UI
{
    public partial class DashboardForm : UIForm
    {
        // ================================================================
        // DI 服务
        // ================================================================
        private readonly MonitoringService _monitoringService;
        private readonly PatientBindingService _patientBinding;
        private readonly ExportService _exportService;
        private readonly VitalSignsQueryRepository _vitalSignsRepo;
        private readonly ThresholdConfigService _thresholdConfig;
        private readonly BackgroundStatisticsService _statsService;
        private readonly AlarmDetectionService _alarmService;

        // ================================================================
        // 运行时状态
        // ================================================================
        private string _currentOperatorName;
        private string _currentOperatorRole;
        private string _selectedBedNo;
        private bool _isDetailView;
        private static bool IsDesignTime =>
            LicenseManager.UsageMode == LicenseUsageMode.Designtime;

        // ================================================================
        // 顶层控件（Designer.cs 创建）
        // ================================================================
        private UIPanel _topToolbar;
        private UIPanel _gridViewPanel;
        private UIPanel _detailViewPanel;
        private UIPanel _statsBar;

        // ================================================================
        // 工具栏控件
        // ================================================================
        private UIButton _btnSave, _btnSaveAs, _btnNew, _btnDelete, _btnSettings, _btnLogout;
        private UILabel _lblTitle;
        private UIContextMenuStrip _saveAsMenu;

        // ================================================================
        // 侧边栏
        // ================================================================
        private SidebarControl _sidebar;

        // ================================================================
        // 床位卡片（最多一页 20 张）
        // ================================================================
        private readonly List<BedCardControl> _bedCards = new List<BedCardControl>();
        private List<BedMappingModel> _allBindings;
        private int _currentPage = 1;
        private int _totalPages = 1;
        private const int PageSize = 20;
        private UIFlowLayoutPanel _cardGridPanel;

        // ================================================================
        // 详情视图控件
        // ================================================================
        private UILabel _lblDetailHeader;
        private WaveformViewControl _waveformHR;
        private WaveformViewControl _waveformSpO2;
        private VitalSignPanelControl _vitalPanel;
        private UISymbolButton _btnBackToGrid;


        // ================================================================
        // 底部状态栏
        // ================================================================
        private UILabel _lblStats;
        private Timer _statsRefreshTimer;

        private readonly MedicalMonitorDbContext _db;

        // ================================================================
        // 无参构造 — VS 设计器
        // ================================================================
        public DashboardForm()
        {
            InitializeComponent();
        }

        // ================================================================
        // DI 构造 — 运行时入口
        // ================================================================
        public DashboardForm(
            MonitoringService monitoringService,
            PatientBindingService patientBinding,
            ExportService exportService,
            VitalSignsQueryRepository vitalSignsRepo,
            ThresholdConfigService thresholdConfig,
            BackgroundStatisticsService statsService,
            AlarmDetectionService alarmService,
            MedicalMonitorDbContext db) : this()
        {
            _monitoringService = monitoringService;
            _patientBinding = patientBinding;
            _exportService = exportService;
            _vitalSignsRepo = vitalSignsRepo;
            _thresholdConfig = thresholdConfig;
            _statsService = statsService;
            _alarmService = alarmService;
            _db = db;

            WireEvents();
        }

        // ================================================================
        // 公开方法
        // ================================================================

        public void SetOperator(string fullName, string role)
        {
            _currentOperatorName = fullName;
            _currentOperatorRole = role;
            _sidebar?.SetOperator(fullName, role);
        }

        // ================================================================
        // 界面构建（Designer 中 InitializeComponent 末尾自动调用）
        // ================================================================

        private void BuildLayout()
        {
            int cardW = 240, cardH = 140, margin = 10;
            int footerH = 50;
            int panelW = _gridViewPanel.Width;

            // ---- 顶部工具栏 ----
            _lblTitle = new UILabel
            {
                Text = "医疗设备监控系统 — 多床位总览",
                Location = new Point((this.Width - 320) / 2 - 50, 4),
                Size = new Size(300, 36),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Microsoft YaHei", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 50, 50),
                BackColor = Color.Transparent,
                AutoSize = false
            };
            _topToolbar.Controls.Add(_lblTitle);

            _btnNew = new UIButton { Text = "新建", Location = new Point(860, 8), Size = new Size(60, 32), Style = UIStyle.Blue };
            _btnDelete = new UIButton { Text = "解绑", Location = new Point(926, 8), Size = new Size(60, 32), Style = UIStyle.Blue };
            _btnSave = new UIButton { Text = "导出", Location = new Point(992, 8), Size = new Size(60, 32), Style = UIStyle.Blue };
            _btnSettings = new UIButton { Text = "设置", Location = new Point(1058, 8), Size = new Size(60, 32), Style = UIStyle.Blue };
            _btnLogout = new UIButton { Text = "退出", Location = new Point(1124, 8), Size = new Size(60, 32), Style = UIStyle.Blue };
            _topToolbar.Controls.AddRange(new Control[] { _btnNew, _btnDelete, _btnSave, _btnSettings, _btnLogout });

            // ---- 侧边栏 ----
            _sidebar = new SidebarControl
            {
                Location = new Point(0, 48),
                Size = new Size(240, this.ClientSize.Height - 84),
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom
            };
            this.Controls.Add(_sidebar);

            // ---- 床位卡片网格 ----
            _cardGridPanel = new UIFlowLayoutPanel
            {
                Location = new Point(15, 10),
                Size = new Size(panelW - 30, _gridViewPanel.Height - footerH - 30),
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom,
                FillColor = Color.Transparent,
                RectColor = Color.Transparent,
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true
            };

            for (int i = 0; i < PageSize; i++)
            {
                var card = new BedCardControl
                {
                    Size = new Size(cardW, cardH),
                    Margin = new Padding(margin),
                    Visible = false
                };
                card.BedSelected += OnBedCardSelected;
                _bedCards.Add(card);
                _cardGridPanel.Controls.Add(card);
            }

            // 底部翻页栏
            UIPanel footer = new UIPanel
            {
                Location = new Point(15, _gridViewPanel.Height - footerH - 10),
                Size = new Size(panelW, footerH),
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right,
                FillColor = Color.Transparent,
                RectColor = Color.Transparent
            };

            var btnPrev = new UISymbolButton
            {
                Text = "上一页",
                Symbol = 61473,
                Location = new Point(0, 0),
                Size = new Size(80, 36)
            };
            btnPrev.Click += (s, e) => { if (_currentPage > 1) ShowPage(_currentPage - 1); };

            var lblPage = new UILabel
            {
                Text = "第 1/1 页",
                Location = new Point(panelW / 2 - 60, 6),
                Size = new Size(120, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Microsoft YaHei", 10),
                ForeColor = Color.FromArgb(100, 100, 100),
                BackColor = Color.Transparent,
                Name = "lblPageIndicator",
                AutoSize = false
            };

            var btnNext = new UISymbolButton
            {
                Text = "下一页",
                Symbol = 61474,
                Location = new Point(panelW - 110, 0),
                Size = new Size(80, 36)
            };
            btnNext.Click += (s, e) => { if (_currentPage < _totalPages) ShowPage(_currentPage + 1); };

            footer.Controls.AddRange(new Control[] { btnPrev, lblPage, btnNext });
            _gridViewPanel.Controls.Add(_cardGridPanel);
            _gridViewPanel.Controls.Add(footer);

            // ---- 详情视图 ----
            _lblDetailHeader = new UILabel
            {
                Location = new Point(15, 10),
                Size = new Size(panelW - 30, 30),
                Text = "患者详情",
                Font = new Font("Microsoft YaHei", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 50, 50),
                BackColor = Color.Transparent,
                AutoSize = false
            };

            _btnBackToGrid = new UISymbolButton
            {
                Text = "返回", Symbol = 61473,
                Location = new Point(panelW - 110, 10), Size = new Size(80, 32),
                Style = UIStyle.Blue
            };

            _waveformHR = new WaveformViewControl
            {
                Location = new Point(15, 50), Size = new Size(panelW / 2 - 30, 200),
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right
            };

            _waveformSpO2 = new WaveformViewControl
            {
                Location = new Point(panelW / 2 + 5, 50), Size = new Size(panelW / 2 - 30, 200),
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right
            };

            _vitalPanel = new VitalSignPanelControl
            {
                Location = new Point(15, 260), Size = new Size(panelW - 30, 200),
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right
            };

            _detailViewPanel.Controls.AddRange(new Control[] { _lblDetailHeader, _btnBackToGrid, _waveformHR, _waveformSpO2, _vitalPanel });

            // ---- 底部状态栏 ----
            _lblStats = new UILabel
            {
                Dock = DockStyle.Fill,
                Text = "📊 今日统计: 加载中...  |  在线设备: 0/" + (_monitoringService?.DataSourceCount ?? 0) + "  |  累计数据帧: 0",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Microsoft YaHei", 10),
                ForeColor = Color.FromArgb(100, 100, 100),
                BackColor = Color.Transparent,
                AutoSize = false
            };
            _statsBar.Controls.Add(_lblStats);
        }

        // ================================================================
        // 事件绑定
        // ================================================================

        private void WireEvents()
        {
            _btnSave.Click += (s, e) => ExportToExcel();
            _btnNew.Click += (s, e) => OnNewPatient();
            _btnDelete.Click += (s, e) => OnDeleteSession();
            _btnSettings.Click += (s, e) => OnOpenSettings();
            _btnLogout.Click += (s, e) => OnLogout();
            _btnBackToGrid.Click += (s, e) => ShowGridView();

            foreach (var card in _bedCards)
                card.BedSelected += OnBedCardSelected;

            _sidebar.BedClicked += OnSidebarBedClicked;
            _sidebar.NavClicked += OnSidebarNavClicked;
            _sidebar.LogoutClicked += (s, e) => OnLogout();

            _monitoringService.SnapshotUpdated += OnSnapshotUpdated;
            _monitoringService.ConnectionChanged += OnConnectionChanged;
            _alarmService.AlarmTriggered += OnAlarmTriggered;

            this.Resize += (s, e) =>
            {
                if (_lblTitle != null)
                    _lblTitle.Location = new Point((this.Width - 300) / 2 - 50, 4);
            };
            this.FormClosing += (s, e) =>
            {
                _statsRefreshTimer?.Stop();
                _statsService?.Stop();
                _monitoringService?.StopAll();
            };

            _statsRefreshTimer = new Timer(300000);
            _statsRefreshTimer.AutoReset = true;
            _statsRefreshTimer.Elapsed += (s, e) => RefreshStatsBar();
            _statsRefreshTimer.Start();
        }

        // ================================================================
        // 视图切换
        // ================================================================

        private void ShowGridView()
        {
            _isDetailView = false;
            _gridViewPanel.Visible = true;
            _detailViewPanel.Visible = false;
            _selectedBedNo = null;
        }

        private void ShowDetailView(string bedNo)
        {
            _isDetailView = true;
            _selectedBedNo = bedNo;
            _gridViewPanel.Visible = false;
            _detailViewPanel.Visible = true;

            var binding = _allBindings?.FirstOrDefault(b => b.BedNo == bedNo);
            if (binding != null)
            {
                _lblDetailHeader.Text = $"{bedNo} — {(binding.Patient?.Name ?? "空床")}";
                var deviceId = binding.DeviceId;

                // 用数据库历史记录初始化波形图
                try
                {
                    var history = _vitalSignsRepo?.GetRecentRecords(deviceId, 60);
                    if (history != null)
                    {
                        var reversed = history.Reverse().ToList();
                        _waveformHR.InitializeWithHistory(reversed.Select(r => ((r.RecordTime - DateTime.Today).TotalSeconds, (double?)r.HeartRate, (double?)null)));
                        _waveformSpO2.InitializeWithHistory(reversed.Select(r => ((r.RecordTime - DateTime.Today).TotalSeconds, (double?)null, (double?)r.SpO2)));
                    }
                }
                catch { /* DB未就绪时忽略，波形图从零开始 */ }

                var snapshot = _monitoringService.GetSnapshot(deviceId);
                if (snapshot != null)
                {
                    _vitalPanel.UpdateVitalSigns(snapshot);
                }
            }
        }

        // ================================================================
        // 监控数据事件
        // ================================================================

        private void OnSnapshotUpdated(object sender, VitalSignsModel vitals)
        {
            if (IsDesignTime) return;
            if (this.InvokeRequired) { this.BeginInvoke(new Action(() => OnSnapshotUpdated(sender, vitals))); return; }
            this.SuspendLayout();

            foreach (var card in _bedCards)
            {
                if (card.Binding == null) continue;
                bool matched = card.Binding.DeviceId == vitals.DeviceId;
                if (!matched && !string.IsNullOrEmpty(card.Binding.BedNo) && card.Binding.BedNo.Length >= 6)
                {
                    string derivedId = card.Binding.DeviceId;
                    matched = derivedId == vitals.DeviceId;
                }
                if (matched)
                {
                    card.Binding.LatestVitalSigns = vitals;
                    card.RefreshDisplay();
                    break;
                }
            }

            if (_isDetailView && _selectedBedNo != null)
            {
                // 通过 _allBindings 查找床位号对应的 DeviceId（BedNo ≠ DeviceId，不能直接比较）
                var binding = _allBindings?.FirstOrDefault(b => b.BedNo == _selectedBedNo);
                if (binding != null && binding.DeviceId == vitals.DeviceId)
                {
                    _vitalPanel.UpdateVitalSigns(vitals);
                    _waveformHR.AddDataPoint(vitals.HeartRate, null);
                    _waveformSpO2.AddDataPoint(null, vitals.SpO2);
                }
            }
            this.ResumeLayout(false);

        }

        private void OnConnectionChanged(object sender, (string DeviceId, bool IsOnline) args)
        {
            if (IsDesignTime) return;
            if (this.InvokeRequired) { this.BeginInvoke(new Action(() => OnConnectionChanged(sender, args))); return; }

            var bedNo = _patientBinding.GetBedNoByDeviceId(args.DeviceId);
            if (bedNo != null)
            {
                foreach (var card in _bedCards)
                {
                    if (card.Binding != null && card.Binding.BedNo == bedNo)
                    {
                        card.Binding.IsOnline = args.IsOnline;
                        card.RefreshDisplay();
                        break;
                    }
                }
                _sidebar.UpdateBedItem(bedNo, _patientBinding.GetPatientByBedNo(bedNo)?.Name, args.IsOnline);
            }
        }

        // ================================================================
        // 侧边栏事件
        // ================================================================

        private void OnSidebarBedClicked(object sender, string bedNo)
        {
            _selectedBedNo = bedNo;
            ShowDetailView(bedNo);
        }

        private void OnSidebarNavClicked(object sender, string navItem)
        {
            switch (navItem)
            {
                case "监护总览": ShowGridView(); break;
                case "报警管理": UIMessageBox.ShowInfo("报警管理模块将在后续版本中实现"); break;
                case "患者管理": ShowGridView(); break;
                case "数据导出": ExportToExcel(); break;
            }
        }

        private void OnBedCardSelected(object sender, BedMappingModel binding)
        {
            if (binding != null)
            {
                _selectedBedNo = binding.BedNo;
                ShowDetailView(binding.BedNo);
            }
        }

        // ================================================================
        // 工具栏操作
        // ================================================================

        private void OnNewPatient()
        {
            if (string.IsNullOrEmpty(_selectedBedNo)) { UIMessageBox.ShowWarning("请先在左侧选择一个床位"); return; }
            if (IsDesignTime) return;
            var sp = (System.IServiceProvider)Program.ServiceProvider;
            var frm = sp.GetService<SetPatientsInfo>();
            frm?.ShowDialog(this);
            RefreshDashboard(_currentPage);
        }

        private void OnDeleteSession()
        {
            if (string.IsNullOrEmpty(_selectedBedNo)) { UIMessageBox.ShowWarning("请先在左侧选择一个床位"); return; }
            if (UIMessageBox.ShowAsk($"确定要解除 {_selectedBedNo} 的患者绑定吗？"))
            {
                _patientBinding?.UnbindPatient(_selectedBedNo);
                _sidebar?.UpdateBedItem(_selectedBedNo, null, true);
                foreach (var card in _bedCards)
                    if (card.Binding != null && card.Binding.BedNo == _selectedBedNo) { card.Binding.Patient = null; card.RefreshDisplay(); break; }
                ShowGridView();
                UIMessageBox.ShowSuccess($"{_selectedBedNo} 已解绑");
            }
        }

        private void ExportToExcel()
        {
            if (string.IsNullOrEmpty(_selectedBedNo)) { UIMessageBox.ShowWarning("请先在左侧选择一个床位"); return; }
            var deviceId = _patientBinding.GetDeviceIdByBedNo(_selectedBedNo);
            var outputPath = ExportService.GenerateDefaultPath(deviceId);
            var result = _exportService?.ExportToExcel(deviceId, outputPath);
            if (result != null) UIMessageBox.ShowSuccess($"导出成功！\n\n{result}");
            else UIMessageBox.ShowError("导出失败，请检查日志");
        }

        private void OnOpenSettings()
        {
            if (IsDesignTime) return;
            var sp = (System.IServiceProvider)Program.ServiceProvider;
            var frm = sp.GetService<SettingsForm>();
            if (frm != null) frm.ShowDialog(this);
        }

        private void OnLogout()
        {
            if (IsDesignTime) return;
            if (UIMessageBox.ShowAsk("确定要退出登录吗？"))
            {
                _monitoringService?.StopAll();
                _statsService?.Stop();
                _statsRefreshTimer?.Stop();
                var sp = (System.IServiceProvider)Program.ServiceProvider;
                sp.GetRequiredService<MainForm>().Show();
                this.Close();
            }
        }

        // ================================================================
        // 窗体加载
        // ================================================================

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (IsDesignTime) return;

            RefreshDashboard(1);

            // 根据数据库患者动态创建设备模拟器
            CreateAndRegisterSimulators();

            _monitoringService.StartAll();
            _statsService.Start();
        }

        /// <summary>
        /// 刷新仪表盘数据
        /// </summary>
        private void RefreshDashboard(int showPageIdx)
        {
            _patientBinding.InitializeDefaultBindings();
            _allBindings = _patientBinding.GetAllBindings()
                .Where(b => b.Patient != null)
                .OrderBy(b => b.BedNo)
                .ToList();
            _totalPages = Math.Max(1, (int)Math.Ceiling((double)_allBindings.Count / PageSize));
            ShowPage(showPageIdx);

            foreach (var b in _allBindings)
                _sidebar.UpdateBedItem(b.BedNo, b.Patient?.Name, b.IsOnline);
            RefreshStatsBar();
        }

        private void ShowPage(int page)
        {
            if (_allBindings == null || page < 1 || page > _totalPages) return;
            _currentPage = page;

            int start = (page - 1) * PageSize;
            for (int i = 0; i < PageSize; i++)
            {
                int idx = start + i;
                if (idx < _allBindings.Count)
                {
                    _bedCards[i].Binding = _allBindings[idx];
                    _bedCards[i].Visible = true;
                }
                else
                {
                    _bedCards[i].Binding = null;
                    _bedCards[i].Visible = false;
                }
            }

            foreach (Control c in _gridViewPanel.Controls)
            {
                if (c is UIPanel footer)
                {
                    foreach (Control fc in footer.Controls)
                    {
                        if (fc is UILabel lbl && lbl.Name == "lblPageIndicator")
                        {
                            lbl.Text = $"第 {page}/{_totalPages} 页";
                            break;
                        }
                    }
                }
            }
        }

        // ================================================================
        // 动态创建设备模拟器
        // ================================================================

        /// <summary>
        /// 根据数据库中患者数量动态创建设备模拟器。
        /// 每个患者的 BoundDeviceId 对应一个模拟器实例，
        /// 基线参数根据患者年龄自动计算。
        /// 若数据库无患者则创建 8 台默认模拟器（开发演示用）。
        /// </summary>
        private void CreateAndRegisterSimulators()
        {
            var deviceIds = _patientBinding.GetDeviceIds();
            if (deviceIds.Count == 0)
            {
                var defaults = new (string id, double hr, double spo2, double sys, double dia, double resp, double temp)[]
                {
                    ("SIM-001", 72, 98, 118, 78, 14, 36.6),
                    ("SIM-002", 88, 96, 145, 92, 18, 37.0),
                    ("SIM-003", 65, 99, 125, 82, 12, 36.3),
                    ("SIM-004", 95, 94, 135, 88, 20, 37.5),
                    ("SIM-005", 78, 97, 128, 84, 16, 36.8),
                    ("SIM-006", 60, 98, 110, 70, 15, 36.4),
                    ("SIM-007", 82, 95, 140, 90, 19, 37.2),
                    ("SIM-008", 75, 97, 122, 80, 13, 36.9),
                };
                foreach (var (id, hr, spo2, sys, dia, resp, temp) in defaults)
                {
                    var sim = new DeviceSimulatorService(id, hr, spo2, sys, dia, resp, temp);
                    _monitoringService.AddDataSource(sim);
                }
            }
            else
            {
                foreach (var deviceId in deviceIds)
                {
                    var patient = _patientBinding.GetPatientByDeviceId(deviceId);
                    var (hr, spo2, sys, dia, resp, temp) = ComputeBaselineFromPatient(patient);
                    var sim = new DeviceSimulatorService(deviceId, hr, spo2, sys, dia, resp, temp);
                    _monitoringService.AddDataSource(sim);
                }
            }

            // 更新状态栏设备数
            RefreshStatsBar();
        }

        /// <summary>
        /// 根据患者年龄计算生理参数基线。
        /// 年龄越大 → HR越低、血压越高。
        /// </summary>
        private static (double hr, double spo2, double sys, double dia, double resp, double temp)
            ComputeBaselineFromPatient(PatientModel patient)
        {
            int age = patient?.Age ?? 40;
            double hr   = 80 - (age - 40) * 0.15;
            double spo2 = 97;
            double sys  = 115 + age * 0.3;
            double dia  = 70 + age * 0.15;
            double resp = 16;
            double temp = 36.8;

            hr  = hr  < 55 ? 55  : hr  > 100 ? 100 : hr;
            sys = sys < 100 ? 100 : sys > 170 ? 170 : sys;
            dia = dia < 65  ? 65  : dia > 100 ? 100 : dia;

            return (Math.Round(hr, 0), Math.Round(spo2, 0), Math.Round(sys, 0), Math.Round(dia, 0), Math.Round(resp, 0), Math.Round(temp, 1));
        }

        // ================================================================
        // 报警处理

        private void OnAlarmTriggered(object sender, AlarmEventModel alarm)
        {
            if (IsDesignTime) return;
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => OnAlarmTriggered(sender, alarm)));
                return;
            }

            // 弹出报警窗口
            var popup = new AlarmPopupForm(alarm, _alarmService, _currentOperatorName);
            popup.Show();

            // 更新侧边栏报警计数
            int unacknowledged = _alarmService.GetUnacknowledgedAlarms().Count;
            _sidebar?.UpdateAlarmCount(unacknowledged);
        }

        // ================================================================
        // 底部状态栏
        // ================================================================

        private void RefreshStatsBar()
        {
            if (_lblStats == null) return;
            try
            {
                var (avgHr, avgSpO2, totalRecords) = _vitalSignsRepo.GetTodayOverallStats();
                int onlineDevices = _bedCards.Count(c => c.Binding != null && c.Binding.IsOnline);
                int totalDevices = _monitoringService?.DataSourceCount ?? 0;
                _lblStats.Text = string.Format(
                    "📊 今日统计: 平均HR {0:F0} bpm | 平均SpO2 {1:F1}% | 在线设备: {2}/{3} | 累计数据帧: {4}",
                    avgHr ?? 0, avgSpO2 ?? 0, onlineDevices, totalDevices, totalRecords);
            }
            catch
            {
                int count = _monitoringService?.DataSourceCount ?? 0;
                _lblStats.Text = $"📊 今日统计: (数据库未连接)  |  在线设备: {count}/{count}  |  累计数据帧: --";
            }
        }
    }
}
