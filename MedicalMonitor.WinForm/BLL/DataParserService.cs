using MedicalMonitor.WinForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalMonitor.WinForm.BLL
{
    /// <summary>
    /// 设备数据帧解析引擎。
    ///
    /// 业务背景：
    ///   设备通过串口发送的原始数据是文本格式（或二进制格式），
    ///   需要按照预定义的协议规则解析成结构化的 VitalSignsModel 对象，
    ///   才能被后续的报警引擎、界面显示、数据存储等模块消费。
    ///
    /// 支持的协议格式（第一阶段）：
    ///   文本协议：
    ///     HR:75,SpO2:98,NIBP:120/80,RESP:16,TEMP:36.8,TS:2026-06-07 10:30:00\n
    ///     - 键值对以冒号分隔
    ///     - 不同参数以逗号分隔
    ///     - 以换行符作为帧结束标志
    ///
    /// 解析策略：
    ///   1. 健壮性优先：缺失的参数填 null，不因为一个字段异常而丢弃整帧
    ///   2. 合理性校验：解析后检查数值是否在临床合理范围内
    ///   3. 日志记录：解析失败时记录原始帧和错误原因，便于事后审计
    /// </summary>
    public class DataParserService
    {

        // ========== 临床合理范围（用于超出范围时标记 Invalid） ==========
        //
        // 这些范围比报警阈值更宽，表示"这个值在生理上是不可能的"。
        // 例如 HR=500 显然是数据错误而非真实的心率。

        private const double HR_MIN = 0;        // bpm，0 以下不可能
        private const double HR_MAX = 350;      // bpm，350 以上在成人中极不可能

        private const double SpO2_MIN = 0;      // %
        private const double SpO2_MAX = 100;    // %

        private const double NIBP_MIN = 0;      // mmHg
        private const double NIBP_MAX = 300;    // mmHg，300 以上极不可能

        private const double RESP_MIN = 0;      // 次/分
        private const double RESP_MAX = 60;     // 次/分，60 以上极不可能

        private const double TEMP_MIN = 30.0;   // ℃，30℃ 以下在活体中不可能
        private const double TEMP_MAX = 45.0;   // ℃，45℃ 以上在活体中不可能


        // ========== 公共方法 ==========

        /// <summary>
        /// 解析设备原始文本帧。
        ///
        /// 解析流程：
        ///   1. 空帧检查：如果输入为空或纯空白，返回 null
        ///   2. 键值对分割：按逗号拆分，每个片段按冒号提取键值对
        ///   3. 逐字段解析：按参数名分发到对应的赋值逻辑
        ///   4. 范围校验：每个解析后的值检查是否在生理合理范围内
        ///   5. 封装结果：构建 VitalSignsModel 并返回
        ///
        /// 错误处理：
        ///   单个字段解析失败不影响其他字段——缺失的字段保持 null。
        ///   只有所有字段都无法解析时，才返回 null。
        /// </summary>
        /// <param name="rawFrame">原始数据帧文本，如 "HR:75,SpO2:98,NIBP:120/80,RESP:16,TEMP:36.8"</param>
        /// <param name="deviceId">来源设备ID，用于追踪数据来源</param>
        /// <returns>解析成功返回 VitalSignsModel，完全失败返回 null</returns>
        public VitalSignsModel Parse(string rawFrame, string deviceId = "UNKNOWN")
        {
            if (string.IsNullOrWhiteSpace(rawFrame)) return null;
            //键值对分割：
            //例如：  "HR:75,SpO2:98 ->["HR:75","SpO2:98"]
            var pairs = rawFrame.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var vitals = new VitalSignsModel
            {
                Timestamp = DateTime.Now,
                DeviceId = deviceId,
                IsValid = true
            };
            int parsedCount = 0;  // 成功解析的字段数，等于0表示完全失败
            foreach(var pair in pairs)
            {
                var kv = pair.Split(new[] { ':' }, 2);//最多分成两部分，用于处理值中可能的冒号
                if (kv.Length != 2) continue;
                string key = kv[0].Trim().ToUpperInvariant();//统一转为大写
                string value = kv[1].Trim();
                switch (key)
                {
                    case "HR":
                        if (TryParseDouble(value, out double hr))
                        {
                            vitals.HeartRate = hr;
                            parsedCount++;
                        }
                        break;

                    case "SPO2":
                        if (TryParseDouble(value, out double spo2))
                        {
                            vitals.SpO2 = spo2;
                            parsedCount++;
                        }
                        break;

                    case "NIBP":
                        // NIBP 格式特殊: "120/80" → 收缩压/舒张压
                        var nibpParts = value.Split('/');
                        if (nibpParts.Length == 2)
                        {
                            if (TryParseDouble(nibpParts[0], out double sys))
                            {
                                vitals.NIBP_Systolic = sys;
                                parsedCount++;
                            }
                            if (TryParseDouble(nibpParts[1], out double dia))
                            {
                                vitals.NIBP_Diastolic = dia;
                                parsedCount++;
                            }
                        }
                        break;

                    case "RESP":
                        if (TryParseDouble(value, out double resp))
                        {
                            vitals.RespiratoryRate = resp;
                            parsedCount++;
                        }
                        break;

                    case "TEMP":
                        if (TryParseDouble(value, out double temp))
                        {
                            vitals.Temperature = temp;
                            parsedCount++;
                        }
                        break;

                    case "TS":
                        // TS 是设备时间戳，不参与生理参数解析
                        // 未来可用于分析数据传输延迟或数据时间对齐
                        break;

                    default:
                        // 未知参数名，记录日志后忽略（允许协议扩展）
                        Console.WriteLine($"[DataParser] 未知参数: {key}");
                        break;
                }

            }
            // ---- 4. 范围校验 ----
            if (!ValidateValueRange(vitals.HeartRate, HR_MIN, HR_MAX))
            {
                vitals.IsValid = false;
                Console.WriteLine($"[DataParser] HR 超出合理范围: {vitals.HeartRate}");
            }
            if (!ValidateValueRange(vitals.SpO2, SpO2_MIN, SpO2_MAX))
            {
                vitals.IsValid = false;
                Console.WriteLine($"[DataParser] SpO2 超出合理范围: {vitals.SpO2}");
            }
            if (!ValidateValueRange(vitals.NIBP_Systolic, NIBP_MIN, NIBP_MAX) ||
                !ValidateValueRange(vitals.NIBP_Diastolic, NIBP_MIN, NIBP_MAX))
            {
                vitals.IsValid = false;
                Console.WriteLine($"[DataParser] NIBP 超出合理范围");
            }
            if (!ValidateValueRange(vitals.RespiratoryRate, RESP_MIN, RESP_MAX))
            {
                vitals.IsValid = false;
                Console.WriteLine($"[DataParser] RESP 超出合理范围: {vitals.RespiratoryRate}");
            }
            if (!ValidateValueRange(vitals.Temperature, TEMP_MIN, TEMP_MAX))
            {
                vitals.IsValid = false;
                Console.WriteLine($"[DataParser] TEMP 超出合理范围: {vitals.Temperature}");
            }

            // ---- 5. 封装结果 ----
            if (parsedCount == 0)
            {
                // 没有任何字段解析成功，视为完全失败的帧
                return null;
            }

            return vitals;
        }


        // ========== 私有辅助方法 ==========

        /// <summary>
        /// 安全地尝试将字符串解析为 double。
        /// 避免 NumberFormatException 和 null 输入。
        /// </summary>
        /// <param name="text">要解析的字符串</param>
        /// <param name="result">解析结果（out 参数）</param>
        /// <returns>true 表示解析成功</returns>
        private bool TryParseDouble(string text, out double result)
        {
            // InvariantCulture 确保小数点用 '.' 而非中文环境的 '。'
            return double.TryParse(
                text,
                System.Globalization.NumberStyles.Any,  //允许任何数字格式,比如允许：123，123.45，-123，123（带空格），1e3（科学计数法）
                System.Globalization.CultureInfo.InvariantCulture,
                out result
            );
        }

        /// <summary>
        /// 校验值是否在合理范围内。
        /// null 值视为通过（无数据不报异常）。
        /// </summary>
        /// <param name="value">待校验的值（可空）</param>
        /// <param name="min">合理范围下限（含）</param>
        /// <param name="max">合理范围上限（含）</param>
        /// <returns>true 表示在合理范围内</returns>
        private bool ValidateValueRange(double? value, double min, double max)
        {
            if (!value.HasValue)
                return true;       // null = 无数据，不视为异常

            return value.Value >= min && value.Value <= max;
        }





    }
}
