using MedicalMonitor.WinForm.DAL;
using MedicalMonitor.WinForm.DAL.Entities;
using MedicalMonitor.WinForm.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MedicalMonitor.WinForm.BLL
{
    /// <summary>
    /// 患者-床位绑定服务。
    ///
    /// 业务规则：
    ///   一个床位只能绑定一个患者，一个患者只能绑定一个床位。
    ///   解绑后床位恢复空床状态。
    ///
    /// 数据来源：
    ///   从 SQL Server Patients 表加载患者信息，
    ///   通过 BoundDeviceId 字段建立设备→患者→床位 的映射关系。
    /// </summary>
    public class PatientBindingService
    {
        /// <summary>床位号 → 患者 的映射关系</summary>
        private readonly Dictionary<string, PatientModel> _bindings
            = new Dictionary<string, PatientModel>();

        /// <summary>设备ID → 床位号 的映射关系</summary>
        private readonly Dictionary<string, string> _deviceToBed
            = new Dictionary<string, string>();

        private readonly MedicalMonitorDbContext _db;

        public PatientBindingService(MedicalMonitorDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// 初始化床位绑定关系，从数据库加载所有患者信息并建立映射。
        /// </summary>
        public void InitializeDefaultBindings()
        {
            _bindings.Clear();
            _deviceToBed.Clear();

            var patientList = _db.Patients
                .OrderBy(x => x.BedNo)
                .ToList();

            foreach (PatientEntity entity in patientList)
            {
                var patientModel = new PatientModel
                {
                    Id = entity.PatientId.ToString(),
                    Name = entity.Name,
                    BedNo = entity.BedNo,
                    Age = entity.Age,
                    Gender = entity.Gender == "女" ? Models.Enums.Gender.Female : Models.Enums.Gender.Male,
                    Diagnosis = entity.Diagnosis,
                    AdmissionTime = entity.AdmissionTime,
                };

                if (!string.IsNullOrEmpty(entity.BoundDeviceId))
                {
                    _deviceToBed[entity.BoundDeviceId] = entity.BedNo;
                }
                _bindings[entity.BedNo] = patientModel;
            }
        }

        /// <summary>获取所有已绑定的设备 ID 列表</summary>
        public List<string> GetDeviceIds()
        {
            return _deviceToBed.Keys.OrderBy(k => k).ToList();
        }

        /// <summary>获取指定床位号绑定的设备 ID</summary>
        public string GetDeviceIdByBedNo(string bedNo)
        {
            return _deviceToBed.FirstOrDefault(kv => kv.Value == bedNo).Key;
        }

        /// <summary>获取指定床位的患者（空床返回 null）</summary>
        public PatientModel GetPatientByBedNo(string bedNo)
        {
            _bindings.TryGetValue(bedNo, out var patient);
            return patient;
        }

        /// <summary>根据设备ID查找床位号</summary>
        public string GetBedNoByDeviceId(string deviceId)
        {
            _deviceToBed.TryGetValue(deviceId, out var bedNo);
            return bedNo;
        }

        /// <summary>根据设备ID获取患者</summary>
        public PatientModel GetPatientByDeviceId(string deviceId)
        {
            var bedNo = GetBedNoByDeviceId(deviceId);
            return bedNo != null ? GetPatientByBedNo(bedNo) : null;
        }

        /// <summary>获取所有床位绑定列表</summary>
        public List<BedMappingModel> GetAllBindings()
        {
            var result = new List<BedMappingModel>();
            foreach (var kv in _deviceToBed)
            {
                result.Add(new BedMappingModel
                {
                    BedNo = kv.Value,
                    DeviceId = kv.Key,
                    Patient = GetPatientByBedNo(kv.Value),
                    IsOnline = true
                });
            }
            return result.OrderBy(b => b.BedNo).ToList();
        }

        /// <summary>绑定患者到指定床位</summary>
        public void BindPatient(string bedNo, PatientModel patient)
        {
            if (string.IsNullOrEmpty(bedNo) || patient == null)
                return;
            patient.BedNo = bedNo;
            _bindings[bedNo] = patient;
        }

        /// <summary>解绑指定床位的患者</summary>
        public void UnbindPatient(string bedNo)
        {
            if (_bindings.ContainsKey(bedNo))
                _bindings.Remove(bedNo);
        }
    }
}
