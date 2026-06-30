using ClosedXML.Excel;
using MedicalMonitor.WinForm.DAL;
using MedicalMonitor.WinForm.Models;
using System;
using System.IO;
using System.Linq;

namespace MedicalMonitor.WinForm.BLL
{
    /// <summary>
    /// 数据导出服务 — 将生理参数/患者数据导出为 Excel 或医疗报告格式。
    ///
    /// 使用 ClosedXML（MIT 协议，无需安装 Excel）生成 .xlsx 文件。
    /// 导出内容包含：患者基本信息、24小时生理参数表、统计摘要。
    /// </summary>
    public class ExportService
    {
        private readonly VitalSignsQueryRepository _vitalSignsRepo;
        private readonly PatientBindingService _patientBinding;

        public ExportService(
            VitalSignsQueryRepository vitalSignsRepo,
            PatientBindingService patientBinding)
        {
            _vitalSignsRepo = vitalSignsRepo;
            _patientBinding = patientBinding;
        }

        /// <summary>
        /// 导出指定设备的 24 小时生理参数到 Excel。
        ///
        /// 输出列：记录时间 | 心率(bpm) | 血氧(%) | 收缩压(mmHg) | 舒张压(mmHg) | 呼吸(次/分) | 体温(℃)
        /// </summary>
        /// <param name="deviceId">设备ID</param>
        /// <param name="outputPath">输出文件路径（含 .xlsx 扩展名）</param>
        /// <returns>导出成功返回文件路径，失败返回 null</returns>
        public string ExportToExcel(string deviceId, string outputPath)
        {
            try
            {
                var records = _vitalSignsRepo.GetRecentRecords(deviceId,30)
                    .OrderBy(r => r.RecordTime)
                    .ToList();

                var patient = _patientBinding.GetPatientByDeviceId(deviceId);

                using (var workbook = new XLWorkbook())
                {
                    var sheet = workbook.Worksheets.Add("生理参数记录");

                    // ---- 患者信息 ----
                    if (patient != null)
                    {
                        sheet.Cell(1, 1).Value = "患者姓名:";
                        sheet.Cell(1, 2).Value = patient.Name;
                        sheet.Cell(2, 1).Value = "床号:";
                        sheet.Cell(2, 2).Value = patient.BedNo;
                        sheet.Cell(3, 1).Value = "年龄:";
                        sheet.Cell(3, 2).Value = patient.Age;
                        sheet.Cell(4, 1).Value = "诊断:";
                        sheet.Cell(4, 2).Value = patient.Diagnosis;
                    }

                    // ---- 表头 ----
                    int headerRow = patient != null ? 6 : 1;
                    sheet.Cell(headerRow, 1).Value = "记录时间";
                    sheet.Cell(headerRow, 2).Value = "心率(bpm)";
                    sheet.Cell(headerRow, 3).Value = "血氧(%)";
                    sheet.Cell(headerRow, 4).Value = "收缩压(mmHg)";
                    sheet.Cell(headerRow, 5).Value = "舒张压(mmHg)";
                    sheet.Cell(headerRow, 6).Value = "呼吸(次/分)";
                    sheet.Cell(headerRow, 7).Value = "体温(℃)";

                    // 表头加粗 + 背景色
                    //起始行、起始列，结束行、结束列
                    var headerRange = sheet.Range(headerRow, 1, headerRow, 7);
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;

                    // ---- 数据行 ----
                    int row = headerRow + 1;
                    foreach (var rec in records)
                    {
                        sheet.Cell(row, 1).Value = rec.RecordTime.ToString("yyyy-MM-dd HH:mm:ss");
                        sheet.Cell(row, 2).Value = rec.HeartRate;
                        sheet.Cell(row, 3).Value = rec.SpO2;
                        sheet.Cell(row, 4).Value = rec.NibpSystolic;
                        sheet.Cell(row, 5).Value = rec.NibpDiastolic;
                        sheet.Cell(row, 6).Value = rec.RespiratoryRate;
                        sheet.Cell(row, 7).Value = rec.Temperature;
                        row++;
                    }

                    // 自动列宽
                    sheet.Columns().AdjustToContents();

                    workbook.SaveAs(outputPath);
                }

                return outputPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Export] Excel导出失败: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 生成默认导出文件路径（桌面）。
        /// 文件名格式: {设备ID}_{日期时间}.xlsx
        /// </summary>
        public static string GenerateDefaultPath(string deviceId)
        {
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            return Path.Combine(desktop, $"{deviceId}_{timestamp}.xlsx");
        }
    }
}
