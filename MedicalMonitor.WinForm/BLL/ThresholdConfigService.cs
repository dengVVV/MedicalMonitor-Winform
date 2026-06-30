using MedicalMonitor.WinForm.Models;
using System.Collections.Generic;

namespace MedicalMonitor.WinForm.BLL
{
    /// <summary>
    /// 生理参数阈值配置服务 — 为第三阶段报警引擎预留。
    ///
    /// 本阶段提供阈值数据结构和默认值，
    /// 第三阶段 AlarmDetectionService 将依赖本服务获取阈值进行越限判断。
    /// </summary>
    public class ThresholdConfigService
    {
        /// <summary>单个参数的阈值范围</summary>
        public class ParameterThreshold
        {
            public double LowLimit { get; set; }
            public double HighLimit { get; set; }
            public string Unit { get; set; }
        }

        /// <summary>所有参数的默认阈值配置</summary>
        public Dictionary<string, ParameterThreshold> Defaults { get; }

        public ThresholdConfigService()
        {
            Defaults = new Dictionary<string, ParameterThreshold>
            {
                ["HR"] = new ParameterThreshold
                {
                    LowLimit = 50, HighLimit = 120, Unit = "bpm"
                },
                ["SpO2"] = new ParameterThreshold
                {
                    LowLimit = 90, HighLimit = 100, Unit = "%"
                },
                ["NIBP_Systolic"] = new ParameterThreshold
                {
                    LowLimit = 80, HighLimit = 180, Unit = "mmHg"
                },
                ["NIBP_Diastolic"] = new ParameterThreshold
                {
                    LowLimit = 50, HighLimit = 110, Unit = "mmHg"
                },
                ["RESP"] = new ParameterThreshold
                {
                    LowLimit = 8, HighLimit = 30, Unit = "次/分"
                },
                ["TEMP"] = new ParameterThreshold
                {
                    LowLimit = 35.0, HighLimit = 38.5, Unit = "℃"
                }
            };
        }

        /// <summary>获取指定参数的阈值</summary>
        public ParameterThreshold GetThreshold(string parameterType)
        {
            Defaults.TryGetValue(parameterType, out var threshold);
            return threshold;
        }
    }
}
