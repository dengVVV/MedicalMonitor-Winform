using System;
using System.ComponentModel.DataAnnotations;

namespace MedicalMonitor.WinForm.DAL.Entities
{
    /// <summary>
    /// 用户实体 — 对应 dbo.Users 表。
    /// DAL 层内部使用，不暴露到 BLL 或 UI 层。
    /// </summary>
    public class UserEntity
    {
        [Key]
        public int UserId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public string FullName { get; set; }
        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastLoginAt { get; set; }
    }
}
