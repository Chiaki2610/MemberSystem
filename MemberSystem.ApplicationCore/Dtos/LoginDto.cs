using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemberSystem.ApplicationCore.Dtos
{
    public class LoginDto
    {
        public int MemberId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public bool? IsApproved { get; set; }
        public int RoleId { get; set; }
    }
}
