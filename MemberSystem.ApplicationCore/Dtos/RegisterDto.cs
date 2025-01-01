using System;

namespace MemberSystem.ApplicationCore.Dtos
{
    public class RegisterDto
    {
        public int MemberId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string BloodType { get; set; }
        public int RoleId { get; set; }
        public bool? IsApproved { get; set; }
    }
}
