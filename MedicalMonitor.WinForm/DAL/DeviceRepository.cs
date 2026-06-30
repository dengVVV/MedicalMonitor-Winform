using MedicalMonitor.WinForm.DAL.Entities;
using System.Collections.Generic;
using System.Linq;

namespace MedicalMonitor.WinForm.DAL
{
    /// <summary>
    /// 设备数据访问 — EF6 简单 CRUD。
    /// </summary>
    public class DeviceRepository
    {
        private readonly MedicalMonitorDbContext _dbContext;

        public DeviceRepository(MedicalMonitorDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>获取所有设备列表</summary>
        public List<DeviceEntity> GetAll()
        {
            return _dbContext.Devices
                .OrderBy(d => d.DeviceId)
                .ToList();
        }

        /// <summary>根据设备ID获取</summary>
        public DeviceEntity GetByDeviceId(string deviceId)
        {
            return _dbContext.Devices
                .FirstOrDefault(d => d.DeviceId == deviceId);
        }

        /// <summary>更新设备在线状态</summary>
        public void UpdateOnlineStatus(string deviceId, bool isOnline)
        {
            var device = _dbContext.Devices
                .FirstOrDefault(d => d.DeviceId == deviceId);
            if (device != null)
            {
                device.IsOnline = isOnline;
                _dbContext.SaveChanges();
            }
        }

        /// <summary>新增设备</summary>
        public void Add(DeviceEntity device)
        {
            _dbContext.Devices.Add(device);
            _dbContext.SaveChanges();
        }
    }
}
