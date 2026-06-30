using MedicalMonitor.WinForm.DAL.Entities;
using System.Collections.Generic;
using System.Linq;

namespace MedicalMonitor.WinForm.DAL
{
    /// <summary>
    /// 患者数据访问 — EF6 简单 CRUD。
    ///
    /// 使用 EF6 LINQ 处理单表增删改查，
    /// 复杂联表/聚合查询交给 VitalSignsQueryRepository（Dapper）。
    /// </summary>
    public class PatientRepository
    {
        private readonly MedicalMonitorDbContext _dbContext;

        public PatientRepository(MedicalMonitorDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>获取所有活跃患者列表</summary>
        public List<PatientEntity> GetAllActive()
        {
            return _dbContext.Patients
                .Where(p => p.IsActive)
                .OrderBy(p => p.BedNo)
                .ToList();
        }

        /// <summary>根据床位号获取患者</summary>
        public PatientEntity GetByBedNo(string bedNo)
        {
            return _dbContext.Patients
                .FirstOrDefault(p => p.BedNo == bedNo && p.IsActive);
        }

        /// <summary>根据患者ID获取</summary>
        public PatientEntity GetById(int patientId)
        {
            return _dbContext.Patients.Find(patientId);
        }

        /// <summary>新增患者</summary>
        public void Add(PatientEntity patient)
        {
            _dbContext.Patients.Add(patient);
            _dbContext.SaveChanges();
        }

        /// <summary>更新患者信息</summary>
        public void Update(PatientEntity patient)
        {
            _dbContext.Entry(patient).State = System.Data.Entity.EntityState.Modified;
            _dbContext.SaveChanges();
        }

        /// <summary>软删除患者（标记 IsActive=false）</summary>
        public void SoftDelete(int patientId)
        {
            var patient = _dbContext.Patients.Find(patientId);
            if (patient != null)
            {
                patient.IsActive = false;
                _dbContext.SaveChanges();
            }
        }
    }
}
